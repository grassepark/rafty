using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Check if the event is being triggered by printing the name of the colliding object
        Debug.Log("Triggered by: " + other.gameObject.name);

        // Check if the GameObject colliding has the tag "Oar"
        if (other.gameObject.tag == "Oar")
        {
            Debug.Log("AAAAA");
        }
        else
        {
            Debug.Log("Collided with non-Oar object");
        }
    }
}