using UnityEngine;

public class noisePlanetFilter
{
    private Noise noise = new Noise();
    private noiseSettings settings;

    public noisePlanetFilter(noiseSettings settings)
    {
        this.settings = settings;
    }

    public float VertexAltitude(Vector3 vertex)
    {
        //float noiseValue = (noise.Evaluate(vertex * settings.roughness + settings.centre) * settings.strength + 1) * 0.5f;
        float noiseValue = 0;
        float frequency = settings.baseRoughnesss;
        float amplitude = 1;
        for (int i = 0; i < settings.layers; i++)
        {
            float v = noise.Evaluate(vertex * frequency + settings.centre) * settings.strength;
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }

        noiseValue = Mathf.Max(1, noiseValue - settings.minValue);
        return noiseValue;
    }
}
