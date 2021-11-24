using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetMeshFace : MonoBehaviour
{
    //mesh
    public Mesh mesh;
    private MeshFilter surfaceMesh;
    //player
    private Transform skyboxCam;
    private float skyboxScale;
    private float minDistance;

    private Vector3[] enlargedVertices;
    private int enlargedVertexCount;

    private void Start()
    {
        //player
        skyboxCam = GameObject.Find("SkyboxCamera").transform;
        skyboxScale = skyboxCam.parent.GetComponent<scaleControl>().skyboxScale;
        minDistance = 1f;

        enlargedVertices = new Vector3[3];
    }

    private void Update()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            if (Vector3.Distance(mesh.vertices[i], skyboxCam.position) <= minDistance && enlargedVertexCount < 3)
            {
                enlargedVertices[enlargedVertexCount] = mesh.vertices[i];
                enlargedVertexCount++;
            }
        }

        if (enlargedVertexCount == 3)
        {
            Debug.Log("a");
            GameObject enlargedObject = new GameObject("Surface");
            Mesh enlargedMesh = new Mesh();
            enlargedObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            surfaceMesh = enlargedObject.AddComponent<MeshFilter>();
            surfaceMesh.sharedMesh = new Mesh();
            surfaceMesh.sharedMesh.vertices = enlargedVertices;
            surfaceMesh.sharedMesh.triangles = new int[]{0, 1, 2};
            surfaceMesh.sharedMesh.RecalculateNormals();
            /*
            */
            enlargedVertexCount = 4;
        }
    }
}
