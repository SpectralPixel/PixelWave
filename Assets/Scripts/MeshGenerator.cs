using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MeshGenerator
{

    public class FaceData
    {
        public FaceData(Vector3[] _vertices, int[] _triangles, int[] _UVIndexOrder)
        {
            Vertices = _vertices;
            Indices = _triangles;
            UVIndexOrder = _UVIndexOrder;
        }

        public Vector3[] Vertices;
        public int[] Indices;
        public int[] UVIndexOrder;
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
    #region FaceUVData

    static readonly int[] XUVOrder = new int[]
    {
     2, 3, 1, 0
    };

    static readonly int[] YUVOrder = new int[]
    {
      0, 1, 3, 2
    };


    static readonly int[] ZUVOrder = new int[]
    {
      3, 1, 0, 2
    };


    #endregion
    #region InitializeCubeFaces

    private Dictionary<Vector3Int, FaceData> cubeFaces = new Dictionary<Vector3Int, FaceData>();
    private TextureLoader textureLoaderInstance;

    public MeshGenerator(TextureLoader _textureLoaderInstance)
    {
        cubeFaces = new Dictionary<Vector3Int, FaceData>();
        textureLoaderInstance = _textureLoaderInstance;

        for (int i = 0; i < CheckDirections.Length; i++)
        {
            if (CheckDirections[i] == Vector3Int.up)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(UpFace, UpTris, YUVOrder));
            }
            else if (CheckDirections[i] == Vector3Int.down)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(DownFace, DownTris, YUVOrder));
            }
            else if (CheckDirections[i] == Vector3Int.forward)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(ForwardFace, ForwardTris, ZUVOrder));
            }
            else if (CheckDirections[i] == Vector3Int.back)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(BackFace, BackTris, ZUVOrder));
            }
            else if (CheckDirections[i] == Vector3Int.left)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(LeftFace, LeftTris, XUVOrder));
            }
            else if (CheckDirections[i] == Vector3Int.right)
            {
                cubeFaces.Add(CheckDirections[i], new FaceData(RightFace, RightTris, XUVOrder));
            }
        }
    }

    #endregion

    public IEnumerator CreateNewMesh(WorldChunk _chunk, Action<Mesh> _callback)
    {
        Mesh _mesh = new Mesh();

        int _blocksPerChunk = _chunk.size;
        int _verticesPerChunk = _blocksPerChunk + 1;

        List<Vector3> _vertices = new List<Vector3>();
        List<int> _indices = new List<int>();
        List<Vector2> _UVs = new List<Vector2>();

        Task _task = Task.Factory.StartNew(delegate
        {
            for (int x = 0; x < _verticesPerChunk; x++) // loops over all dimensions
            {
                for (int y = 0; y < _verticesPerChunk; y++)
                {
                    for (int z = 0; z < _verticesPerChunk; z++)
                    {
                        Vector3Int _currentBlockPosition = new Vector3Int(x, y, z);
                        bool _currentBlockIsSolid = _chunk.GetBlock(_currentBlockPosition).IsSolid;

                        for (int i = 0; i < CheckDirections.Length; i++)
                        {
                            bool _checkedBlockIsSolid = _chunk.GetBlock((_currentBlockPosition + CheckDirections[i])).IsSolid;
                            try
                            {
                                if (!_checkedBlockIsSolid)
                                {
                                    if (_currentBlockIsSolid)
                                    {
                                        int _currentBlockID = _chunk.GetBlock(_currentBlockPosition).BlockID;
                                        TextureLoader.CubeTexture _textureToApply = textureLoaderInstance.Textures[_currentBlockID];

                                        FaceData _faceToApply = cubeFaces[CheckDirections[i]];

                                        foreach (Vector3 _vertice in _faceToApply.Vertices)
                                        {
                                            _vertices.Add(new Vector3(x, y, z) + _vertice);
                                        }

                                        foreach (int _triangle in _faceToApply.Indices)
                                        {
                                            _indices.Add(_vertices.Count - 4 + _triangle);
                                        }

                                        Vector2[] _UVsToAdd = _textureToApply.GetUVsAtDirectionT(CheckDirections[i]);
                                        foreach (int _UVIndex in _faceToApply.UVIndexOrder)
                                        {
                                            _UVs.Add(_UVsToAdd[_UVIndex]);
                                        }
                                    }
                                }
                            }
                            catch (System.Exception e)
                            {
                                if (_currentBlockIsSolid)
                                {
                                    int _currentBlockID = _chunk.GetBlock(_currentBlockPosition).BlockID;
                                    TextureLoader.CubeTexture _textureToApply = textureLoaderInstance.Textures[_currentBlockID];

                                    FaceData _faceToApply = cubeFaces[CheckDirections[i]];

                                    foreach (Vector3 _vertice in _faceToApply.Vertices)
                                    {
                                        _vertices.Add(new Vector3(x, y, z) + _vertice);
                                    }

                                    foreach (int _triangle in _faceToApply.Indices)
                                    {
                                        _indices.Add(_vertices.Count - 4 + _triangle);
                                    }

                                    Vector2[] _UVsToAdd = _textureToApply.GetUVsAtDirectionT(CheckDirections[i]);
                                    foreach (int _UVIndex in _faceToApply.UVIndexOrder)
                                    {
                                        _UVs.Add(_UVsToAdd[_UVIndex]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        yield return new WaitUntil(() =>
        {
            return _task.IsCompleted;
        });

        if (_task.Exception != null) Debug.LogError(_task.Exception);

        _mesh.Clear();

        _mesh.SetVertices(_vertices);
        _mesh.SetIndices(_indices, MeshTopology.Triangles, 0);
        _mesh.SetUVs(0, _UVs);

        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();
        //_mesh.Optimize();

        _callback(_mesh);
    }
}
