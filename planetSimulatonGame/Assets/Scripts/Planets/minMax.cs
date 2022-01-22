using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minMax
{
    public float min { get; private set; }
    public float max { get; private set; }

    public minMax()
    {
        min = float.MaxValue;
        max = float.MaxValue;
    }

    public void AddValue(float v)
    {
        if (v > max)
        {
            max = v;
        }

        if (v < min)
        {
            min = v;
        }
    }
}
