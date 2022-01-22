using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class colourGenerator
{
    private planetSettings settings;
    private Texture2D texture;
    private const int textureRes = 50;

    public colourGenerator(planetSettings settings)
    {
        this.settings = settings;
        texture = new Texture2D(textureRes, 1);
    }

    /*
    public void UpdateElevation(minMax elevationMinMax)
    {
        settings.planetMat.SetVector("_elevationMinMax", new Vector4(elevationMinMax.min, elevationMinMax.max));
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[textureRes];
        for (int i = 0; i < textureRes; i++)
        {
            colours[i] = settings.gradient.Evaluate(i / (textureRes - 1f));
        }

        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMat.SetTexture("_texture", texture);
    }
    */
}
