using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{

    public class FaceData
    {
        public FaceData(Vector3[] _vertices, int[] _triangles)
        {
            Vertices = _vertices;
            Indices = _triangles;
        }

        public Vector3[] Vertices;
        public int[] Indices;
    }

    #region FaceData

    static readonly Vector3Int[] CheckDirections = new Vector3Int[]
    {
        Vector3Int.right,
        Vector3Int.left,
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.forward,
        Vector3Int.back
    };

    static readonly Vector3[] RightFace = new Vector3[]
    {
        new Vector3(.5f, -.5f, -.5f),
        new Vector3(.5f, -.5f, .5f),
        new Vector3(.5f, .5f, .5f),
        new Vector3(.5f, .5f, -.5f)
    };

    static readonly int[] RightTris = new int[]
    {
        0,2,1,0,3,2
    };

    static readonly Vector3[] LeftFace = new Vector3[]
    {
        new Vector3(-.5f, -.5f, -.5f),
        new Vector3(-.5f, -.5f, .5f),
        new Vector3(-.5f, .5f, .5f),
        new Vector3(-.5f, .5f, -.5f)
    };

    static readonly int[] LeftTris = new int[]
    {
        0,1,2,0,2,3
    };

    static readonly Vector3[] UpFace = new Vector3[]
    {
        new Vector3(-.5f, .5f, -.5f),
        new Vector3(-.5f, .5f, .5f),
        new Vector3(.5f, .5f, .5f),
        new Vector3(.5f, .5f, -.5f)
    };

    static readonly int[] UpTris = new int[]
    {
        0,1,2,0,2,3
    };

    static readonly Vector3[] DownFace = new Vector3[]
    {
        new Vector3(-.5f, -.5f, -.5f),
        new Vector3(-.5f, -.5f, .5f),
        new Vector3(.5f, -.5f, .5f),
        new Vector3(.5f, -.5f, -.5f)
    };

    static readonly int[] DownTris = new int[]
    {
        0,2,1,0,3,2
    };

    static readonly Vector3[] ForwardFace = new Vector3[]
    {
        new Vector3(-.5f, -.5f, .5f),
        new Vector3(-.5f, .5f, .5f),
        new Vector3(.5f, .5f, .5f),
        new Vector3(.5f, -.5f, .5f)
    };

    static readonly int[] ForwardTris = new int[]
    {
        0,2,1,0,3,2
    };

    static readonly Vector3[] BackFace = new Vector3[]
    {
        new Vector3(-.5f, -.5f, -.5f),
        new Vector3(-.5f, .5f, -.5f),
        new Vector3(.5f, .5f, -.5f),
        new Vector3(.5f, -.5f, -.5f)
    };

    static readonly int[] BackTris = new int[]
    {
        0,1,2,0,2,3
    };

    #endregion
    #region InitializeCubeFaces

    private Dictionary<Vector3Int, FaceData> cubeFaces = new Dictionary<Vector3Int, FaceData>();

    public MeshGenerator()
    {
        cubeFaces = new Dictionary<Vector3Int, FaceData>();

        for (int i = 0; i < CheckDirections.Length; i++)
        {
            if (CheckDirections[i] == Vector3Int.up)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(UpFace, UpTris));
            }
            else if (CheckDirections[i] == Vector3Int.down)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(DownFace, DownTris));
            }
            else if (CheckDirections[i] == Vector3Int.forward)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(ForwardFace, ForwardTris));
            }
            else if (CheckDirections[i] == Vector3Int.back)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(BackFace, BackTris));
            }
            else if (CheckDirections[i] == Vector3Int.left)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(LeftFace, LeftTris));
            }
            else if (CheckDirections[i] == Vector3Int.right)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(RightFace, RightTris));
            }
        }
    }

    #endregion

    public Mesh CreateNewMesh(WorldChunk _chunk)
    {
        Mesh _mesh = new Mesh();

        int _blocksPerChunk = _chunk.size;
        int _verticesPerChunk = _blocksPerChunk + 1;

        List<Vector3> _vertices = new List<Vector3>();
        List<int> _indices = new List<int>();

        for (int x = 0; x < _verticesPerChunk; x++) // loops over all dimensions
        {
            for (int y = 0; y < _verticesPerChunk; y++)
            {
                for (int z = 0; z < _verticesPerChunk; z++)
                {
                    bool _currentBlockIsSolid = _chunk.GetBlock(new Vector3Int(x, y, z)).solid;

                    for (int i = 0; i < CheckDirections.Length; i++)
                    {
                        bool _checkedBlockIsSolid = _chunk.GetBlock((new Vector3Int(x, y, z) + CheckDirections[i])).solid;
                        try
                        {
                            if (!_checkedBlockIsSolid)
                            {
                                if (_currentBlockIsSolid)
                                {
                                    FaceData _faceToApply = cubeFaces[CheckDirections[i]];

                                    foreach (Vector3 _vertice in _faceToApply.Vertices)
                                    {
                                        _vertices.Add(new Vector3(x, y, z) + _vertice);
                                    }

                                    foreach (int _triangle in _faceToApply.Indices)
                                    {
                                        _indices.Add(_vertices.Count - 4 + _triangle);
                                    }
                                }
                            }
                        }
                        catch (System.Exception e)
                        {
                            if (_currentBlockIsSolid)
                            {
                                FaceData _faceToApply = cubeFaces[CheckDirections[i]];

                                foreach (Vector3 _vertice in _faceToApply.Vertices)
                                {
                                    _vertices.Add(new Vector3(x, y, z) + _vertice);
                                }

                                foreach (int _triangle in _faceToApply.Indices)
                                {
                                    _indices.Add(_vertices.Count - 4 + _triangle);
                                }
                            }
                        }
                    }
                }
            }
        }

        _mesh.Clear();

        _mesh.SetVertices(_vertices);
        _mesh.SetIndices(_indices, MeshTopology.Triangles, 0);
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();
        //_mesh.Optimize();

        return _mesh;
    }
}
