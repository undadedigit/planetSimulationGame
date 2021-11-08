using UnityEngine;

public class playerFly : MonoBehaviour
{
    //player
    private CharacterController controller;
    private Transform playerCam;
    //movement
    private float forward;
    private float right;
    private float up;
    //speed
    public float speed = 20f;
    private int speedMultiplier = 2000;

    private void Start()
    {
        controller = transform.GetComponent<CharacterController>();
        playerCam = transform.Find("Head").Find("PlayerCam");
    }

    private void Update()
    {
        Movement();
        Move();
        Speed();
    }

    private void Movement()
    {
        forward = CalculateMovement("e", "d");
        right = CalculateMovement("f", "s");
        up = CalculateMovement("space", "left shift");
    }

    private float CalculateMovement(string input, string antiInput)
    {
        if (Input.GetKey(input))
        {
            return speed;
        }

        if (Input.GetKey(antiInput))
        {
            return -speed;
        }

        return 0;
    }

    private void Move()
    {
        Vector3 movement = playerCam.forward * forward + playerCam.right * right + playerCam.up * up;
        controller.Move(movement * Time.deltaTime);
    }

    private void Speed()
    {
        speed += Input.mouseScrollDelta.y * speedMultiplier * Time.deltaTime;
    }
}
