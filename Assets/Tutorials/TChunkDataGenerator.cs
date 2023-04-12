using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TChunkDataGenerator
{

    public class TGenData
    {
        public System.Action<int[,,]> OnComplete;
        public Vector3Int GenerationPoint;
    }

    private TWorldGenerator generatorInstance;
    private Queue<TGenData> dataToGenerate;
    private int chunkMeshesGenerating = 0;
    public bool Terminate;

    public TChunkDataGenerator(TWorldGenerator _worldGenerator)
    {
        generatorInstance = _worldGenerator;
        dataToGenerate = new Queue<TGenData>();

        _worldGenerator.StartCoroutine(DataGenLoop());
    }

    public void QueueDataToGenerate(TGenData data)
    {
        dataToGenerate.Enqueue(data);
    }

    public IEnumerator DataGenLoop()
    {
        while (!Terminate)
        {
            if (dataToGenerate.Count > 0 && chunkMeshesGenerating < 2)
            {
                TGenData data = dataToGenerate.Dequeue();
                generatorInstance.StartCoroutine(GenerateData(data.GenerationPoint, data.OnComplete));
                chunkMeshesGenerating++;
                yield return new WaitUntil(() => data.OnComplete != null);
                chunkMeshesGenerating--;
            }

            yield return null;
        }
    }

    public IEnumerator GenerateData(Vector3Int _offset, System.Action<int[,,]> _callback)
    {

        Vector3Int _chunkSize = TWorldGenerator.ChunkSize;
        Vector2 _noiseOffset = generatorInstance.NoiseOffset;
        Vector2 _noiseScale = generatorInstance.NoiseScale;

        float _heightIntensity = generatorInstance.HeightIntensity;
        float _heightOffset = generatorInstance.HeightOffset;

        int[,,] _tempData = new int[_chunkSize.x, _chunkSize.y, _chunkSize.z]; //int[,,] is a 3 dimensional int array

        Task _task = Task.Factory.StartNew(delegate
        {
            for (int x = 0; x < _chunkSize.x; x++)
            {
                for (int z = 0; z < _chunkSize.z; z++)
                {
                    float PerlinCoordX = _noiseOffset.x + (x + (_offset.x * 16f)) / (float)_chunkSize.x * _noiseScale.x;
                    float PerlinCoordY = _noiseOffset.y + (z + (_offset.z * 16f)) / (float)_chunkSize.z * _noiseScale.y;
                    int HeightGen = Mathf.RoundToInt(Mathf.PerlinNoise(PerlinCoordX, PerlinCoordY) * _heightIntensity + _heightOffset);

                    for (int y = HeightGen; y >= 0; y--)
                    {
                        int _blockTypeToAssign = 0;

                        // create grass
                        if (y == HeightGen) _blockTypeToAssign = 1;

                        // next 3 blocks dirt
                        if (y < HeightGen && y > HeightGen - 4) _blockTypeToAssign = 2;

                        // everything between dirt range (inclusive) and and 0 (exclusive) is stone
                        if (y <= HeightGen - 4 && y > 0) _blockTypeToAssign = 3;

                        // height 0 is bedrock
                        if (y == 0) _blockTypeToAssign = 4;

                        _tempData[x, y, z] = _blockTypeToAssign;
                    }
                }
            }
        });

        yield return new WaitUntil(() =>
        {
            return _task.IsCompleted;
        });

        if (_task.Exception != null) Debug.LogError(_task.Exception);

        TWorldGenerator.WorldData.Add(_offset, _tempData);
        _callback(_tempData);
    }
}
