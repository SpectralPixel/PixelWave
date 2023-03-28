using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class TWorldGenerator : MonoBehaviour
{
    public static Dictionary<Vector3Int, int[,,]> WorldData;
    public static Dictionary<Vector2Int, GameObject> ActiveChunks;
    public static readonly Vector3Int ChunkSize = new Vector3Int(16, 256, 16);
    [Space]
    [SerializeField] private TTextureLoader TextureLoaderInstance;
    public Material _meshMaterial;
    [Space]
    public Vector2 NoiseScale = Vector2.one;
    public Vector2 NoiseOffset = Vector2.zero;
    [Space]
    public int HeightOffset = 60;
    public float HeightIntensity = 5;

    private TChunkMeshCreator meshCreator;

    void Start()
    {
        WorldData = new Dictionary<Vector3Int, int[,,]>();
        ActiveChunks = new Dictionary<Vector2Int, GameObject>();

        meshCreator = new TChunkMeshCreator(TextureLoaderInstance);

        for (int x = -2; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                CreateChunk(new Vector2Int(x, y));
            }
        }
    }

    public void CreateChunk(Vector2Int _chunkCoord)
    {
        Vector3Int _position = new Vector3Int(_chunkCoord.x, 0, _chunkCoord.y);
        int[,,] dataToApply = WorldData.ContainsKey(_position) ? WorldData[_position] : GenerateData(_position);

        string _newChunkName = $"Chunk {_chunkCoord.x} {_chunkCoord.y}";
        GameObject _newChunk = new GameObject(_newChunkName, new System.Type[]
        {
            typeof (MeshRenderer),
            typeof(MeshFilter),
            typeof (MeshCollider)
        });

        ////////////////////////////////////////////// https://youtu.be/Y8LCHU3hi2U?list=PLEvnA6UOOVbkOTaFPe4P0RA3W1mINbWCf&t=321
        MeshRenderer _newChunkRenderer = _newChunk.GetComponent<MeshRenderer>();
        MeshFilter _newChunkFilter = _newChunk.GetComponent<MeshFilter>();
        MeshCollider _newChunkCollider = _newChunk.GetComponent<MeshCollider>();

        _newChunk.transform.position = new Vector3(_chunkCoord.x * 16f, 0f, _chunkCoord.y * 16f);
        _newChunkRenderer.material = _meshMaterial;
        _newChunkFilter.mesh = meshCreator.CreateMeshFromData(dataToApply);
        _newChunkCollider.sharedMesh = _newChunkFilter.mesh;
        ActiveChunks.Add(_chunkCoord, _newChunk);
    }

    public int[,,] GenerateData(Vector3Int _offset)
    {
        int[,,] _tempData = new int[ChunkSize.x, ChunkSize.y, ChunkSize.z]; //int[,,] is a 3 dimensional int array

        for (int x = 0; x < ChunkSize.x; x++)
        {
            for (int z = 0; z < ChunkSize.z; z++)
            {
                float PerlinCoordX = NoiseOffset.x + (x + (_offset.x * 16f)) / (float)ChunkSize.x * NoiseScale.x;
                float PerlinCoordY = NoiseOffset.y + (z + (_offset.z * 16f)) / (float)ChunkSize.z * NoiseScale.y;
                int HeightGen = Mathf.RoundToInt(Mathf.PerlinNoise(PerlinCoordX, PerlinCoordY) * HeightIntensity + HeightOffset);

                for (int y = HeightGen; y >= 0; y--)
                {
                    int _blockTypeToAssign = 0;

                    // create grass
                    if (y == HeightGen) _blockTypeToAssign = 1;

                    // next 3 blocks dirt
                    if (y < HeightGen && y > HeightGen - 4) _blockTypeToAssign = 2;

                    // everything between dirt range (inclusive) and and 0 (exclusive) is stone
                    if (y <= HeightGen - 4 && y > 0) _blockTypeToAssign = 3;

                    // height 0 is bedrock
                    if (y == 0) _blockTypeToAssign = 4;

                    _tempData[x, y, z] = _blockTypeToAssign;
                }
            }
        }

        WorldData.Add(_offset, _tempData);
        return _tempData;
    }

}
