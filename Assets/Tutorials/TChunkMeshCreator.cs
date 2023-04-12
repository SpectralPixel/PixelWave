using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TChunkMeshCreator
{

    public class CreateMesh
    {
        public int[,,] DataToDraw;
        public System.Action<Mesh> OnComplete;
    }

    public class TFaceData
    {
        public TFaceData(Vector3[] _vertices, int[] _triangles, int[] _UVIndexOrder)
        {
            UVIndexOrder = _UVIndexOrder;
            Vertices = _vertices;
            Indices = _triangles;
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

    private Dictionary<Vector3Int, TFaceData> CubeFaces = new Dictionary<Vector3Int, TFaceData>();
    private TTextureLoader TextureLoaderInstance;
    private TWorldGenerator generator;
    private Queue<CreateMesh> meshesToCreate;
    private int chunkMeshesGenerating = 0;
    public bool Terminate;

    public TChunkMeshCreator(TTextureLoader _textureLoaderInstance, TWorldGenerator _worldGenerator)
    {
        CubeFaces = new Dictionary<Vector3Int, TFaceData>();
        TextureLoaderInstance = _textureLoaderInstance;
        meshesToCreate = new Queue<CreateMesh>();
        generator = _worldGenerator;

        for (int i = 0; i < CheckDirections.Length; i++)
        {
            if (CheckDirections[i] == Vector3Int.up) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(UpFace, UpTris, YUVOrder));
            } else if (CheckDirections[i] == Vector3Int.down) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(DownFace, DownTris, YUVOrder));
            } else if (CheckDirections[i] == Vector3Int.forward) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(ForwardFace, ForwardTris, ZUVOrder));
            } else if (CheckDirections[i] == Vector3Int.back) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(BackFace, BackTris, ZUVOrder));
            } else if (CheckDirections[i] == Vector3Int.left) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(LeftFace, LeftTris, XUVOrder));
            } else if (CheckDirections[i] == Vector3Int.right) {
                CubeFaces.Add(CheckDirections[i], new TFaceData(RightFace, RightTris, XUVOrder));
            }
        }

        generator.StartCoroutine(MeshGenLoop());
    }


    public void QueueDataToDraw(CreateMesh createMeshData)
    {
        meshesToCreate.Enqueue(createMeshData);
    }

    public IEnumerator MeshGenLoop()
    {
        while (!Terminate)
        {
            if (meshesToCreate.Count > 0 && chunkMeshesGenerating < 2)
            {
                CreateMesh createMesh = meshesToCreate.Dequeue(); // gets the mesh at the beginning of the queue and removes it from the queue

                generator.StartCoroutine(CreateMeshFromData(createMesh.DataToDraw, createMesh.OnComplete));
                chunkMeshesGenerating++;
                yield return new WaitUntil(() => createMesh.OnComplete != null);
                chunkMeshesGenerating--;
            }

            yield return null;
        }
    }


    public IEnumerator CreateMeshFromData(int[,,] _data, Action<Mesh> _meshCallback)
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<int> _indices = new List<int>();
        List<Vector2> _UVs = new List<Vector2>();

        Mesh _mesh = new Mesh();

        Task _task = Task.Factory.StartNew(delegate
        {
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
                                        int _currentBlockID = _data[_blockPos.x, _blockPos.y, _blockPos.z];
                                        TTextureLoader.TCubeTexture _textureToApply = TextureLoaderInstance.Textures[_currentBlockID];

                                        TFaceData _faceToApply = CubeFaces[CheckDirections[i]];

                                        foreach (Vector3 _vertex in _faceToApply.Vertices)
                                        {
                                            _vertices.Add(new Vector3(x, y, z) + _vertex);
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
                                if (_data[_blockPos.x, _blockPos.y, _blockPos.z] != 0)
                                {
                                    int _currentBlockID = _data[_blockPos.x, _blockPos.y, _blockPos.z];
                                    TTextureLoader.TCubeTexture _textureToApply = TextureLoaderInstance.Textures[_currentBlockID];

                                    TFaceData _faceToApply = CubeFaces[CheckDirections[i]];

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

        _mesh.SetVertices(_vertices);
        _mesh.SetIndices(_indices, MeshTopology.Triangles, 0);
        _mesh.SetUVs(0, _UVs);

        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
        _mesh.RecalculateNormals();

        _meshCallback(_mesh);

    }

}
