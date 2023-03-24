using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    Vector3Int position;
    int size;
    Block[] chunkData;

    public WorldChunk(Vector3Int _position, int _size, Block[] _chunkData)
    {
        position = _position;
        size = _size;
        chunkData = _chunkData;

        MeshGenerator.Instance.CreateNewMesh(size, chunkData);
    }

    public void UpdateChunk(Vector3Int _position, int _newBlock)
    {
        int _updatePosition = GetPositionInChunkData(_position);
        chunkData[_updatePosition] = new Block(_newBlock, _position);
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
