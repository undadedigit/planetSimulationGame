/*using System;
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

    public void SetValues(Vector3 localUp, int resolution, float radius, float scale, int layers, float baseAmplitude,
        float baseFrequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed,
        float colourHeight)
    {
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

    public void ConstructTree()
    {
        //reset mesh
        //Array.Clear(vertices, 0, vertices.Length);
        //Array.Clear(triangles, 0, triangles.Length);

        //generate chunks
        Chunk parentChunk = new Chunk(null, null, localUp.normalized, radius, 0, localUp, axisA, axisB);
        parentChunk.GenerateChildren();

        //chunk mesh data
        int triangleOffset = 0;
        foreach (Chunk child in parentChunk.GetVisibleChildren())
        {
            (Vector3[], int[]) verticesAndTriangles = child.CalculateVerticesAndTriangles(triangleOffset);

            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();
            verticesList.AddRange(verticesAndTriangles.Item1);
            trianglesList.AddRange(verticesAndTriangles.Item2);
            vertices = verticesList.ToArray();
            triangles = trianglesList.ToArray();

            triangleOffset += verticesAndTriangles.Item1.Length;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
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
        Vector2 percent = (new Vector2(x, z) / (resolution - 1));
        Vector3 flatVertex = (localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB) * radius;
        Vector3 sphereVertex = flatVertex.normalized;

        float y = GenerateNoise(sphereVertex, scale, layers, baseAmplitude, baseFrequency, ampltiudeMultiplier,
            frequencyMultiplier, offset, seed);

        vertices[i] = sphereVertex * (y * ((noiseMultiplier == 0) ? 1 : noiseMultiplier)) * radius;
        //vertices[i] = flatVertex;

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

    public float GenerateNoise(Vector3 point, float scale, int layers, float amplitude, float frequency,
        float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed)
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
        float[] minMax = { minAltitude, maxAltitude };
        return minMax;
    }

    public Vector3[] GetVertices()
    {
        return mesh.vertices;
    }

    public void SetColours(Color[] colourMap)
    {
        texture = new Texture2D(resolution, resolution);
        texture.SetPixels(colourMap);
        texture.Apply();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}

public class Chunk
{
    public Chunk[] children;
    public Chunk parent;
    public Vector3 position;
    public float radius;
    public int detailLevel;
    public Vector3 localUp;
    public Vector3 axisA;
    public Vector3 axisB;

    public float[] detailLevelDistances = { 1, 10 };

    public Chunk(Chunk[] children, Chunk parent, Vector3 position, float radius, int detailLevel, Vector3 localUp, Vector3 axisA, Vector3 axisB)
    {
        this.children = children;
        this.parent = parent;
        this.position = position;
        this.radius = radius;
        this.detailLevel = detailLevel;
        this.localUp = localUp;
        this.axisA = axisA;
        this.axisB = axisB;
    }

    public void GenerateChildren()
    {
        Transform player = GameObject.Find("Player").transform;

        if (detailLevel <= 1 & detailLevel >= 0)
        {
            if (Vector3.Distance(position.normalized, player.position) <= detailLevelDistances[detailLevel])
            {
                children = new Chunk[4];
                children[0] = new Chunk(new Chunk[0], this, position + axisA * radius / 2 + axisB * radius / 2, radius, detailLevel + 1, localUp, axisA, axisB);
                children[1] = new Chunk(new Chunk[0], this, position + axisA * radius / 2 + axisB * radius / 2, radius, detailLevel + 1, localUp, axisA, axisB);
                children[2] = new Chunk(new Chunk[0], this, position + axisA * radius / 2 + axisB * radius / 2, radius, detailLevel + 1, localUp, axisA, axisB);
                children[3] = new Chunk(new Chunk[0], this, position + axisA * radius / 2 + axisB * radius / 2, radius, detailLevel + 1, localUp, axisA, axisB);

                foreach (Chunk child in children)
                {
                    child.GenerateChildren();
                }

            }
        }

    }

    public Chunk[] GetVisibleChildren()
    {
        List<Chunk> toBeRendered = new List<Chunk>();

        if (children == null || children.Length == 0)
        {
            toBeRendered.Add(this);
        }
        else

        {
            foreach (Chunk child in children)
            {
                toBeRendered.AddRange(GetVisibleChildren());
            }
        }

        return toBeRendered.ToArray();
    }

    public (Vector3[], int[]) CalculateVerticesAndTriangles(int triangleOffset)
    {
        int resolution = 8;
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                Vector3 pointOnCube = position + ((percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB) * radius;
                Vector3 pointOnSphere = pointOnCube.normalized;
                vertices[i] = pointOnSphere;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i + triangleOffset;
                    triangles[triIndex + 1] = i + resolution + 1 + triangleOffset;
                    triangles[triIndex + 2] = i + resolution + triangleOffset;
                    triangles[triIndex + 3] = i + triangleOffset;
                    triangles[triIndex + 4] = i + 1 + triangleOffset;
                    triangles[triIndex + 5] = i + resolution + 1 + triangleOffset;

                    triIndex += 6;
                }
            }
        }

        return (vertices, triangles);
    }
}
*/