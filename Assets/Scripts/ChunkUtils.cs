using UnityEngine;

// static because there will only ever be one of them
public static class ChunkUtils
{

    public static (Vector3Int, Vector3Int) WorldToLocalAndChunkPosition(Vector3Int _worldPosition, int _chunkSize)
    {
        int _localXPosition = _worldPosition.x % _chunkSize;
        int _localYPosition = _worldPosition.y % _chunkSize;
        int _localZPosition = _worldPosition.z % _chunkSize;

        int _chunkXPosition = Mathf.FloorToInt(_worldPosition.x / _chunkSize);
        int _chunkYPosition = Mathf.FloorToInt(_worldPosition.y / _chunkSize);
        int _chunkZPosition = Mathf.FloorToInt(_worldPosition.z / _chunkSize);

        return (
            new Vector3Int(_localXPosition, _localYPosition, _localZPosition),
            new Vector3Int(_chunkXPosition, _chunkYPosition, _chunkZPosition)
        ); // https://www.grepper.com/answers/494992/c%23+return+2+values?ucard=1
    }

    public static Vector3Int WorldToLocalPosition(Vector3Int _worldPosition, int _chunkSize)
    {
        int _localXPosition = _worldPosition.x % _chunkSize;
        int _localYPosition = _worldPosition.y % _chunkSize;
        int _localZPosition = _worldPosition.z % _chunkSize;

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