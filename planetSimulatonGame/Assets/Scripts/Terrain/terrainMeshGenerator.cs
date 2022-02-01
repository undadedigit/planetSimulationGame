using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class terrainMeshGenerator : MonoBehaviour
{
    //mesh
    private Mesh mesh;
    public int resolution;
    private float radius;
    private MeshRenderer meshRenderer;
    private Vector3 position;
    private Vector3 localUp;
    private Vector3 axisA;
    private Vector3 axisB;
    //chunks
    public int divisions;
    private int xChunk;
    private int zChunk;
    private chunksGenerator generator;
    public bool chunk;
    //vertices
    private Vector3[] vertices;
    private float maxAltitude;
    private float minAltitude;
    public Vector3 midpoint;
    //triangles
    private int[] triangles;
    private int tris;
    //noise
    private float scale;
    private int layers;
    private float baseAmplitude;
    private float baseFrequency;
    private float amplitudeMultiplier;
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
    //player
    private Transform player;

    public void Initialise()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        noise = new Noise();

        meshCollider = gameObject.AddComponent<MeshCollider>();

        player = GameObject.Find("Player").transform;
    }

    public void SetValues(Vector3 position, int divisions, int xChunk, int zChunk, Vector3 localUp, int resolution, float radius, float scale, int layers, float baseAmplitude, float baseFrequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed, float colourHeight)
    {
        this.position = position;
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
        this.amplitudeMultiplier = amplitudeMultiplier;
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
        float chunkMultiplier = (float) (resolution - 1) / resolution;
        float xPercent = (x + resolution * (chunkMultiplier * xChunk)) / divisions;
        float zPercent = (z + resolution * (chunkMultiplier * zChunk)) / divisions;

        Vector2 percent = new Vector2(xPercent, zPercent) / (resolution - 1);
        Vector3 flatVertex = (localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB) * radius;
        Vector3 sphereVertex = flatVertex.normalized;

        float y = GenerateNoise(sphereVertex, layers, baseAmplitude, baseFrequency, amplitudeMultiplier, frequencyMultiplier, offset, seed);
        //y = 1;

        vertices[i] = flatVertex;
        vertices[i] = sphereVertex * (y * ((noiseMultiplier == 0) ? 1 : noiseMultiplier)) * radius + position;
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

    public float GenerateNoise(Vector3 point, int layers, float amplitude, float frequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed)
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

    private void Update()
    {
        float playerDistance = Vector3.Distance(player.position, midpoint);
        
        if (playerDistance < 2000 && divisions < 4 && !chunk)
        {
            Chunk();
            chunk = true;
        }
    }

    public void Chunk()
    {
        int addition = 2 - (int)Mathf.Log(divisions, 2);
        int x = xChunk * 2;//((int)Mathf.Log(divisions, 2) + addition);
        int z = zChunk * 2;//((int)Mathf.Log(divisions, 2) + addition);

        mesh.Clear();
        generator = gameObject.AddComponent<chunksGenerator>();
        generator.SetValues(x, z, x + 2, z + 2, transform.position, divisions * 2, localUp, resolution, radius, scale, layers, baseAmplitude, baseFrequency, amplitudeMultiplier, frequencyMultiplier, offset, seed, colourHeight);
        generator.GenerateChunks();
    }
}
