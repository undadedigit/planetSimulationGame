using System.Collections.Generic;
using UnityEngine;

public class gravitationalAttractor : MonoBehaviour
{
    public const float g = 0.67408f;
    public Rigidbody rb;
    public static List<gravitationalAttractor> attractors;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        foreach (gravitationalAttractor attractor in attractors)
        {
            if (attractor != this)
            {
                Attract(attractor);
            }
        }
    }

    private void OnEnable()
    {
        if (attractors == null)
        {
            attractors = new List<gravitationalAttractor>();
        }

        attractors.Add(this);
    }

    private void OnDisable()
    {
        attractors.Remove(this);
    }

    private void Attract(gravitationalAttractor attractObject)
    {
        Rigidbody attractRb = attractObject.rb;

        Vector3 direction = rb.position - attractRb.position;
        float distance = direction.magnitude;

        if (distance == 0)
        {
            return;
        }

        float magnitude = g * (rb.mass * attractRb.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * magnitude;
        
        attractRb.AddForce(force);
    }
}
