using UnityEngine;

public class BuoyancyObject : MonoBehaviour
{
    public float amplitude = 0.5f; // Height of the oscillation
    public float frequency = 1f; // Speed of the oscillation

    private Vector3 startPosition;
    private float phase = 0.0f;

    void Start()
    {
        // Save the original position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new y position using a sine wave
        float newY = startPosition.y + amplitude * Mathf.Sin(phase);
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Increment the phase by frequency times the deltaTime
        phase += frequency * Time.deltaTime;

        // Reset the phase to avoid precision issues over time
        if (phase > 2 * Mathf.PI)
        {
            phase -= 2 * Mathf.PI;
        }
    }
}
