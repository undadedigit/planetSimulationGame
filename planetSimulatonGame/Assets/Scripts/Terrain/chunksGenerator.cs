using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunksGenerator : MonoBehaviour
{
    //chunks
    private GameObject[] chunkFaces;
    private terrainMeshGenerator[] generators;
    public int divisions = 3;
    private int chunks;
    public int xStart;
    public int zStart;
    private int xEnd;
    private int zEnd;
    //mesh
    private Vector3 position;
    private Vector3 localUp;
    public int resolution = 40;
    public float radius = 10f;
    public float scale = 50f;
    public int layers = 5;
    public float baseAmplitude = 1;
    public float baseFrequency = 0.1f;
    public float amplitudeMultiplier = 0.25f;
    public float frequencyMultiplier = 10f;
    public Vector2 offset;
    public int seed;
    public float colourHeight = 0.75f;
    
    public void SetValues(int xStart, int zStart, int xEnd, int zEnd, Vector3 position, int divisions, Vector3 localUp, int resolution, float radius, float scale, int layers, float baseAmplitude, float baseFrequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed, float colourHeight, int chunks = -1)
    {
        this.xStart = xStart;
        this.zStart = zStart;
        this.xEnd = xEnd;
        this.zEnd = zEnd;
        this.divisions = divisions;
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
    }

    public void GenerateChunks()
    {
        chunks = divisions * divisions;
        chunkFaces = new GameObject[chunks];
        generators = new terrainMeshGenerator[chunks];

        for (int x = xStart, i = 0; x < xEnd; x++)
        {
            for (int z = zStart; z < zEnd; z++)
            {
                chunkFaces[i] = new GameObject("Chunk" + i);
                chunkFaces[i].transform.parent = transform;
                chunkFaces[i].layer = LayerMask.NameToLayer("Ground");
                generators[i] = chunkFaces[i].AddComponent<terrainMeshGenerator>();
                generators[i].Initialise();

                GenerateMeshes(i, x, z);

                i++;
            }
        }
    }

    private void GenerateMeshes(int i, int x, int z)
    {
        generators[i].SetValues(transform.position, divisions, x, z, localUp, resolution, radius, scale, layers, baseAmplitude, baseFrequency, amplitudeMultiplier, frequencyMultiplier, offset, seed, colourHeight);
        generators[i].GenerateMesh();
    }
}
