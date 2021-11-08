using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MovementValues
{
    public string input;
    public float initial;
    public float max;
    public float acceleration;
    public float decceleration;

    public MovementValues(string newInput, float newInitial, float newMax, float newAcceleraton, float newDecceleration)
    {
        input = newInput;
        initial = newInitial;
        max = newMax;
        acceleration = newAcceleraton;
        decceleration = newDecceleration;
    }
}

public class playerMovement : MonoBehaviour
{
    //script attached to player object for movement
    //player
    private CharacterController controller;
    //movement
    private float forwardDirection;
    private float rightDirection;
    private float forward;
    private float backward;
    private float right;
    private float left;
    private float up;
    //values
    private MovementValues forwardValues = new MovementValues("e", 10, 30, 20, -5);
    private MovementValues backwardValues = new MovementValues("d", 10, 30, 20, -5);
    private MovementValues rightValues = new MovementValues("f", 10, 30, 20, -5);
    private MovementValues leftValues = new MovementValues("s", 10, 30, 20, -5);
    //jump
    private float jump = 10f;
    private float jumpTimer = 0;
    private float jumpResetTime = 0.2f;
    //gravity
    private float gravity = 20f;
    //ground check
    private Transform feet;
    private groundCheck grounded;

    private void Start()
    {
        //player
        controller = transform.GetComponent<CharacterController>();
        //ground check
        feet = transform.Find("Feet");
        grounded = feet.GetComponent<groundCheck>();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
        Gravity();
        Move();
    }

    private void Movement()
    {
        forwardDirection = Vector3.Dot(controller.velocity, transform.forward);
        rightDirection = Vector3.Dot(controller.velocity, transform.right);

        forward = CalculateMovement(forwardValues.input, forwardDirection, forwardValues.initial, forwardValues.max, forwardValues.acceleration, forwardValues.decceleration);
        backward = CalculateMovement(backwardValues.input, -forwardDirection, backwardValues.initial, backwardValues.max, backwardValues.acceleration, backwardValues.decceleration);
        right = CalculateMovement(rightValues.input, rightDirection, rightValues.initial, rightValues.max, rightValues.acceleration, rightValues.decceleration);
        left = CalculateMovement(leftValues.input, -rightDirection, leftValues.initial, leftValues.max, leftValues.acceleration, leftValues.decceleration);
    }

    private float CalculateMovement(string input, float currentVelocity, float initial, float max, float acceleration, float decceleration)
    {
        if (Input.GetKey(input))
        {
            if (currentVelocity < (initial - 0.2f))
            {
                //key pressed, set to initial
                return initial;
            }
            else if (currentVelocity < max)
            {
                //key held, accelerate to max
                return currentVelocity + acceleration;
            }
            else
            {
                //key held, exceeding max, deccelerate back down
                return currentVelocity + decceleration;
            }
        }
        else
        {
            //key not held, deccelerate to initial
            if (currentVelocity > initial)
            {
                //subtract decceleration to deccelerate back to initial
                return currentVelocity + decceleration;
            }
        }

        return 0;
    }

    private void Jump()
    {
        if (Input.GetKey("space") && grounded.GroundCheck() && jumpTimer <= 0)
        {
            up += jump;
            jumpTimer = jumpResetTime;
        }

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    private void Gravity()
    {
        if (!grounded.GroundCheck())
        {
            up -= gravity * Time.deltaTime;
        }
        else if (up < 0)
        {
            up = 0;
        }
    }

    private void Move()
    {
        Vector3 movement = transform.forward * (forward - backward) + transform.right * (right - left) + transform.up * up;
        controller.Move(movement * Time.deltaTime);
    }
}