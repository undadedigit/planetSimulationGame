using UnityEngine;

[CreateAssetMenu()]
public class noiseSettings : ScriptableObject
{
    //noise
    public float strength = 1;
    public int layers = 1;
    public float baseRoughnesss = 1;
    public float roughness = 2;
    public float persistance = 0.5f;
    public Vector3 centre;
    public float minValue;
}
