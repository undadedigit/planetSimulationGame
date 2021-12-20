using UnityEngine;

public class planet: MonoBehaviour
{
    //planet
    private planetMeshFace[] faceScripts;
    private MeshFilter[] meshFilters;
    public int resolution = 20;
    public float radius;
    private Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    //surface
    private GameObject enlargedPlanet;
    private bool enlarged;
    private int minDistance;
    public Material surfaceMat;
    //player
    private Transform player;
    private playerMovement playerScript;
    private Transform skyboxCam;
    private float skyboxScale;
    //stats
    public Vector3 position;
    public float mass;


    private void Start()
    {
        //surface
        enlargedPlanet = GameObject.Find("EnlargedPlanet");
        minDistance = 150;
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
    }

    private void Initialise()
    {
        faceScripts = new planetMeshFace[6];

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        CreateSkyboxFaces();
    }

    private void CreateSkyboxFaces()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("PlanetFace");
                meshObject.transform.parent = transform;
                meshObject.layer = LayerMask.NameToLayer("Skybox");

                meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();

                faceScripts[i] = meshObject.AddComponent<planetMeshFace>();
                faceScripts[i].NewFace(meshFilters[i].sharedMesh, resolution, directions[i], radius);
                faceScripts[i].ConstructMesh();
            }

        }
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
            CreateSurfaceFaces(enlargedPlanet);

            enlarged = true;
        }
    }

    private void CreateSurfaceFaces(GameObject enlargedPlanet)
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject meshObject = new GameObject("EnlargedPlanetFace");
            meshObject.transform.parent = enlargedPlanet.transform;
            meshObject.layer = LayerMask.NameToLayer("Ground");
            meshObject.transform.localPosition = Vector3.zero;

            meshObject.AddComponent<MeshRenderer>().sharedMaterial = surfaceMat;
            meshFilters[i] = meshObject.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();


            faceScripts[i] = meshObject.AddComponent<planetMeshFace>();
            faceScripts[i].NewFace(meshFilters[i].sharedMesh, resolution, directions[i], radius);
            faceScripts[i].ConstructMesh(skyboxScale);

            MeshCollider collider = meshObject.AddComponent<MeshCollider>();
            collider.convex = true;
        }
    }
}
