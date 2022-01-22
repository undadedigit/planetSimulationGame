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
    private planetTerrainGenerator terrainGenerator;
    //settings
    public planetSettings settings;

    private void Start()
    {
        //surface
        enlargedPlanet = GameObject.Find("EnlargedPlanet");
        minDistance = 150;
        //terrain
        noiseFilters = new noisePlanetFilter[settings.noiseLayers.Length];
        terrainGenerator = new planetTerrainGenerator(settings);
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
        
        if (Input.GetKey("a"))
        {
            GenerateMesh();
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
        CreateFaces("SurfaceFace", transform.GetChild(0), "Ground", 0, skyboxScale, true);
        EnableSurfaceFaces(false);
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
            EnableSkyboxFaces(false);
            EnableSurfaceFaces(true);

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
            faceScripts[i].NewFace(terrainGenerator, meshFilters[i].sharedMesh, resolution, directions[i], settings);

            if (colliders)
            {
                MeshCollider collider = meshObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            faceScripts[i].ConstructMesh(skyboxScale);
        }

        SetColour();
    }

    private void SetColour()
    {
    }

    private void EnableSkyboxFaces(bool t)
    {
        for (int i = 1; i < 7; i++)
        {
            transform.GetChild(i).gameObject.SetActive(t);
        }
    }

    private void EnableSurfaceFaces(bool t)
    {
        transform.GetChild(0).gameObject.SetActive(t);
    }
}