using UnityEngine;

[CreateAssetMenu()]
public class planetSettings : ScriptableObject
{
    //shape
    public float radius;
    //colour
    public Color colour;
    public Material planetMat;
    //noise
    public noiseLayer[] noiseLayers;
    [System.Serializable]
    public class noiseLayer
    {
        public bool enabled = true;
        public bool firstLayerMask;
        public noiseSettings noise;
    }
}
