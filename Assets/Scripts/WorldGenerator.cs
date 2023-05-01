using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public static WorldGenerator Instance;
    public static Dictionary<Vector3Int, WorldChunk> WorldChunks;
    public static Dictionary<Vector2Int, int[,]> WorldChunkColumns;
    public static Dictionary<Vector3Int, GameObject> ActiveChunks;

    private List<Vector3Int> ChunksWithMeshes;

    static readonly public int BlocksPerChunk = 16;
    public int ParallelGenerationCap;
    [Space]
    public Vector2 NoiseScale;
    public Vector2 NoiseOffset;
    public int TerrainOffset;
    public int WorldHeight;
    public float HeightIntensity;
    [Space]
    [SerializeField] private Material meshMaterial;

    [HideInInspector] public int VertecesPerChunk; // BlocksPerChunk + 1

    [SerializeField] private TextureLoader TextureLoaderInstance;
    public MeshGenerator MeshCreator;
    public ColumnDataGenerator ColumnGenerator;
    public ChunkDataGenerator ChunkGenerator;

    private void Awake() => Instance = this;

    void Start()
    {
        WorldChunks = new Dictionary<Vector3Int, WorldChunk>();
        WorldChunkColumns = new Dictionary<Vector2Int, int[,]>();
        ActiveChunks = new Dictionary<Vector3Int, GameObject>();

        ChunksWithMeshes = new List<Vector3Int>();

        MeshCreator = new MeshGenerator(TextureLoaderInstance, this);
        ColumnGenerator = new ColumnDataGenerator(this);
        ChunkGenerator = new ChunkDataGenerator(this);

        VertecesPerChunk = BlocksPerChunk + 1;

        WorldHeight = TerrainOffset - WorldHeight;
    }

    public IEnumerator CreateNewWorldChunk(Vector3Int _chunkCoord)
    {
        string _newChunkName = $"Chunk {_chunkCoord.x} {_chunkCoord.y} {_chunkCoord.z}";
        GameObject _newChunk = new GameObject(_newChunkName, new System.Type[]
        {
            typeof(MeshRenderer),
            typeof(MeshFilter)
        });

        _newChunk.transform.position = new Vector3(_chunkCoord.x * 16f, _chunkCoord.y * 16f, _chunkCoord.z * 16f);
        ActiveChunks.Add(_chunkCoord, _newChunk);

        Vector2Int _worldColumn = new Vector2Int(_chunkCoord.x, _chunkCoord.z);
        int[,] _newHeightData = WorldChunkColumns.ContainsKey(_worldColumn) ? WorldChunkColumns[_worldColumn] : null; // a column is used to store data on where the ground should start
        WorldChunk _newChunkData = WorldChunks.ContainsKey(_chunkCoord) ? WorldChunks[_chunkCoord] : null; // if the chunk already exists in WorldChunks, load it.
        Mesh _newChunkMesh = null;

        if (_newHeightData == null)
        {
            ColumnGenerator.QueueColumnToGenerate(new ColumnDataGenerator.ColumnGenData
            {
                GenerationPoint = _worldColumn,
                OnComplete = x => _newHeightData = x,
            });

            yield return new WaitUntil(() => _newHeightData != null);
        }

        if (_newChunkData == null) // if the chunkdata doesnt exists yet, generate it
        {
            ChunkGenerator.QueueDataToGenerate(new ChunkDataGenerator.ChunkGenData
            {
                GenerationPoint = _chunkCoord,
                GroundHeights = _newHeightData,
                OnComplete = x => _newChunkData = x,
            });

            yield return new WaitUntil(() => _newChunkData != null);
        }

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

            _newChunkRenderer.material = meshMaterial;
            _newChunkFilter.mesh = _newChunkMesh;
            ChunksWithMeshes.Add(_chunkCoord);

            _newChunk.layer = 6; // chunk layer
        }
    }

    public void CreateChunkCollider(Vector3Int _chunkCoord)
    {
        GameObject _currentChunk = ActiveChunks[_chunkCoord];
        MeshFilter _meshFilter = _currentChunk.GetComponent<MeshFilter>();
        if (ChunksWithMeshes.Contains(_chunkCoord))
        {
            MeshCollider _meshCollider = _currentChunk.AddComponent<MeshCollider>();
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }
    }

    public void UpdateChunkMesh(Vector3Int _chunkPosition, Block[,,] _newChunkData) /////////////////////////////////////////////// UPDATE NEIGHBORING CHUNKS IF UPDATE IS ON BORDER
    {
        WorldChunks[_chunkPosition] = new WorldChunk(_chunkPosition, BlocksPerChunk, _newChunkData);

        GameObject _targetChunk = ActiveChunks[_chunkPosition];
        MeshFilter _targetFilter = _targetChunk.GetComponent<MeshFilter>();

        if (_targetChunk.GetComponent<MeshCollider>() == null) CreateChunkCollider(_chunkPosition);
        MeshCollider _targetCollider = _targetChunk.GetComponent<MeshCollider>();

        StartCoroutine(MeshCreator.CreateNewMesh(WorldChunks[_chunkPosition], x =>
        {
            _targetFilter.mesh = x;
            _targetCollider.sharedMesh = x;
        }));
    }
}