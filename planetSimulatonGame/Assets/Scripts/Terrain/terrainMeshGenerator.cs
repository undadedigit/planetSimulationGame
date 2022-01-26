using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class terrainMeshGenerator : MonoBehaviour
{
    //mesh
    private Mesh mesh;
    private int resolution;
    private float radius;
    private MeshRenderer meshRenderer;
    private Vector3 localUp;
    private Vector3 axisA;
    private Vector3 axisB;
    //chunks
    private int divisions;
    private int xChunk;
    private int zChunk;
    //vertices
    private Vector3[] vertices;
    private float maxAltitude;
    private float minAltitude;
    //triangles
    private int[] triangles;
    private int tris;
    //noise
    private float scale;
    private int layers;
    private float baseAmplitude;
    private float baseFrequency;
    private float ampltiudeMultiplier;
    private float frequencyMultiplier;
    private Vector2 offset;
    private int seed;
    private float noiseMultiplier = 1;
    private Noise noise;
    //colours
    private Color[] colourMap;
    private Texture2D texture;
    private Vector2[] uvs;
    private float colourHeight;
    //collisions
    private MeshCollider meshCollider;

    public void Initialise()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        noise = new Noise();

        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    public void SetValues(int divisions, int xChunk, int zChunk, Vector3 localUp, int resolution, float radius, float scale, int layers, float baseAmplitude, float baseFrequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed, float colourHeight)
    {
        this.divisions = divisions;
        this.xChunk = xChunk;
        this.zChunk = zChunk;
        this.localUp = localUp;
        this.resolution = resolution;
        this.radius = radius;
        this.scale = scale;
        this.layers = layers;
        this.baseAmplitude = baseAmplitude;
        this.baseFrequency = baseFrequency;
        this.ampltiudeMultiplier = amplitudeMultiplier;
        this.frequencyMultiplier = frequencyMultiplier;
        this.offset = offset;
        this.seed = seed;
        this.colourHeight = colourHeight;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void GenerateMesh()
    {
        vertices = new Vector3[resolution * resolution];

        triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        tris = 0;

        colourMap = new Color[resolution * resolution];

        uvs = new Vector2[resolution * resolution];

        for (int x = 0, i = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                SetVertices(i, x, z);
                SetTriangles(i, x, z);
                SetUVs(i, x, z);
                SetColours(i, x, z);

                i++;
            }
        }

        texture = new Texture2D(resolution, resolution);
        texture.SetPixels(colourMap);
        texture.Apply();

        UpdateMesh(vertices, triangles, uvs);
    }

    private void SetVertices(int i, int x, int z)
    {
        Vector2 percent = new Vector2((x + resolution * xChunk) / divisions, (z + resolution * zChunk) / divisions) / (resolution - 1);
        Vector3 flatVertex = (localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB) * radius;
        Vector3 sphereVertex = flatVertex.normalized;

        float y = GenerateNoise(sphereVertex, scale, layers, baseAmplitude, baseFrequency, ampltiudeMultiplier, frequencyMultiplier, offset, seed);

        vertices[i] = flatVertex;
        vertices[i] = sphereVertex * (y * ((noiseMultiplier == 0) ? 1 : noiseMultiplier)) * radius;

        float altitude = Vector3.Distance(Vector3.zero, vertices[i]);
        if (altitude > maxAltitude)
        {
            maxAltitude = altitude;
        }
        else if (altitude < minAltitude)
        {
            minAltitude = altitude;
        }
    }

    private void SetTriangles(int i, int x, int z)
    {
        if (x < resolution - 1 && z < resolution - 1)
        {
            triangles[tris] = i;
            triangles[tris + 1] = i + resolution;
            triangles[tris + 2] = i + resolution + 1;
            triangles[tris + 3] = i;
            triangles[tris + 4] = i + resolution + 1;
            triangles[tris + 5] = i + 1;

            tris += 6;
        }
    }

    private void SetUVs(int i, int x, int z)
    {
        uvs[i] = new Vector2(x / (float)resolution, z / (float)resolution);
    }

    private void SetColours(int i, int x, int z)
    {
        float strength = Mathf.PerlinNoise(x / 10, z / 10);
        colourMap[i] = new Color(strength, strength, strength);
    }

    public float GenerateNoise(Vector3 point, float scale, int layers, float amplitude, float frequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed)
    {
        System.Random rng = new System.Random(seed);

        float noiseValue = 0;

        for (int i = 0; i < layers; i++)
        {
            float layerNoise = noise.Evaluate(point * frequency);
            noiseValue += (layerNoise + 1) * 0.5f * amplitude;
            amplitude *= amplitudeMultiplier;
            frequency *= frequencyMultiplier;
        }

        return noiseValue;
    }

    private void UpdateMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        meshRenderer.sharedMaterial.mainTexture = texture;

        meshCollider.sharedMesh = mesh;
    }

    public float[] MinMaxValues()
    {
        float[] minMax = {minAltitude, maxAltitude};
        return minMax;
    }

    public Vector3[] GetVertices()
    {
        return mesh.vertices;
    }
}
