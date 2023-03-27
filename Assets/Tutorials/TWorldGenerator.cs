using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWorldGenerator : MonoBehaviour
{
    [SerializeField] private TTextureLoader TextureLoaderInstance;
    [Space]
    public static readonly Vector3Int ChunkSize = new Vector3Int(16, 256, 16);
    public Vector2 NoiseScale = Vector2.one;
    public Vector2 NoiseOffset = Vector2.zero;
    [Space]
    public int HeightOffset = 60;
    public float HeightIntensity = 5;
    [Space]
    public Material MeshMaterial;
    private int[,,] tempData; //int[,,] is a 3 dimensional int array

    void Start()
    {
        tempData = new int[ChunkSize.x, ChunkSize.y, ChunkSize.z];

        for (int x = 0; x < ChunkSize.x; x++)
        {
            for (int z = 0; z < ChunkSize.z; z++)
            {
                float PerlinCoordX = NoiseOffset.x + x / (float)ChunkSize.x * NoiseScale.x;
                float PerlinCoordY = NoiseOffset.y + z / (float)ChunkSize.z * NoiseScale.y;
                int HeightGen = Mathf.RoundToInt(Mathf.PerlinNoise(PerlinCoordX, PerlinCoordY) * HeightIntensity + HeightOffset);

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

                    tempData[x, y, z] = _blockTypeToAssign;
                }
            }
        }

        GameObject _tempChunk = new GameObject("Chunk", new System.Type[] { typeof(MeshFilter), typeof(MeshRenderer) });
        _tempChunk.GetComponent<MeshFilter>().mesh = new TChunkMeshCreator(TextureLoaderInstance).CreateMeshFromData(tempData);
        _tempChunk.GetComponent<MeshRenderer>().material = MeshMaterial;
    }

    /*private void OnDrawGizmos()
    {
        if (tempData != null)
        {
            for (int x = 0; x < ChunkSize.x; x++)
            {

                for (int y = 0; y < ChunkSize.y; y++)
                {

                    for (int z = 0; z < ChunkSize.z; z++)
                    {
                        switch(tempData[x, y, z])
                        {
                            case 0:
                                continue;
                            case 1:
                                Gizmos.color = Color.green;
                                break;
                            case 2:
                                Gizmos.color = Color.red;
                                break;
                            case 3:
                                Gizmos.color = Color.gray;
                                break;
                            case 4:
                                Gizmos.color = Color.black;
                                break;
                        }

                        Gizmos.DrawWireCube(new Vector3(x, y, z), Vector3.one);
                    }
                }
            }
        }
    }*/
}
