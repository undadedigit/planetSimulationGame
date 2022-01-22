using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;

public class planetGenerator : MonoBehaviour
{
    //faces
    private GameObject[] faces;
    private terrainMeshGenerator[] generators;
    private Vector3[] directions = {Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back};
    private string[] directionNames = {"Top", "Bottom", "Right", "Left", "Front", "Behind"};
    //mesh
    public int resolution = 40;
    public float radius = 10f;
    public float scale = 50f;
    public int layers = 5;
    public float baseAmplitude = 1;
    public float baseFrequency = 0.1f;
    public float amplitudeMultiplier = 0.25f;
    public float frequencyMultiplier = 10f;
    public Vector2 offset;
    public int seed;
    public float colourHeight = 0.75f;
    //generation
    public bool regenerate;
    public bool regenerateOnce;
    //vertices
    private float minAltitude;
    private float maxAltitude;

    private void Start()
    {
        faces = new GameObject[6];
        generators = new terrainMeshGenerator[6];

        for (int i = 0; i < 6; i++)
        {
            faces[i] = new GameObject("Face" + directionNames[i]);
            faces[i].transform.parent = transform.transform;
            faces[i].layer = LayerMask.NameToLayer("Ground");

            generators[i] = faces[i].AddComponent<terrainMeshGenerator>();
            generators[i].Initialise();
        }

        Generate();
    }

    private void Update()
    {
        if (Input.GetKey("space") || regenerate || regenerateOnce)
        {
            Generate();
            regenerateOnce = false;
        }
    }

    private void Generate()
    {
        for (int i = 0; i < 6; i++)
        {
            generators[i].SetValues(directions[i], resolution, radius, scale, layers, baseAmplitude, baseFrequency, amplitudeMultiplier, frequencyMultiplier, offset, seed, colourHeight);
            generators[i].GenerateMesh();

            minAltitude = 0;
            maxAltitude = 0;
            MinMax(i);

            AltitudeColours(i);
        }
    }

    private void MinMax(int i)
    {
        if (generators[i].MinMaxValues()[0] < minAltitude)
        {
            minAltitude = generators[i].MinMaxValues()[0];
        }
        else if (generators[i].MinMaxValues()[1] > maxAltitude)
        {
            maxAltitude = generators[i].MinMaxValues()[1];
        }
    }

    private void AltitudeColours(int i)
    {
        Color[] colourMap = new Color[resolution * resolution];

        for (int x = 0, j = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float altitude = Vector3.Distance(Vector3.zero, generators[i].GetVertices()[j]);

                float strength = altitude / maxAltitude;
                strength = Mathf.Lerp(0, 1, strength * strength);
                colourMap[j] = new Color(strength, 0, 1 - strength);

                j++;
            }
        }

        generators[i].SetColours(colourMap);
    }
}
