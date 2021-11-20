using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planet : MonoBehaviour
{
    private planetFace[] planetFaces;
    private MeshFilter[] meshFilters;
    private int resolution = 10;
    private float radius;


    private void Start()
    {
        Initialise();
    }

    private void Update()
    {
        GenerateMesh();
    }

    private void Initialise()
    {
        planetFaces = new planetFace[6];

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        radius = 5;

        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back};

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("PlanetFace");
                meshObject.layer = LayerMask.NameToLayer("Skybox");
                meshObject.transform.parent = transform;
                meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            planetFaces[i] = new planetFace(meshFilters[i].sharedMesh, resolution, directions[i], radius);
        }
    }

    private void GenerateMesh()
    {
        foreach (planetFace face in planetFaces)
        {
            face.ConstructMesh();
        }
    }
}
