using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TInfiniteTerrainGenerator : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private int renderDistance;
    private TWorldGenerator GeneratorInstance;
    private List<Vector2Int> CoordsToRemove;

    void Start()
    {
        GeneratorInstance = GetComponent<TWorldGenerator>();
        CoordsToRemove = new List<Vector2Int>();
    }

    void Update()
    {
        //converts player to chunk coords
        int _playerChunkX = (int)player.position.x / TWorldGenerator.ChunkSize.x;
        int _playerChunkZ = (int)player.position.z / TWorldGenerator.ChunkSize.z;
        CoordsToRemove.Clear();

        foreach(KeyValuePair<Vector2Int, GameObject> _activeChunk in TWorldGenerator.ActiveChunks)
        {
            CoordsToRemove.Add(_activeChunk.Key);
        }

        for (int x = _playerChunkX - renderDistance; x <= _playerChunkX + renderDistance; x++)
        {
            for (int y = _playerChunkZ - renderDistance; y <= _playerChunkZ + renderDistance; y++)
            {
                Vector2Int _chunkCoord = new Vector2Int(x, y);
                if (!TWorldGenerator.ActiveChunks.ContainsKey(_chunkCoord))
                {
                    StartCoroutine(GeneratorInstance.CreateChunk(_chunkCoord));
                }

                CoordsToRemove.Remove(_chunkCoord);
            }
        }

        foreach (Vector2Int _coord in CoordsToRemove)
        {
            GameObject _chunkToDelete = TWorldGenerator.ActiveChunks[_coord];
            TWorldGenerator.ActiveChunks.Remove(_coord);
            Destroy(_chunkToDelete);
        }

    }
}
