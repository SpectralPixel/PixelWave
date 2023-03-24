using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    public Vector3Int Position;
    public Block[] ChunkData;
    int size;

    public WorldChunk(Vector3Int _position, int _size, Block[] _chunkData)
    {
        Position = _position;
        size = _size;
        ChunkData = _chunkData;

        MeshGenerator.Instance.CreateNewMesh(this);
    }

    public void UpdateChunk(Vector3Int _position, int _newBlock)
    {
        int _updatePosition = GetPositionInChunkData(_position);
        ChunkData[_updatePosition] = new Block(_newBlock, _position);
    }

    public int GetPositionInChunkData(Vector3Int _position)
    {
        int _newPosition = 0;

        _newPosition += _position.x;
        _newPosition += _position.y * size;
        _newPosition += _position.z * size * size;

        return _newPosition;
    }

    public Block GetBlock(Vector3Int _position)
    {
        return ChunkData[GetPositionInChunkData(_position)];
    }
}
