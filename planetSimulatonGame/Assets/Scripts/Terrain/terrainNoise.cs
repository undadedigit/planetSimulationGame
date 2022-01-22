using UnityEngine;

public class terrainNoise 
{
    public float Generate(int x, int z, float scale, int layers, float amplitude, float frequency, float amplitudeMultiplier, float frequencyMultiplier, Vector2 offset, int seed)
    {
        System.Random rng = new System.Random(seed);

        Vector2[] layerOffsets = new Vector2[layers];
        for (int i = 0; i < layers; i++)
        {
            float xOffset = rng.Next(-10000, 1000) + offset.x;
            float zOffset = rng.Next(-10000, 1000) + offset.y ;
            layerOffsets[i] = new Vector2(xOffset, zOffset);
        }

        float noiseValue = 0;

        for (int i = 0; i < layers; i++)
        {
            float xSample = x / scale * frequency + layerOffsets[i].x;
            float zSample = z / scale * frequency + layerOffsets[i].y;

            noiseValue += Mathf.PerlinNoise(xSample, zSample) * amplitude;

            amplitude *= amplitudeMultiplier;
            frequency *= frequencyMultiplier;
        }

        return noiseValue;
    }
}
