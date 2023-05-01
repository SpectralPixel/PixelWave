using UnityEngine;

// static because there will only ever be one of them
public static class ChunkUtils
{

    public static readonly Vector3Int[] NeighborVectors = new Vector3Int[]
    {
        Vector3Int.up,
        Vector3Int.forward,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.back,
        Vector3Int.left
    };

    public static Vector3Int WorldToChunkPos(Vector3 _worldPosition, int _chunkSize)
    {
        int _chunkPosX = Mathf.FloorToInt(_worldPosition.x / _chunkSize);
        int _chunkPosY = Mathf.FloorToInt(_worldPosition.y / _chunkSize);
        int _chunkPosZ = Mathf.FloorToInt(_worldPosition.z / _chunkSize);

        return new Vector3Int(_chunkPosX, _chunkPosY, _chunkPosZ);
    }

    public static Vector3Int WorldToLocalPosition(Vector3Int _worldPosition, Vector3Int _chunkCoords, int _chunkSize)
    {
        int _localXPosition = _worldPosition.x - (_chunkCoords.x * _chunkSize);
        int _localYPosition = _worldPosition.y - (_chunkCoords.y * _chunkSize);
        int _localZPosition = _worldPosition.z - (_chunkCoords.z * _chunkSize);

        return new Vector3Int(_localXPosition, _localYPosition, _localZPosition);
    }

    public static Vector3Int LocalToWorldPosition(Vector3Int _localPosition, Vector3Int _chunkPosition, int _chunkSize)
    {
        int _globalXPosition = (_chunkPosition.x * _chunkSize) + _localPosition.x;
        int _globalYPosition = (_chunkPosition.y * _chunkSize) + _localPosition.y;
        int _globalZPosition = (_chunkPosition.z * _chunkSize) + _localPosition.z;
        return new Vector3Int(_globalXPosition, _globalYPosition, _globalZPosition);
    }

    public static Vector2Int LocalToWorldPosition(Vector2Int _localPosition, Vector2Int _chunkPosition, int _chunkSize)
    {
        int _globalXPosition = (_chunkPosition.x * _chunkSize) + _localPosition.x;
        int _globalYPosition = (_chunkPosition.y * _chunkSize) + _localPosition.y;
        return new Vector2Int(_globalXPosition, _globalYPosition);
    }

    public static int LocalToWorldHeight(int _localHeight, Vector3Int _chunkPosition, int _chunkSize)
    {
        int _globalHeight = (-_chunkPosition.y * _chunkSize) + _localHeight;
        return _globalHeight;
    }
}