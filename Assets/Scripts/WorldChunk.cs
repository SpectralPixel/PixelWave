using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk
{
    Vector3 position;
    Vector3 size;
    int[] chunkData;

    WorldChunk(Vector3 _position, Vector3 _size, int[] _chunkData)
    {
        position = _position;
        size = _size;
        chunkData = _chunkData;
    }
}
