using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TChunkMeshCreator
{

    public class TFaceData
    {
        public TFaceData(Vector3[] _vertices, int[] _triangles)
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

    private Dictionary<Vector3Int, TFaceData> CubeFaces = new Dictionary<Vector3Int, TFaceData>();

    public TChunkMeshCreator()
    {
        CubeFaces = new Dictionary<Vector3Int, TFaceData>();

        for (int i = 0; i < CheckDirections.Length; i++)
        {
            if (CheckDirections[i] == Vector3Int.up) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(UpFace, UpTris));
            } else if (CheckDirections[i] == Vector3Int.down) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(DownFace, DownTris));
            } else if (CheckDirections[i] == Vector3Int.forward) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(ForwardFace, ForwardTris));
            } else if (CheckDirections[i] == Vector3Int.back) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(BackFace, BackTris));
            } else if (CheckDirections[i] == Vector3Int.left) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(LeftFace, LeftTris));
            } else if (CheckDirections[i] == Vector3Int.right) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(RightFace, RightTris));
            }
        }
    }

    public Mesh CreateMeshFromData(int[,,] _data)
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<int> _indices = new List<int>();

        Mesh _mesh = new Mesh();

        for (int x = 0; x < TWorldGenerator.ChunkSize.x; x++) // loops over all dimensions
        {
            for (int y = 0; y < TWorldGenerator.ChunkSize.y; y++)
            {
                for (int z = 0; z < TWorldGenerator.ChunkSize.z; z++)
                {
                    Vector3Int _blockPos = new Vector3Int(x, y, z);

                    for (int i = 0; i < CheckDirections.Length; i++)
                    {
                        Vector3Int _blockToCheck = _blockPos + CheckDirections[i];
                        try
                        {
                            if (_data[_blockToCheck.x, _blockToCheck.y, _blockToCheck.z] == 0)
                            {
                                if (_data[_blockPos.x, _blockPos.y, _blockPos.z] != 0)
                                {
                                    TFaceData _faceToApply = CubeFaces[CheckDirections[i]];

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
                            if (_data[_blockPos.x, _blockPos.y, _blockPos.z] != 0)
                            {
                                TFaceData _faceToApply = CubeFaces[CheckDirections[i]];

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

        _mesh.SetVertices(_vertices);
        _mesh.SetIndices(_indices, MeshTopology.Triangles, 0);
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();

        return _mesh;

    }

}
