using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 3f;

    private float vRot = 0f;
    private float hRot = 0f;
    private Rigidbody rb;
  
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    /// <summary>
    /// Player movement
    /// </summary>
    void Update()
    {
        // Mouse rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        hRot += mouseX;
        vRot -= mouseY;
        vRot = Mathf.Clamp(vRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(0f, hRot, 0f);
        Camera.main.transform.localRotation = Quaternion.Euler(vRot, 0f, 0f);

        // Player movement
        float moveForward = Input.GetAxis("Vertical") * movementSpeed;
        float moveLeft = Input.GetAxis("Horizontal") * movementSpeed;

        Vector3 movement = (transform.forward * moveForward) + (transform.right * moveLeft);
        movement.y = 0f;

        rb.velocity = movement;
    }
}
