using UnityEngine;

public class planet: MonoBehaviour
{

    //planet
    private planetMeshFace[] faceScripts;
    private MeshFilter[] meshFilters;
    public int resolution = 20;
    private Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    //surface
    private GameObject enlargedPlanet;
    private bool enlarged;
    private int minDistance;
    //player
    private Transform player;
    private playerMovement playerScript;
    private Transform skyboxCam;
    private float skyboxScale;
    //stats
    public Vector3 position;
    public float mass;
    //terrain
    private noisePlanetFilter[] noiseFilters;
    //settings
    public planetSettings settings;
    public noiseSettings noise;

    private void Start()
    {
        //surface
        enlargedPlanet = GameObject.Find("EnlargedPlanet");
        minDistance = 150;
        //terrain
        noiseFilters = new noisePlanetFilter[settings.noiseLayers.Length];
        //player
        player = GameObject.Find("Player").transform;
        playerScript = player.gameObject.GetComponent<playerMovement>();
        skyboxCam = GameObject.Find("SkyboxCamera").transform;
        skyboxScale = skyboxCam.parent.GetComponent<scaleControl>().skyboxScale;

        Initialise();
    }

    private void Update()
    {
        Position();
        PlayerDistance();
        
        if (Input.GetKey("a") || true)
        {
            Regenerate();
        }
    }

    private void Initialise()
    {
        faceScripts = new planetMeshFace[6];

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new noisePlanetFilter(settings.noiseLayers[i].noise);
        }

        CreateFaces("PlanetFace", transform, "Skybox");
    }

    private void Position()
    {
        transform.localPosition = position;
        enlargedPlanet.transform.position = position * skyboxScale;
    }

    private void PlayerDistance()
    {
        if (Vector3.Distance(transform.position, skyboxCam.position) < minDistance && !enlarged)
        {
            CreateFaces("SurfaceFace", transform.GetChild(0), "Ground", 0, skyboxScale, true);

            enlarged = true;
        }
    }

    private void CreateFaces(string name, Transform parent, string layer, int localPosition = 1, float skyboxScale = 1, bool colliders = false)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject meshObject = new GameObject(name);
            meshObject.transform.parent = parent;
            meshObject.layer = LayerMask.NameToLayer(layer);
            meshObject.transform.localPosition *= localPosition;

            meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilters[i] = meshObject.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();

            faceScripts[i] = meshObject.AddComponent<planetMeshFace>();
            faceScripts[i].NewFace(meshFilters[i].sharedMesh, resolution, directions[i], settings, noise);
            faceScripts[i].ConstructMesh(noiseFilters, skyboxScale);

            if (colliders)
            {
                MeshCollider collider = meshObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }

        SetColour();
    }

    private void SetColour()
    {
        foreach (MeshFilter filter in meshFilters)
        {
            filter.GetComponent<MeshRenderer>().sharedMaterial.color = settings.colour;
        }
    }

    private void Regenerate()
    {
        for (int i = 0; i < 6; i++)
        {
            faceScripts[i].ConstructMesh(noiseFilters, skyboxScale);
        }

        SetColour();
    }
}