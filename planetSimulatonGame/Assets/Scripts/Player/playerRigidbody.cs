using UnityEngine;

public class playerRigidbody : MonoBehaviour
{
    //player
    private Rigidbody rb;
    //jump
    private float jumpVelocity = 50;

    private float jumpDelay;
    //ground check
    private groundCheck grounded;
    private Transform feet;

    private void Start()
    {
        //player
        rb = GetComponent<Rigidbody>();
        //ground check
        feet = transform.Find("Feet");
        grounded = feet.GetComponent<groundCheck>();
    }

    private void FixedUpdate()
    {
        KeepGrounded();
        Jump();
    }

    private void KeepGrounded()
    {
        if (grounded.GroundCheck() && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    private void Jump()
    {
        if (jumpDelay <= 0)
        {
            if (grounded.GroundCheck() && Input.GetKey("space"))
            {
                rb.velocity += new Vector3(0, jumpVelocity, 0) * Time.deltaTime;
                Debug.Log("a");
                jumpDelay = 0.5f;
            }
        }
        else
        {
            jumpDelay -= Time.deltaTime;
        }
    }
}
