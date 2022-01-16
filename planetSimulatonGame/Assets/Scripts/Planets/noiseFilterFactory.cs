using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class noiseFilterFactory
{
    public static iNoiseFilter CreateNoiseFilter(noiseSettings settings)
    {
        switch (settings.filterType)
        {
            case noiseSettings.FilterType.Simple:
                return new noisePlanetFilter(settings);
            case noiseSettings.FilterType.Rigid:
                return new rigidNoiseFilter(settings);
        }

        return null;
    }
}
