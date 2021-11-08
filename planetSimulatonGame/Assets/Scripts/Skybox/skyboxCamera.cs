using UnityEngine;

public class skyboxCamera : MonoBehaviour
{
    //player
    private Transform player;
    private Transform playerCam;
    //scale
    private scaleControl scaleScript;
    private float skyboxScale;

    private void Start()
    {
        //player
        player = GameObject.Find("Player").transform;
        playerCam = player.Find("Head").Find("PlayerCam").transform;
        //scale
        scaleScript = transform.GetComponentInParent<scaleControl>();
        skyboxScale = scaleScript.skyboxScale;
    }

    private void Update()
    {
        SkyboxCamera();
    }

    private void SkyboxCamera()
    {
        //rotation
        transform.rotation = playerCam.rotation;

        //movement
        transform.localPosition = playerCam.position / skyboxScale;
    }
}
