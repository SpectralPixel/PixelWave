using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MeshGenerator : MonoBehaviour
{

    public static MeshGenerator Instance;

    [SerializeField] private Vector3 cubePos;
    [SerializeField] private Material meshMaterial;

    private Vector3[] meshVertices;
    private int[] meshTriangles;
    private int blocksPerChunk;
    private int vertecesPerChunk;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        blocksPerChunk = WorldGenerator.Instance.BlocksPerChunk;
        vertecesPerChunk = blocksPerChunk + 1;
        //CreateNewCube(size, cubePos);
    }

    public void CreateNewMesh(float _size, Block[] chunkData)
    {
        Mesh _mesh = new Mesh();

        meshVertices = new Vector3[vertecesPerChunk * vertecesPerChunk * vertecesPerChunk];
        for (int i = 0, z = 0; z <= blocksPerChunk; z++)
        {
            for (int y = 0; y <= blocksPerChunk; y++)
            {
                for (int x = 0; x <= blocksPerChunk; x++)
                {
                    meshVertices[i] = new Vector3(x, y, z);
                    i++;
                }
            }
        }

        meshTriangles = new int[blocksPerChunk * blocksPerChunk * blocksPerChunk * 36];
        int vertex = 0;
        int triangle = 0;
        int _forward = vertecesPerChunk * vertecesPerChunk;
        int _up = vertecesPerChunk;
        for (int i = 0, z = 0; z < blocksPerChunk; z++)
        {
            for (int y = 0; y < blocksPerChunk; y++)
            {
                for (int x = 0; x < blocksPerChunk; x++)
                {
                    if ((chunkData[i].blockID != 0))
                    {
                        // bottom
                        meshTriangles[triangle + 0] = vertex + 0;
                        meshTriangles[triangle + 1] = vertex + 1;
                        meshTriangles[triangle + 2] = vertex + 0 + _forward;
                        meshTriangles[triangle + 3] = vertex + 1;
                        meshTriangles[triangle + 4] = vertex + 1 + _forward;
                        meshTriangles[triangle + 5] = vertex + 0 + _forward;
                        // left
                        meshTriangles[triangle + 7] = vertex + 0 + _forward;
                        meshTriangles[triangle + 8] = vertex + 0 + _forward + _up;
                        meshTriangles[triangle + 6] = vertex + 0;
                        meshTriangles[triangle + 9] = vertex + 0;
                        meshTriangles[triangle + 10] = vertex + 0 + _forward + _up;
                        meshTriangles[triangle + 11] = vertex + 0 + _up;
                        // front
                        meshTriangles[triangle + 12] = vertex + 1 + _forward;
                        meshTriangles[triangle + 13] = vertex + 1 + _forward + _up;
                        meshTriangles[triangle + 14] = vertex + 0 + _forward;
                        meshTriangles[triangle + 15] = vertex + 0 + _forward;
                        meshTriangles[triangle + 16] = vertex + 1 + _forward + _up;
                        meshTriangles[triangle + 17] = vertex + 0 + _forward + _up;
                        // back
                        meshTriangles[triangle + 18] = vertex + 0;
                        meshTriangles[triangle + 19] = vertex + 0 + _up;
                        meshTriangles[triangle + 20] = vertex + 1;
                        meshTriangles[triangle + 21] = vertex + 1;
                        meshTriangles[triangle + 22] = vertex + 0 + _up;
                        meshTriangles[triangle + 23] = vertex + 1 + _up;
                        // right
                        meshTriangles[triangle + 24] = vertex + 1;
                        meshTriangles[triangle + 25] = vertex + 1 + _up;
                        meshTriangles[triangle + 26] = vertex + 1 + _forward;
                        meshTriangles[triangle + 27] = vertex + 1 + _forward;
                        meshTriangles[triangle + 28] = vertex + 1 + _up;
                        meshTriangles[triangle + 29] = vertex + 1 + _forward + _up;
                        // top
                        meshTriangles[triangle + 30] = vertex + 0 + _up;
                        meshTriangles[triangle + 31] = vertex + 0 + _forward + _up;
                        meshTriangles[triangle + 32] = vertex + 1 + _up;
                        meshTriangles[triangle + 33] = vertex + 1 + _up;
                        meshTriangles[triangle + 34] = vertex + 0 + _forward + _up;
                        meshTriangles[triangle + 35] = vertex + 1 + _forward + _up;
                    }

                    vertex++;
                    triangle += 36;
                    i++;
                }

                vertex++;
            }

            vertex += vertecesPerChunk;

            /*int[] _meshTriangles = new int[]
            {
                0, 1, 3,        1, 2, 3,        // Bottom	
                4, 5, 7,        5, 6, 7,        // Left
                8, 9, 11,       9, 10, 11,      // Front
                12, 13, 15,     13, 14, 15,     // Back
                16, 17, 19,     17, 18, 19,	    // Right
                20, 21, 23,     21, 22, 23,	    // Top
            };*/

        }

        UpdateMesh(_mesh);
    }

    void UpdateMesh(Mesh _mesh)
    {
        _mesh.Clear();

        _mesh.vertices = meshVertices;
        _mesh.triangles = meshTriangles;
        _mesh.RecalculateNormals();
        _mesh.Optimize();

        GameObject _meshObject = new GameObject("Mesh101");
        MeshFilter meshFilter = _meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshObject.transform.SetParent(gameObject.transform);

        meshFilter.mesh = _mesh;
        meshRenderer.material = meshMaterial;
    }

    void CreateNewCube(float _size, Vector3 _position)
    {
        // create cube: https://gist.github.com/prucha/866b9535d525adc984c4fe883e73a6c7
        // create plane: https://docs.unity.cn/560/Documentation/Manual/Example-CreatingaBillboardPlane.html

        Mesh _mesh = new Mesh();

        Vector3[] _meshVertexCoordinates = new Vector3[8];

        _meshVertexCoordinates[0] = new Vector3(-blocksPerChunk * 0.5f + _position.x, -blocksPerChunk * 0.5f + _position.y, -blocksPerChunk * 0.5f + _position.z); // * 0.5f to center the cube around the origin position
        _meshVertexCoordinates[1] = new Vector3(blocksPerChunk * 0.5f + _position.x, -blocksPerChunk * 0.5f + _position.y, -blocksPerChunk * 0.5f + _position.z);
        _meshVertexCoordinates[2] = new Vector3(blocksPerChunk * 0.5f + _position.x, -blocksPerChunk * 0.5f + _position.y, blocksPerChunk * 0.5f + _position.z);
        _meshVertexCoordinates[3] = new Vector3(-blocksPerChunk * 0.5f + _position.x, -blocksPerChunk * 0.5f + _position.y, blocksPerChunk * 0.5f + _position.z);
        _meshVertexCoordinates[4] = new Vector3(-blocksPerChunk * 0.5f + _position.x, blocksPerChunk * 0.5f + _position.y, -blocksPerChunk * 0.5f + _position.z);
        _meshVertexCoordinates[5] = new Vector3(blocksPerChunk * 0.5f + _position.x, blocksPerChunk * 0.5f + _position.y, -blocksPerChunk * 0.5f + _position.z);
        _meshVertexCoordinates[6] = new Vector3(blocksPerChunk * 0.5f + _position.x, blocksPerChunk * 0.5f + _position.y, blocksPerChunk * 0.5f + _position.z);
        _meshVertexCoordinates[7] = new Vector3(-blocksPerChunk * 0.5f + _position.x, blocksPerChunk * 0.5f + _position.y, blocksPerChunk * 0.5f + _position.z);

        Vector3[] _c = _meshVertexCoordinates;
        Vector3[] _meshVertices = new Vector3[]
        {
            _c[0], _c[1], _c[2], _c[3], // Bottom
	        _c[7], _c[4], _c[0], _c[3], // Left
	        _c[4], _c[5], _c[1], _c[0], // Front
	        _c[6], _c[7], _c[3], _c[2], // Back
	        _c[5], _c[6], _c[2], _c[1], // Right
	        _c[7], _c[6], _c[5], _c[4]  // Top
        };

        Vector3 _up = Vector3.up;
        Vector3 _down = Vector3.down;
        Vector3 _forward = Vector3.forward;
        Vector3 _back = Vector3.back;
        Vector3 _left = Vector3.left;
        Vector3 _right = Vector3.right;

        Vector3[] _meshNormals = new Vector3[]
        {
            _down,    _down,    _down,    _down,    // Bottom
	        _left,    _left,    _left,    _left,    // Left
	        _back,    _back,    _back,    _back,    // Back
	        _forward, _forward, _forward, _forward,	// Front
	        _right,   _right,   _right,   _right,   // Right
	        _up,      _up,      _up,      _up	    // Top
        };

        Vector2 _bottomLeft = new Vector2(0f, 0f);
        Vector2 _topLeft = new Vector2(1f, 0f);
        Vector2 _bottomRight = new Vector2(0f, 1f);
        Vector2 _topRight = new Vector2(1f, 1f);

        Vector2[] _meshUVs = new Vector2[]
        {
            _topRight, _bottomRight, _bottomLeft, _topLeft, // Bottom
	        _topRight, _bottomRight, _bottomLeft, _topLeft, // Left
	        _topRight, _bottomRight, _bottomLeft, _topLeft, // Front
	        _topRight, _bottomRight, _bottomLeft, _topLeft, // Back	        
	        _topRight, _bottomRight, _bottomLeft, _topLeft, // Right 
	        _topRight, _bottomRight, _bottomLeft, _topLeft  // Top
        };

        int[] _meshTriangles = new int[]
        {
            0, 1, 3,        1, 2, 3,        // Bottom	
	        4, 5, 7,        5, 6, 7,        // Left
	        8, 9, 11,       9, 10, 11,      // Front
	        12, 13, 15,     13, 14, 15,     // Back
	        16, 17, 19,     17, 18, 19,	    // Right
	        20, 21, 23,     21, 22, 23,	    // Top
        };

        /*int[] _meshTriangles = new int[]
        {
            0, 1, 3,        1, 2, 3,        // Bottom	
	        4, 5, 7,        5, 6, 7,        // Left
	        8, 9, 11,       9, 10, 11,      // Front
	        12, 13, 15,     13, 14, 15,     // Back
	        16, 17, 19,     17, 18, 19,	    // Right
	        20, 21, 23,     21, 22, 23,	    // Top
        };*/

        _mesh.vertices = _meshVertices; // add corners to mesh
        _mesh.triangles = _meshTriangles; // add triangles (everything gets rendered in triangles)
        _mesh.normals = _meshNormals; // makes sure mesh is lit properly
        _mesh.uv = _meshUVs; // allows for textures to be properly displayed
        _mesh.Optimize();

        GameObject _meshObject = new GameObject("Mesh101");
        MeshFilter meshFilter = _meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshObject.transform.SetParent(gameObject.transform);

        meshFilter.mesh = _mesh;
        meshRenderer.material = meshMaterial;
    }
    private void OnDrawGizmos()
    {
        if (meshVertices == null) return;

        for (int i = 0; i < meshVertices.Length; i++)
        {
            Gizmos.color = Color.HSVToRGB(((float)i / meshVertices.Length), 1f, 1f);
            Gizmos.DrawSphere(meshVertices[i], .1f);
        }
    }
}
