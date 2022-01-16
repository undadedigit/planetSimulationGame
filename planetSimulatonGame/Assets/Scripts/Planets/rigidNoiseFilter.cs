using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rigidNoiseFilter : iNoiseFilter
{
    private Noise noise = new Noise();
    private noiseSettings settings;

    public rigidNoiseFilter(noiseSettings settings)
    {
        this.settings = settings;
    }

    public float VertexAltitude(Vector3 vertex)
    {
        //float noiseValue = (noise.Evaluate(vertex * settings.roughness + settings.centre) * settings.strength + 1) * 0.5f;
        float noiseValue = 0;
        float frequency = settings.baseRoughnesss;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < settings.layers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(vertex * frequency + settings.centre) * settings.strength);
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);
            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistance;
        }

        noiseValue = Mathf.Max(1, noiseValue - settings.minValue);
        return noiseValue;
    }
}
