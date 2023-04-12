using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkDataGenerator
{

    public class ChunkGenData
    {
        public System.Action<WorldChunk> OnComplete;
        public Vector3Int GenerationPoint;
        public int[,] GroundHeights;
    }

    private WorldGenerator generatorInstance;
    private Queue<ChunkGenData> dataToGenerate;
    private int chunkDataGenerating = 0;
    private int parallelGenerationCap;
    public bool Terminate;

    public ChunkDataGenerator(WorldGenerator _worldGenerator)
    {
        generatorInstance = _worldGenerator;
        parallelGenerationCap = generatorInstance.ParallelGenerationCap;
        dataToGenerate = new Queue<ChunkGenData>();

        _worldGenerator.StartCoroutine(DataGenLoop());
    }

    public void QueueDataToGenerate(ChunkGenData data)
    {
        dataToGenerate.Enqueue(data);
    }

    public IEnumerator DataGenLoop()
    {
        while (!Terminate)
        {
            if (dataToGenerate.Count > 0 && chunkDataGenerating < parallelGenerationCap)
            {
                ChunkGenData data = dataToGenerate.Dequeue();
                generatorInstance.StartCoroutine(GenerateWorldChunk(data.GenerationPoint, data.GroundHeights, data.OnComplete));
                chunkDataGenerating++;
                yield return new WaitUntil(() => data.OnComplete != null);
                chunkDataGenerating--;
            }

            yield return null;
        }
    }

    public IEnumerator GenerateWorldChunk(Vector3Int _chunkPosition, int[,] _groundHeights, System.Action<WorldChunk> _callback)
    {
        int _blocksPerChunk = WorldGenerator.BlocksPerChunk;
        int _verticesPerChunk = _blocksPerChunk + 1;

        int _worldHeight = generatorInstance.worldHeight;

        Block[,,] _newChunkData = new Block[_verticesPerChunk, _verticesPerChunk, _verticesPerChunk]; // to the power of 3 because 3-dimensional

        Task _task = Task.Factory.StartNew(delegate
        {
            for (int x = 0; x <= _blocksPerChunk; x++)
            {
                for (int y = 0; y <= _blocksPerChunk; y++)
                {
                    for (int z = 0; z <= _blocksPerChunk; z++)
                    {
                        int _groundHeight = LocalToWorldHeight(_groundHeights[x, z], _chunkPosition, _blocksPerChunk);
                        int _minimumHeight = LocalToWorldHeight(_worldHeight, _chunkPosition, _blocksPerChunk);
                        int _blockTypeToAssign = 0;

                        // create grass if at the top layer
                        if (y == _groundHeight) _blockTypeToAssign = 5;

                        // next 3 blocks dirt
                        if (y < _groundHeight && y > _groundHeight - 4) _blockTypeToAssign = 2;

                        // everything between dirt range (inclusive) and and 0 (exclusive) is stone
                        if (y <= _groundHeight - 4 && y > _minimumHeight) _blockTypeToAssign = 3;

                        // height 0 is bedrock
                        if (y == _minimumHeight) _blockTypeToAssign = 4;

                        _newChunkData[x, y, z] = new Block(_blockTypeToAssign, new Vector3Int(x, y, z));
                        if (y > _blocksPerChunk) _newChunkData[x, y, z] = new Block(_blockTypeToAssign, new Vector3Int(x, y, z));
                    }
                }
            }
        });

        yield return new WaitUntil(() =>
        {
            return _task.IsCompleted;
        });

        if (_task.Exception != null) Debug.LogError(_task.Exception);

        WorldChunk _newChunk = new WorldChunk(_chunkPosition, _blocksPerChunk, _newChunkData);
        WorldGenerator.WorldChunks[_chunkPosition] = _newChunk;
        _callback(_newChunk);
    }

    int LocalToWorldHeight(int _localHeight, Vector3Int _chunkPosition, int _chunkSize)
    {
        int _globalHeight = (-_chunkPosition.y * _chunkSize) + _localHeight;
        return _globalHeight;
    }

}
