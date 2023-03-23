using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public static MeshGenerator Instance;

    [SerializeField] private float size;
    [SerializeField] private Material meshMaterial;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateNewMesh(size, meshMaterial);
    }

    void CreateNewMesh(float _size, Material _meshMaterial)
    {
        // create cube: https://gist.github.com/prucha/866b9535d525adc984c4fe883e73a6c7
        // create plane: https://docs.unity.cn/560/Documentation/Manual/Example-CreatingaBillboardPlane.html

        Mesh _mesh = new Mesh();

        Vector3[] _meshVertexCoordinates = new Vector3[8];

        _meshVertexCoordinates[0] = new Vector3(-size * 0.5f, -size * 0.5f, -size * 0.5f); // * 0.5f to center the cube around the origin position
        _meshVertexCoordinates[1] = new Vector3( size * 0.5f, -size * 0.5f, -size * 0.5f);
        _meshVertexCoordinates[2] = new Vector3( size * 0.5f, -size * 0.5f, size * 0.5f );
        _meshVertexCoordinates[3] = new Vector3(-size * 0.5f, -size * 0.5f, size * 0.5f );
        _meshVertexCoordinates[4] = new Vector3(-size * 0.5f,  size * 0.5f, -size * 0.5f);
        _meshVertexCoordinates[5] = new Vector3( size * 0.5f,  size * 0.5f, -size * 0.5f);
        _meshVertexCoordinates[6] = new Vector3( size * 0.5f,  size * 0.5f, size * 0.5f );
        _meshVertexCoordinates[7] = new Vector3(-size * 0.5f,  size * 0.5f, size * 0.5f );

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
            3, 1, 0,        3, 2, 1,        // Bottom	
	        7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,	    // Top
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
        meshRenderer.material = _meshMaterial;
    }
}
