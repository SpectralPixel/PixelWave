using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrainGenerator : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private int renderDistance;
    private WorldGenerator GeneratorInstance;
    private List<Vector3Int> CoordsToRemove;

    void Start()
    {
        GeneratorInstance = GetComponent<WorldGenerator>();
        CoordsToRemove = new List<Vector3Int>();
    }

    void Update()
    {
        //converts player to chunk coords
        int _playerChunkX = (int)player.position.x / WorldGenerator.BlocksPerChunk;
        int _playerChunkY = (int)player.position.y / WorldGenerator.BlocksPerChunk;
        int _playerChunkZ = (int)player.position.z / WorldGenerator.BlocksPerChunk;
        CoordsToRemove.Clear();

        foreach (KeyValuePair<Vector3Int, GameObject> _activeChunk in WorldGenerator.ActiveChunks)
        {
            CoordsToRemove.Add(_activeChunk.Key);
        }

        for (int x = _playerChunkX - renderDistance; x <= _playerChunkX + renderDistance; x++)
        {
            for (int y = _playerChunkY - renderDistance; y <= _playerChunkY + renderDistance; y++)
            {
                for (int z = _playerChunkZ - renderDistance; z <= _playerChunkZ + renderDistance; z++)
                {
                    Vector3Int _chunkCoordinate = new Vector3Int(x, y, z);
                    if (!WorldGenerator.ActiveChunks.ContainsKey(_chunkCoordinate))
                    {
                        StartCoroutine(GeneratorInstance.CreateNewWorldChunk(_chunkCoordinate));
                    }

                    CoordsToRemove.Remove(_chunkCoordinate);
                }
            }
        }

        foreach (Vector3Int _coord in CoordsToRemove)
        {
            GameObject _chunkToDelete = WorldGenerator.ActiveChunks[_coord];
            WorldGenerator.ActiveChunks.Remove(_coord);
            Destroy(_chunkToDelete);
        }

    }
}
