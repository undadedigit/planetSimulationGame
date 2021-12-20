using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundCheck : MonoBehaviour
{
    private bool grounded;
    private float checkSphereRadius = 0.2f;
    private LayerMask groundLayer;
    
    private void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    public bool GroundCheck()
    {
        grounded = Physics.CheckSphere(transform.position, checkSphereRadius, groundLayer);
        return grounded;
    }
}
