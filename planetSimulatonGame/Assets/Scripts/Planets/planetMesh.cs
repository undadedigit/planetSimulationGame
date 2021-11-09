using UnityEngine;

struct CubeFaceVectors
{
    public Vector3 position;
    public Vector3 rotate;

    public CubeFaceVectors(Vector3 position, Vector3 rotate)
    {
        this.position = position;
        this.rotate = rotate;
    }
}

public class planetMesh : MonoBehaviour
{
    //mesh
    private Mesh mesh;
    public int xRes;
    public int zRes;
    public int size;
    private int resolution;
    private Vector3[] vertices;
    private int[] triangles;
    //face to cube vectors
    public int diameter;
    private CubeFaceVectors top = new CubeFaceVectors();

    private void Start()
    {

        CreateMesh(Vector3.zero, Vector3.zero);
    }

    private void Update()
    {
        if (Input.GetKey("space"))
        {
            CreateMesh(Vector3.zero, Vector3.zero);
        }
    }

    private void CreateMesh(Vector3 cubePosition, Vector3 cubeRotate)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
        vertices = new Vector3[(xRes + 1) * (zRes + 1)];
        triangles = new int[xRes * zRes * 6];
        resolution = xRes * zRes;

        SetVertices();
        CreateTriangles();
        UpdateMesh();
        //FaceToCube(cubePosition, cubeRotate);
    }

    private void SetVertices()
    {
        float xMultiplier = (float)size / xRes;
        float zMultiplier = (float)size / zRes;
        Debug.Log(xMultiplier);
        Debug.Log(zMultiplier);

        for (int index = 0, z = 0; z <= zRes; z++)
        {
            for (int x = 0; x <= xRes; x++)
            {
                Vector3 vertexPosition = new Vector3(x * xMultiplier, 0, z * zMultiplier);
                vertices[index] = vertexPosition;

                index++;
            }
        }
    }

    private void CreateTriangles()
    {
        for (int index = 0, triangleIndex = 0, z = 0; z < zRes; z++)
        {
            for (int x = 0; x < xRes; x++)
            {
                triangles[triangleIndex] = index;
                triangles[triangleIndex + 1] = index + xRes + 1;
                triangles[triangleIndex + 2] = index + 1;
                triangles[triangleIndex + 3] = index + 1;
                triangles[triangleIndex + 4] = index + xRes + 1;
                triangles[triangleIndex + 5] = index + xRes + 2;

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

    private void FaceToCube(Vector3 cubePosition, Vector3 cubeRotate)
    {
        transform.position = cubePosition;
        transform.Rotate(cubeRotate);
    }
}
