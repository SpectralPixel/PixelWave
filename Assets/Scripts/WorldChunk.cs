using UnityEngine;

public class WorldChunk
{
    public Vector3Int Position;
    public Block[,,] ChunkData;
    public int size;

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
        size = _size;
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

    public void UpdateBlock(Vector3Int _position, int _newBlock)
    {
        ChunkData[_position.x, _position.y, _position.z] = new Block(_newBlock, _position);
        generatorInstance.UpdateChunkMesh(Position, ChunkData);
    }

    public Block GetBlock(Vector3Int _position)
    {
        try
        {
            return ChunkData[_position.x, _position.y, _position.z];
        }
        catch (System.Exception e)
        {
            ///Debug.LogWarning(e);
            return new Block(0, new Vector3Int(0, 0, 0));
        }
    }

    public Block GetBlockFromNeighbor(Vector3Int _neighbor, Vector3Int _relativePosition)
    {
        Vector3Int _globalPosition = ChunkUtils.LocalToWorldPosition(_relativePosition, Position, size);
        Vector3Int _position = ChunkUtils.WorldToLocalPosition(_globalPosition, Position, size);

        WorldChunk _neighborChunk = WorldGenerator.WorldChunks[_neighbor];
        return _neighborChunk.GetBlock(_position);
    }
}