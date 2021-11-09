using UnityEngine;

public class planetMesh : MonoBehaviour
{
    //mesh
    private Mesh mesh;
    public int width;
    public int height;
    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));

        CreateMesh();
    }

    private void CreateMesh()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];
        triangles = new int[width * height * 6];

        SetVertices();
        CreateTriangles();
        UpdateMesh();
    }

    private void SetVertices()
    {
        for (int index = 0, z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                vertices[index] = new Vector3(x, 0, z);

                index++;
            }
        }
    }

    private void CreateTriangles()
    {
        for (int index = 0, triangleIndex = 0, z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[triangleIndex] = index;
                triangles[triangleIndex + 1] = index + width + 1;
                triangles[triangleIndex + 2] = index + 1;
                triangles[triangleIndex + 3] = index + 1;
                triangles[triangleIndex + 4] = index + width + 1;
                triangles[triangleIndex + 5] = index + width + 2;

                triangleIndex += 6;
                index++;
            }

            index++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
