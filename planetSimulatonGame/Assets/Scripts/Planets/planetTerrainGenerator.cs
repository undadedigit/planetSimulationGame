using UnityEngine;

public class planetTerrainGenerator
{
    private planetSettings settings;
    private iNoiseFilter[] noiseFilters;

    public planetTerrainGenerator(planetSettings settings)
    {
        this.settings = settings;
        noiseFilters = new iNoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = noiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noise);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].VertexAltitude(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].firstLayerMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].VertexAltitude(pointOnUnitSphere) * mask;
            }
        }
        return pointOnUnitSphere * settings.radius * (1 + elevation);
    }
}
