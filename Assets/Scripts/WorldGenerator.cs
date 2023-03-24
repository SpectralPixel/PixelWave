using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public static WorldGenerator Instance;

    [SerializeField] private float solidThreshold;

    public int BlocksPerChunk; // 16
    [HideInInspector] public int VertecesPerChunk; // BlocksPerChunk + 1
    public Dictionary<Vector3Int, WorldChunk> WorldChunks;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WorldChunks = new Dictionary<Vector3Int, WorldChunk>();
        VertecesPerChunk = BlocksPerChunk + 1;

        GenerateNewChunk(new Vector3Int(0, 0, 0), BlocksPerChunk);
    }

    void GenerateNewChunk(Vector3Int _chunkPosition, int _blocksPerChunk)
    {
        Block[] _newChunkData = new Block[VertecesPerChunk * VertecesPerChunk * VertecesPerChunk]; // to the power of 3 because 3-dimensional

        for (int i = 0, z = 0; z <= _blocksPerChunk; z++)
        {
            for (int y = 0; y <= _blocksPerChunk; y++)
            {
                for (int x = 0; x <= _blocksPerChunk; x++)
                {
                    float _randomNumber = Random.Range(0.0f, 1.0f);
                    _newChunkData[i] = new Block((_randomNumber < solidThreshold ? 1 : 0), new Vector3(x, y, z));
                    i++;
                }
            }
        }

        WorldChunk _newChunk = new WorldChunk(_chunkPosition, _blocksPerChunk, _newChunkData);
        WorldChunks.Add(_chunkPosition, _newChunk);
    }
}
