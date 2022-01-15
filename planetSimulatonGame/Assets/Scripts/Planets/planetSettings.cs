using UnityEngine;

[CreateAssetMenu()]
public class planetSettings : ScriptableObject
{
    //shape
    public float radius;
    //colour
    public Color colour;

    //noise
    public noiseLayer[] noiseLayers;
    [System.Serializable]
    public class noiseLayer
    {
        public bool enabled = true;
        public noiseSettings noise;
    }
}
