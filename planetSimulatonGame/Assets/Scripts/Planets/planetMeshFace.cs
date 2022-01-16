using UnityEngine;

public class planetMeshFace : MonoBehaviour
{
    //mesh
    private planetTerrainGenerator terrainGenerator;
    private Mesh mesh;
    private int resolution;
    private Vector3 localUp;
    private planetSettings settings;
    private noiseSettings noise;
    private Vector3 axisA;
    private Vector3 axisB;
    //player
    private Transform skyboxCam;
    private float skyboxScale;

    private void Start()
    {
        //player
        skyboxCam = GameObject.Find("SkyboxCamera").transform;
        skyboxScale = skyboxCam.parent.GetComponent<scaleControl>().skyboxScale;
    }

    public void NewFace(planetTerrainGenerator terrainGenerator, Mesh mesh, int resolution, Vector3 localUp, planetSettings settings)
    {
        this.terrainGenerator = terrainGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.settings = settings;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh(float scale)
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triangleIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = ((localUp) + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB) * settings.radius;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = terrainGenerator.CalculatePointOnPlanet(pointOnUnitSphere) * scale;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triangleIndex] = i;
                    triangles[triangleIndex + 1] = i + resolution + 1;
                    triangles[triangleIndex + 2] = i + resolution;
                    triangles[triangleIndex + 3] = i;
                    triangles[triangleIndex + 4] = i + 1;
                    triangles[triangleIndex + 5] = i + resolution + 1;

                    triangleIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
