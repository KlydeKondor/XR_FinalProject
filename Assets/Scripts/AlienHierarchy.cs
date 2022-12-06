using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienHierarchy : MonoBehaviour
{
    public GameObject alien, moonrock, moonrock1, moonrock2;

    void OnCollisionEnter(Collision other) {
        if (other.gameObject == moonrock || other.gameObject == moonrock1 || other.gameObject == moonrock2) {
            other.transform.parent = alien.transform;
        }        
    } 
}
