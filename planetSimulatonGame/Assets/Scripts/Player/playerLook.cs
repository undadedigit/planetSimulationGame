using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerLook : MonoBehaviour
{
    //look
    private float sensitivity = 100f;
    private float xRotation;
    //player
    private Transform head;
    private Transform cam;

    private void Start()
    {
        //look
        Cursor.lockState = CursorLockMode.Locked;
        //player
        head = transform.GetChild(0);
        cam = head.GetChild(0);
    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
}
