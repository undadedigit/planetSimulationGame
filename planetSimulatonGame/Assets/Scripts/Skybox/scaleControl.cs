using UnityEngine;

public class scaleControl : MonoBehaviour
{
    //float origin
    public int threshold;
    //player
    private Transform player;
    private CharacterController playerController;
    //skybox
    public float skyboxScale;
    private Transform environment;

    private void Start()
    {
        //player
        player = GameObject.Find("Player").transform;
        //playerController = player.GetComponent<CharacterController>();
        //environment
        environment = GameObject.Find("Environment").transform;
    }

    private void Update()
    {
        FloatingOrigin();
    }

    private void FloatingOrigin()
    {
        if (player.position.magnitude >= threshold)
        {
            environment.position -= player.position / skyboxScale;
            //playerController.enabled = false;
            player.position = Vector3.zero;
            //playerController.enabled = true;
        }
    }
}
