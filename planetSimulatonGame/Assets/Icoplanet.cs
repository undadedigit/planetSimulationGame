using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Icoplanet : MonoBehaviour
{
    //planet
    private Icosphere planet;
    public int divisions;
    public float radius;
    //surface
    private int surfaceVertexCount;
    private int[] surfaceVertices;
    private GameObject surface;
    private Mesh surfaceMesh;
    //player
    private Transform skyboxCam;

    private void Start()
    {
        //planet
        CreateIcosphere();
        //surface
        surfaceVertexCount = 0;
        surfaceVertices = new int[3];
        //player
        skyboxCam = GameObject.Find("SkyboxCamera").transform;
    }

    private void Update()
    {
        Size();
        Surface();
    }

    private void CreateIcosphere()
    {
        planet = new Icosphere();
        planet.InitAsIcosohedron();
        planet.Subdivide(divisions);
        planet.GenerateMesh(gameObject);
        planet.UpdateMesh();
    }

    private void Size()
    {
        transform.localScale = new Vector3(radius, radius, radius);
    }

    private void Surface()
    {
        planet.CheckDistance(skyboxCam);
        /*
        if (surfaceVertexCount < 3)
        {
            int i = planet.CheckDistance(skyboxCam);
            if (i != -1 && !surfaceVertices.Contains(i))
            {
                surfaceVertices[surfaceVertexCount] = i;
                surfaceVertexCount++;
            }
        }
        else if(surfaceVertexCount == 3)
        {
            CreateSurface();
            surfaceVertexCount = 4;
        }
        */
    }

    private void CreateSurface()
    {
        surface = new GameObject();
        MeshRenderer surfaceRenderer = surface.AddComponent<MeshRenderer>();
        surfaceRenderer.material = new Material(Shader.Find("Standard"));
        MeshFilter surfaceFilter = surface.AddComponent<MeshFilter>();

        surfaceMesh = new Mesh();
        //surfaceMesh.vertices = planet.GetSurfaceVertices(surfaceVertices, surfaceVertexCount);
        //surfaceMesh.triangles = new[] {0, 1, 2};
        surfaceFilter.mesh = surfaceMesh;
    }
}
