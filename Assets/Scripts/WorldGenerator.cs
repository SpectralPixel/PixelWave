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
    public MeshGenerator MeshCreator;
    public ColumnDataGenerator ColumnGenerator;
    public ChunkDataGenerator ChunkGenerator;

    void Start()
    {
        WorldChunks = new Dictionary<Vector3Int, WorldChunk>();
        WorldChunkColumns = new Dictionary<Vector2Int, int[,]>();
        ActiveChunks = new Dictionary<Vector3Int, GameObject>();

        MeshCreator = new MeshGenerator(TextureLoaderInstance, this);
        ColumnGenerator = new ColumnDataGenerator(this);
        ChunkGenerator = new ChunkDataGenerator(this);

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
            ColumnGenerator.QueueColumnToGenerate(new ColumnDataGenerator.ColumnGenData
            {
                GenerationPoint = _worldColumn,
                OnComplete = x => _newHeightData = x,
            });

            yield return new WaitUntil(() => _newHeightData != null);
        }

        if (_newChunkData == null) // if the chunkdata doesnt exists yet, generate it
        {
            //StartCoroutine(GenerateWorldChunk(_chunkCoord, _newHeightData, x => _newChunkData = x));
            ChunkGenerator.QueueDataToGenerate(new ChunkDataGenerator.ChunkGenData
            {
                GenerationPoint = _chunkCoord,
                GroundHeights = _newHeightData,
                OnComplete = x => _newChunkData = x,
            });

            yield return new WaitUntil(() => _newChunkData != null);
        }

        //StartCoroutine(meshCreator.CreateNewMesh(_newChunkData, x => _newChunkMesh = x));
        MeshCreator.QueueDataToDraw(new MeshGenerator.CreateMesh
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
}