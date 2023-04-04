using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public static WorldGenerator Instance;
    public static Dictionary<Vector3Int, WorldChunk> WorldChunks;
    public static Dictionary<Vector2Int, int[,]> WorldChunkColumns;
    public static Dictionary<Vector3Int, GameObject> ActiveChunks;

    static readonly public int BlocksPerChunk = 16;
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


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WorldChunks = new Dictionary<Vector3Int, WorldChunk>();
        WorldChunkColumns = new Dictionary<Vector2Int, int[,]>();
        ActiveChunks = new Dictionary<Vector3Int, GameObject>();

        meshCreator = new MeshGenerator(TextureLoaderInstance);

        VertecesPerChunk = BlocksPerChunk + 1;

        worldHeight = terrainOffset - worldHeight;

        for (int x = -2; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                for (int z = -2; z < 2; z++)
                {
                    CreateNewWorldChunk(new Vector3Int(x, y, z));
                }
            }
        }
    }

    public void CreateNewWorldChunk(Vector3Int _chunkCoord)
    {
        // a column is used to store data on where the ground should start
        Vector2Int _worldColumn = new Vector2Int(_chunkCoord.x, _chunkCoord.z);
        int[,] _newHeightData = WorldChunkColumns.ContainsKey(_worldColumn) ? WorldChunkColumns[_worldColumn] : GenerateColumnData(_worldColumn);

        // if the chunk already exists in WorldChunks, load it. Otherwise generate it
        WorldChunk _newChunkData = WorldChunks.ContainsKey(_chunkCoord) ? WorldChunks[_chunkCoord] : GenerateWorldChunk(_chunkCoord, _newHeightData);

        string _newChunkName = $"Chunk {_chunkCoord.x} {_chunkCoord.y} {_chunkCoord.z}";
        GameObject _newChunk = new GameObject(_newChunkName);
        _newChunk.transform.position = new Vector3(_chunkCoord.x * 16f, _chunkCoord.y * 16f, _chunkCoord.z * 16f);

        MeshFilter _newChunkFilter = _newChunk.AddComponent<MeshFilter>();
        _newChunkFilter.mesh = meshCreator.CreateNewMesh(_newChunkData);

        MeshRenderer _newChunkRenderer = _newChunk.AddComponent<MeshRenderer>();
        _newChunkRenderer.material = meshMaterial;

        MeshCollider _newChunkCollider = _newChunk.AddComponent<MeshCollider>();
        _newChunkCollider.sharedMesh = _newChunkFilter.mesh;

        ActiveChunks.Add(_chunkCoord, _newChunk);
    }

    int[,] GenerateColumnData(Vector2Int _columnPosition)
    {
        int[,] _groundHeights = new int[VertecesPerChunk, VertecesPerChunk];

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

        WorldChunkColumns.Add(_columnPosition, _groundHeights);
        return _groundHeights;
    }

    WorldChunk GenerateWorldChunk(Vector3Int _chunkPosition, int[,] _groundHeights)
    {
        Block[,,] _newChunkData = new Block[VertecesPerChunk, VertecesPerChunk, VertecesPerChunk]; // to the power of 3 because 3-dimensional

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

        WorldChunk _newChunk = new WorldChunk(_chunkPosition, BlocksPerChunk, _newChunkData);
        WorldChunks.Add(_chunkPosition, _newChunk);
        return _newChunk;
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