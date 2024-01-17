using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerOn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is on floor");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
