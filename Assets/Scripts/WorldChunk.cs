using UnityEngine;

public class WorldChunk
{
    public Vector3Int Position;
    public Block[,,] ChunkData;
    public int Size;

    private WorldChunk[] neighbors;
    private WorldGenerator generatorInstance;

    static readonly Vector3Int[] NeighborVectors = new Vector3Int[]
    {
        Vector3Int.up,
        Vector3Int.forward,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.back,
        Vector3Int.left
    };

    public WorldChunk(Vector3Int _position, int _size, Block[,,] _chunkData)
    {
        Position = _position;
        Size = _size;
        ChunkData = _chunkData;
        generatorInstance = WorldGenerator.Instance;

        neighbors = new WorldChunk[6]; // in 3d there are always 6 neighbors
        for (int i = 0; i < neighbors.Length; i += 2)
        {
            if (WorldGenerator.WorldChunks.ContainsKey(Position + NeighborVectors[i]))
            {
                neighbors[i] = WorldGenerator.WorldChunks[Position + NeighborVectors[i]];
            }
        }
    }

    public void UpdateBlock(Block _newBlock)
    {
        ChunkData[_newBlock.LocalPosition.x, _newBlock.LocalPosition.y, _newBlock.LocalPosition.z] = _newBlock;
        generatorInstance.UpdateChunkMesh(Position, ChunkData);

        for (int i = 0; i < NeighborVectors.Length; i++)
        {
            int _neighborX = _newBlock.LocalPosition.x + NeighborVectors[i].x;
            int _neighborY = _newBlock.LocalPosition.y + NeighborVectors[i].y;
            int _neighborZ = _newBlock.LocalPosition.z + NeighborVectors[i].z;

            bool _updateNeigbors = _neighborX <= 0 || _neighborY <= 0 || _neighborZ <= 0 || _neighborX >= (Size - 1) || _neighborY >= (Size - 1) || _neighborZ >= (Size - 1);
            if (_updateNeigbors)
            {
                Vector3Int _neighborPosition = Position + NeighborVectors[i];
                generatorInstance.UpdateChunkMesh(_neighborPosition, WorldGenerator.WorldChunks[_neighborPosition].ChunkData);
            }
        }
    }

    public Block GetBlock(Vector3Int _position)
    {
        try
        {
            return ChunkData[_position.x, _position.y, _position.z];
        }
        catch
        {
            try
            {
                Vector3Int _worldPosition = ChunkUtils.LocalToWorldPosition(_position, Position, Size);
                return WGetBlockFromNeighbor(_worldPosition);
            }
            catch
            {
                Debug.Log(_position);
                return new Block(0, new Vector3Int(0, 0, 0));
            }
        }
    }

    public Block GetBlockFromNeighbor(Vector3Int _neighbor, Vector3Int _relativePosition)
    {
        Vector3Int _worldPosition = ChunkUtils.LocalToWorldPosition(_relativePosition, Position, Size);
        Vector3Int _position = ChunkUtils.WorldToLocalPosition(_worldPosition, _neighbor, Size);

        WorldChunk _neighborChunk = WorldGenerator.WorldChunks[_neighbor];
        return _neighborChunk.GetBlock(_position);
    }

    public Block WGetBlockFromNeighbor(Vector3Int _worldPosition)
    {
        Vector3Int _neighbor = ChunkUtils.WorldToChunkPos(_worldPosition, Size);
        Vector3Int _position = ChunkUtils.WorldToLocalPosition(_worldPosition, _neighbor, Size);

        WorldChunk _neighborChunk = WorldGenerator.WorldChunks[_neighbor];
        return _neighborChunk.GetBlock(_position);
    }
}