using UnityEngine;

public class DeactivatorPlane : MonoBehaviour
{
    // This method is called when another GameObject with a Collider enters the trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Deactivates the GameObject that collided with this plane.
        other.gameObject.SetActive(false);
    }
}
