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

            _newChunk.layer = 6;
        }
    }

    public void UpdateChunk(Vector2Int _chunkCoord)
    {
        if (ActiveChunks.ContainsKey(_chunkCoord))
        {
            Vector3Int _dataCoords = new Vector3Int(_chunkCoord.x, 0, _chunkCoord.y);

            GameObject _targetChunk = ActiveChunks[_chunkCoord];
            MeshFilter _targetFilter = _targetChunk.GetComponent<MeshFilter>();
            MeshCollider _targetCollider = _targetChunk.GetComponent<MeshCollider>();

            StartCoroutine(meshCreator.CreateMeshFromData(WorldData[_dataCoords], x =>
            {
                _targetFilter.mesh = x;
                _targetCollider.sharedMesh = x;
            }));
        }
    }

    public void SetBlock(Vector3Int _worldPosition, int _blockType = 0)
    {
        Vector2Int _coords = GetChunkCoordsFromPosition(_worldPosition);
        Vector3Int _dataPosition = new Vector3Int(_coords.x, 0, _coords.y);

        if (WorldData.ContainsKey(_dataPosition))
        {
            Vector3Int _coordsToChange = WorldToLocalCoords(_worldPosition, _coords);
            WorldData[_dataPosition][_coordsToChange.x, _coordsToChange.y, _coordsToChange.z] = _blockType;
            UpdateChunk(_coords);
        }
    }

    private Vector2Int GetChunkCoordsFromPosition(Vector3 _worldPosition)
    {
        return new Vector2Int(
            Mathf.FloorToInt(_worldPosition.x / ChunkSize.x),
            Mathf.FloorToInt(_worldPosition.z / ChunkSize.z)
        );
    }

    private Vector3Int WorldToLocalCoords(Vector3Int _worldPosition, Vector2Int _coords)
    {
        return new Vector3Int 
        {
            x = _worldPosition.x - (_coords.x * ChunkSize.x),
            y = _worldPosition.y,
            z = _worldPosition.z - (_coords.y * ChunkSize.z)
        };
    }

}
