using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    Vector3 position;
    int size;
    int[] chunkData;

    WorldChunk(Vector3 _position, int _size, int[] _chunkData)
    {
        position = _position;
        size = _size;
        chunkData = _chunkData;
    }

    /*public void UpdateChunk(Vector3 _position, int _newBlock)
    {

    }

    private void GetPositionInChunkData(Vector3 _position)
    {
        int _newPosition = 
    }*/
}
