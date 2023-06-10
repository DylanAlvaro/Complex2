using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    // variables
    private float camSpeed = 360;
    public float distance = 5;
    public Transform target;
    public float currentDistance;
    public float heightOffset = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        currentDistance = distance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            Vector3 angles = transform.eulerAngles;
            float dx = Input.GetAxis("Mouse Y");
            float dy = Input.GetAxis("Mouse X");

            angles.x = Mathf.Clamp(angles.x + dx * camSpeed * Time.deltaTime, 0, 70);
            angles.y += dy * camSpeed * Time.deltaTime;
            transform.eulerAngles = angles;
        }
    }
}
