using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    public Vector3Int Position;
    public Block[,,] ChunkData;
    public int size;

    public WorldChunk(Vector3Int _position, int _size, Block[,,] _chunkData)
    {
        Position = _position;
        size = _size;
        ChunkData = _chunkData;
    }

    public void UpdateChunk(Vector3Int _position, int _newBlock)
    {
        ChunkData[_position.x, _position.y, _position.z] = new Block(_newBlock, _position);
    }

    public Block GetBlock(Vector3Int _position)
    {
        return ChunkData[_position.x, _position.y, _position.z];
    }
}
