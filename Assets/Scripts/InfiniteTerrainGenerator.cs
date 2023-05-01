using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrainGenerator : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private int renderDistance;
    private List<Vector3Int> ChunksToRemove;
    private WorldGenerator worldGeneratorInstance;

    private Vector3Int[] neighborVectors;

    void Start()
    {
        ChunksToRemove = new List<Vector3Int>();
        worldGeneratorInstance = GetComponent<WorldGenerator>();

        neighborVectors = ChunkUtils.NeighborVectors;
    }

    void Update()
    {
        //converts player to chunk coords
        int _playerChunkX = (int)player.position.x / WorldGenerator.BlocksPerChunk;
        int _playerChunkY = (int)player.position.y / WorldGenerator.BlocksPerChunk;
        int _playerChunkZ = (int)player.position.z / WorldGenerator.BlocksPerChunk;
        Vector3Int _playerChunk = new Vector3Int(_playerChunkX, _playerChunkY, _playerChunkZ);
        ChunksToRemove.Clear();

        foreach (KeyValuePair<Vector3Int, GameObject> _activeChunk in WorldGenerator.ActiveChunks)
        {
            ChunksToRemove.Add(_activeChunk.Key);
        }

        for (int y = _playerChunkY + renderDistance; y >= _playerChunkY - renderDistance; y--)
        {
            for (int z = _playerChunkZ - renderDistance; z <= _playerChunkZ + renderDistance; z++)
            {
                for (int x = _playerChunkX - renderDistance; x <= _playerChunkX + renderDistance; x++)
                {
                    Vector3Int _chunkCoord = new Vector3Int(x, y, z);
                    if (!WorldGenerator.ActiveChunks.ContainsKey(_chunkCoord))
                    {
                        StartCoroutine(worldGeneratorInstance.CreateNewWorldChunk(_chunkCoord));
                    }

                    ChunksToRemove.Remove(_chunkCoord);
                }
            }
        }



        for (int y = _playerChunkY + 1; y >= _playerChunkY - 1; y--)
        {
            for (int z = _playerChunkZ - 1; z <= _playerChunkZ + 1; z++)
            {
                for (int x = _playerChunkX - 1; x <= _playerChunkX + 1; x++)
                {
                    Vector3Int _chunkCoord = new Vector3Int(x, y, z);
                    if (WorldGenerator.ActiveChunks.ContainsKey(_chunkCoord) && WorldGenerator.ActiveChunks[_chunkCoord].GetComponent<MeshCollider>() == null)
                    {
                        worldGeneratorInstance.CreateChunkCollider(_chunkCoord);
                    }
                }
            }
        }

        foreach (Vector3Int _coord in ChunksToRemove)
        {
            GameObject _chunkToDelete = WorldGenerator.ActiveChunks[_coord];
            WorldGenerator.ActiveChunks.Remove(_coord);
            Destroy(_chunkToDelete);
        }
    }
}