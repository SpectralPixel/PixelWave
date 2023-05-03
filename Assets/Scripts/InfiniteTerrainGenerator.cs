using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class InfiniteTerrainGenerator : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private int renderDistance;
    private List<Vector3Int> ChunksToDelete;
    private WorldGenerator worldGeneratorInstance;
    private Vector3Int playerChunk;

    void Start()
    {
        ChunksToDelete = new List<Vector3Int>();
        worldGeneratorInstance = GetComponent<WorldGenerator>();

        StartCoroutine(ChunkStateUpdate());
    }

    private void Update()
    {
        //converts player to chunk coords
        playerChunk = new Vector3Int
        {
            x = Mathf.RoundToInt(player.position.x / WorldGenerator.BlocksPerChunk),
            y = Mathf.RoundToInt(player.position.y / WorldGenerator.BlocksPerChunk),
            z = Mathf.RoundToInt(player.position.z / WorldGenerator.BlocksPerChunk)
        };

        for (int y = playerChunk.y + 1; y >= playerChunk.y - 1; y--)
        {
            for (int z = playerChunk.z - 1; z <= playerChunk.z + 1; z++)
            {
                for (int x = playerChunk.x - 1; x <= playerChunk.x + 1; x++)
                {
                    Vector3Int _chunkCoord = new Vector3Int(x, y, z);
                    if (WorldGenerator.ActiveChunks.ContainsKey(_chunkCoord) && WorldGenerator.ActiveChunks[_chunkCoord].GetComponent<MeshCollider>() == null)
                    {
                        worldGeneratorInstance.CreateChunkCollider(_chunkCoord);
                    }
                }
            }
        }
    }

    IEnumerator ChunkStateUpdate()
    {
        while (true)
        {
            ChunksToDelete.Clear();

            foreach (KeyValuePair<Vector3Int, GameObject> _activeChunk in WorldGenerator.ActiveChunks)
            {
                ChunksToDelete.Add(_activeChunk.Key);
            }

            for (int y = playerChunk.y + renderDistance; y >= playerChunk.y - renderDistance; y--)
            {
                for (int z = playerChunk.z - renderDistance; z <= playerChunk.z + renderDistance; z++)
                {
                    for (int x = playerChunk.x - renderDistance; x <= playerChunk.x + renderDistance; x++)
                    {
                        Vector3Int _chunkCoord = new Vector3Int(x, y, z);

                        if (!WorldGenerator.ActiveChunks.ContainsKey(_chunkCoord))
                        {
                            StartCoroutine(worldGeneratorInstance.CreateNewWorldChunk(_chunkCoord));

                            yield return null;
                        }

                        ChunksToDelete.Remove(_chunkCoord);
                    }
                }
            }

            yield return null;

            foreach (Vector3Int _coord in ChunksToDelete)
            {
                GameObject _chunkToDelete = WorldGenerator.ActiveChunks[_coord];
                WorldGenerator.ActiveChunks.Remove(_coord);
                Destroy(_chunkToDelete);

                yield return null;
            }
        }
    }
}