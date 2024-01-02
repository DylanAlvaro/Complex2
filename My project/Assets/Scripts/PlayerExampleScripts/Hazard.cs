using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{

    public float Damage = 10f;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
            other.GetComponent<Player>().TakeDamage(Damage * Time.deltaTime);
    }
}
