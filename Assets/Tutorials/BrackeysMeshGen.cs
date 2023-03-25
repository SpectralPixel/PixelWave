using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
public class BrackeysMeshGen : MonoBehaviour
{

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int size;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        StartCoroutine(CreateShape());
    }

    private void Update()
    {
        UpdateMesh();
    }

    IEnumerator CreateShape()
    {
        vertices = new Vector3[(size + 1) * (size + 1)];

        for (int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[size * size * 6];
        int vertex = 0;
        int triangle = 0;
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                triangles[0 + triangle] = 0 + vertex;
                triangles[1 + triangle] = 1 + size + vertex;
                triangles[2 + triangle] = 1 + vertex;
                triangles[3 + triangle] = 1 + vertex;
                triangles[4 + triangle] = 1 + size + vertex;
                triangles[5 + triangle] = 2 + size + vertex;

                vertex++;
                triangle += 6;

                yield return new WaitForSeconds(0.002f);
            }

            vertex++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
