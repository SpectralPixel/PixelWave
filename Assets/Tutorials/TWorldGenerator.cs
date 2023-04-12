using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TWorldGenerator : MonoBehaviour
{
    public static Dictionary<Vector3Int, int[,,]> WorldData;
    public static Dictionary<Vector2Int, GameObject> ActiveChunks;
    public static readonly Vector3Int ChunkSize = new Vector3Int(16, 256, 16);
    [Space]
    [SerializeField] private TTextureLoader TextureLoaderInstance;
    public Material meshMaterial;
    [Space]
    public Vector2 NoiseScale = Vector2.one;
    public Vector2 NoiseOffset = Vector2.zero;
    [Space]
    public int HeightOffset = 60;
    public float HeightIntensity = 5;

    private TChunkMeshCreator meshCreator;
    private TChunkDataGenerator chunkDataGenerator;

    void Start()
    {
        WorldData = new Dictionary<Vector3Int, int[,,]>();
        ActiveChunks = new Dictionary<Vector2Int, GameObject>();

        meshCreator = new TChunkMeshCreator(TextureLoaderInstance, this);
        chunkDataGenerator = new TChunkDataGenerator(this);
    }

    public IEnumerator CreateChunk(Vector2Int _chunkCoord)
    {
        Vector3Int _position = new Vector3Int(_chunkCoord.x, 0, _chunkCoord.y);

        string _newChunkName = $"Chunk {_chunkCoord.x} {_chunkCoord.y}";
        GameObject _newChunk = new GameObject(_newChunkName, new System.Type[]
        {
            typeof(MeshRenderer),
            typeof(MeshFilter),
            typeof(MeshCollider)
        });

        _newChunk.transform.position = new Vector3(_chunkCoord.x * 16f, 0f, _chunkCoord.y * 16f);
        ActiveChunks.Add(_chunkCoord, _newChunk);
        
        int[,,] _dataToApply = WorldData.ContainsKey(_position) ? WorldData[_position] : null;
        Mesh _meshToUse = null;

        if (_dataToApply == null)
        {
            chunkDataGenerator.QueueDataToGenerate(new TChunkDataGenerator.TGenData
            {
                GenerationPoint = _position,
                OnComplete = x => _dataToApply = x // sets _dataToApply to x when x is returned
            });

            yield return new WaitUntil(() => _dataToApply != null);
        }

        meshCreator.QueueDataToDraw(new TChunkMeshCreator.CreateMesh
        {
            DataToDraw = _dataToApply,
            OnComplete = x => _meshToUse = x
        });
        yield return new WaitUntil(() => _meshToUse != null); // waits until _meshToUse is not null before excecuting following code (aka waits until mesh has been generated)

        if (_newChunk != null)
        {
            MeshRenderer _newChunkRenderer = _newChunk.GetComponent<MeshRenderer>();
            MeshFilter _newChunkFilter = _newChunk.GetComponent<MeshFilter>();
            MeshCollider _newChunkCollider = _newChunk.GetComponent<MeshCollider>();

            _newChunkFilter.mesh = _meshToUse;
            _newChunkRenderer.material = meshMaterial;
            _newChunkCollider.sharedMesh = _newChunkFilter.mesh;
        }
    }
}
