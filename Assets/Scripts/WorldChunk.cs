using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    Vector3Int position;
    int size;
    int[] chunkData;

    public WorldChunk(Vector3Int _position, int _size, int[] _chunkData)
    {
        position = _position;
        size = _size;
        chunkData = _chunkData;

        MeshGenerator.Instance.CreateNewMesh(size, chunkData);
    }

    public void UpdateChunk(Vector3Int _position, int _newBlock)
    {
        int _updatePosition = GetPositionInChunkData(_position);
        chunkData[_updatePosition] = _newBlock;
    }
    public void UpdateChunk(int _position, int _newBlock)
    {
        chunkData[_position] = _newBlock;
    }

    private int GetPositionInChunkData(Vector3Int _position)
    {
        int _newPosition = 0;
    
        _newPosition += _position.x;
        _newPosition += _position.y * size;
        _newPosition += _position.z * size * size;

        return _newPosition;
    }
}
