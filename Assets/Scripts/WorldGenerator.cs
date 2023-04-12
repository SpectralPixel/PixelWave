using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public static WorldGenerator Instance;
    public static Dictionary<Vector3Int, WorldChunk> WorldChunks;
    public static Dictionary<Vector2Int, int[,]> WorldChunkColumns;
    public static Dictionary<Vector3Int, GameObject> ActiveChunks;

    static readonly public int BlocksPerChunk = 16;
    public int ParallelGenerationCap;
    [Space]
    public Vector2 noiseScale;
    public Vector2 noiseOffset;
    public int terrainOffset;
    public int worldHeight;
    public float heightIntensity;
    [Space]
    [SerializeField] private Material meshMaterial;

    [HideInInspector] public int VertecesPerChunk; // BlocksPerChunk + 1

    [SerializeField] private TextureLoader TextureLoaderInstance;
    private MeshGenerator meshCreator;
    private ColumnDataGenerator columnGenerator;
    private ChunkDataGenerator chunkGenerator;

    void Start()
    {
        WorldChunks = new Dictionary<Vector3Int, WorldChunk>();
        WorldChunkColumns = new Dictionary<Vector2Int, int[,]>();
        ActiveChunks = new Dictionary<Vector3Int, GameObject>();

        meshCreator = new MeshGenerator(TextureLoaderInstance, this);
        columnGenerator = new ColumnDataGenerator(this);
        chunkGenerator = new ChunkDataGenerator(this);

        VertecesPerChunk = BlocksPerChunk + 1;

        worldHeight = terrainOffset - worldHeight;

        /*for (int x = -2; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                for (int z = -2; z < 2; z++)
                {
                    CreateNewWorldChunk(new Vector3Int(x, y, z));
                }
            }
        }*/
    }

    public IEnumerator CreateNewWorldChunk(Vector3Int _chunkCoord)
    {
        string _newChunkName = $"Chunk {_chunkCoord.x} {_chunkCoord.y} {_chunkCoord.z}";
        GameObject _newChunk = new GameObject(_newChunkName, new System.Type[]
        {
            typeof(MeshRenderer),
            typeof(MeshFilter),
            typeof(MeshCollider)
        });

        _newChunk.transform.position = new Vector3(_chunkCoord.x * 16f, _chunkCoord.y * 16f, _chunkCoord.z * 16f);
        ActiveChunks.Add(_chunkCoord, _newChunk);

        Vector2Int _worldColumn = new Vector2Int(_chunkCoord.x, _chunkCoord.z);
        int[,] _newHeightData = WorldChunkColumns.ContainsKey(_worldColumn) ? WorldChunkColumns[_worldColumn] : null; // a column is used to store data on where the ground should start
        WorldChunk _newChunkData = WorldChunks.ContainsKey(_chunkCoord) ? WorldChunks[_chunkCoord] : null; // if the chunk already exists in WorldChunks, load it.
        Mesh _newChunkMesh = null;

        if (_newHeightData == null)
        {
            //StartCoroutine(GenerateColumnData(_worldColumn, x => _newHeightData = x));
            columnGenerator.QueueColumnToGenerate(new ColumnDataGenerator.ColumnGenData
            {
                GenerationPoint = _worldColumn,
                OnComplete = x => _newHeightData = x,
            });

            yield return new WaitUntil(() => _newHeightData != null);
        }

        if (_newChunkData == null) // if the chunkdata doesnt exists yet, generate it
        {
            //StartCoroutine(GenerateWorldChunk(_chunkCoord, _newHeightData, x => _newChunkData = x));
            chunkGenerator.QueueDataToGenerate(new ChunkDataGenerator.ChunkGenData
            {
                GenerationPoint = _chunkCoord,
                GroundHeights = _newHeightData,
                OnComplete = x => _newChunkData = x,
            });

            yield return new WaitUntil(() => _newChunkData != null);
        }

        //StartCoroutine(meshCreator.CreateNewMesh(_newChunkData, x => _newChunkMesh = x));
        meshCreator.QueueDataToDraw(new MeshGenerator.CreateMesh
        {
            DataToDraw = _newChunkData,
            OnComplete = x => _newChunkMesh = x // sets _newChunkMesh to x when x is returned
        });
        yield return new WaitUntil(() => _newChunkMesh != null); // waits until _newChunkMesh is not null before excecuting following code (aka waits until mesh has been generated)

        if (_newChunk != null)
        {
            MeshRenderer _newChunkRenderer = _newChunk.GetComponent<MeshRenderer>();
            MeshFilter _newChunkFilter = _newChunk.GetComponent<MeshFilter>();
            MeshCollider _newChunkCollider = _newChunk.GetComponent<MeshCollider>();

            _newChunkFilter.mesh = _newChunkMesh;
            _newChunkRenderer.material = meshMaterial;
            _newChunkCollider.sharedMesh = _newChunkFilter.mesh;
        }
    }

    public IEnumerator GenerateColumnData(Vector2Int _columnPosition, System.Action<int[,]> _callback)
    {
        int[,] _groundHeights = new int[VertecesPerChunk, VertecesPerChunk];

        Task _task = Task.Factory.StartNew(delegate
        {
            for (int x = 0; x <= BlocksPerChunk; x++)
            {
                for (int z = 0; z <= BlocksPerChunk; z++)
                {
                    float _perlinCoordX = noiseOffset.x + (x + (_columnPosition.x * 16f)) / BlocksPerChunk * noiseScale.x; // gets X coordinate for perlin noise function
                    float _perlinCoordZ = noiseOffset.y + (z + (_columnPosition.y * 16f)) / BlocksPerChunk * noiseScale.y;
                    int _groundHeight = Mathf.RoundToInt(Mathf.PerlinNoise(_perlinCoordX, _perlinCoordZ) * heightIntensity + terrainOffset); // gets a height to start generating at with perlin noise
                    _groundHeights[x, z] = _groundHeight;
                }
            }
        });

        yield return new WaitUntil(() =>
        {
            return _task.IsCompleted;
        });

        if (_task.Exception != null) Debug.LogError(_task.Exception);

        //WorldChunkColumns.Add(_columnPosition, _groundHeights);
        WorldChunkColumns[_columnPosition] = _groundHeights; //// temporary fix, column still being generated by all of the new chunks in a column
        _callback(_groundHeights);
    }

    public IEnumerator GenerateWorldChunk(Vector3Int _chunkPosition, int[,] _groundHeights, System.Action<WorldChunk> _callback)
    {
        Block[,,] _newChunkData = new Block[VertecesPerChunk, VertecesPerChunk, VertecesPerChunk]; // to the power of 3 because 3-dimensional

        Task _task = Task.Factory.StartNew(delegate
        {
            for (int x = 0; x <= BlocksPerChunk; x++)
            {
                for (int y = 0; y <= BlocksPerChunk; y++)
                {
                    for (int z = 0; z <= BlocksPerChunk; z++)
                    {
                        int _groundHeight = LocalToWorldHeight(_groundHeights[x, z], _chunkPosition);
                        int _minimumHeight = LocalToWorldHeight(worldHeight, _chunkPosition);
                        int _blockTypeToAssign = 0;

                        // create grass if at the top layer
                        if (y == _groundHeight) _blockTypeToAssign = 5;

                        // next 3 blocks dirt
                        if (y < _groundHeight && y > _groundHeight - 4) _blockTypeToAssign = 2;

                        // everything between dirt range (inclusive) and and 0 (exclusive) is stone
                        if (y <= _groundHeight - 4 && y > _minimumHeight) _blockTypeToAssign = 3;

                        // height 0 is bedrock
                        if (y == _minimumHeight) _blockTypeToAssign = 4;

                        _newChunkData[x, y, z] = new Block(_blockTypeToAssign, new Vector3Int(x, y, z));
                        if (y > BlocksPerChunk) _newChunkData[x, y, z] = new Block(_blockTypeToAssign, new Vector3Int(x, y, z));
                    }
                }
            }
        });

        yield return new WaitUntil(() =>
        {
            return _task.IsCompleted;
        });

        if (_task.Exception != null) Debug.LogError(_task.Exception);

        WorldChunk _newChunk = new WorldChunk(_chunkPosition, BlocksPerChunk, _newChunkData);
        WorldChunks[_chunkPosition] = _newChunk;
        _callback(_newChunk);
    }

    Vector3Int LocalToWorldPosition(Vector3Int _localPosition, Vector3Int _chunkPosition)
    {
        int _globalXPosition = (_chunkPosition.x * BlocksPerChunk) + _localPosition.x;
        int _globalYPosition = (_chunkPosition.y * BlocksPerChunk) + _localPosition.y;
        int _globalZPosition = (_chunkPosition.z * BlocksPerChunk) + _localPosition.z;
        return new Vector3Int(_globalXPosition, _globalYPosition, _globalZPosition);
    }

    int LocalToWorldHeight(int _localHeight, Vector3Int _chunkPosition)
    {
        int _globalHeight = (-_chunkPosition.y * BlocksPerChunk) + _localHeight;
        return _globalHeight;
    }
}