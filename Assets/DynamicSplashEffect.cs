using UnityEngine;
using System.Collections;

public class DynamicSplashEffect : MonoBehaviour
{
    public GameObject particleEffectPrefab;

    void Start()
    {
        // Start the coroutine to delay the execution
        StartCoroutine(SetupEffectMeshAfterDelay(1.0f)); // 1 second delay
    }

    IEnumerator SetupEffectMeshAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Attempt to find the FLOOR_EffectMesh object
        GameObject floorEffectMesh = GameObject.Find("FLOOR_EffectMesh");
        if (floorEffectMesh == null)
        {
            // Optionally create it if not found
            floorEffectMesh = new GameObject("FLOOR_EffectMesh");
            floorEffectMesh.AddComponent<MeshCollider>(); // Add components as needed
        }

        // Attach the SplashEffect script or find it if already attached
        SplashEffect splashEffect = floorEffectMesh.GetComponent<SplashEffect>();
        if (splashEffect == null)
        {
            splashEffect = floorEffectMesh.AddComponent<SplashEffect>();
        }

        // Set the particle effect prefab
        splashEffect.particleEffectPrefab = particleEffectPrefab;
    }
}

public class SplashEffect : MonoBehaviour
{
    public GameObject particleEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("splash"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 spawnPosition = contact.point;
            Quaternion rotation = Quaternion.Euler(-90, 0, 0); // Set the X rotation to -90 degrees

            // Instantiate the particle effect
            GameObject effect = Instantiate(particleEffectPrefab, spawnPosition, rotation);

            // Scale the particle effect
            effect.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Scale set to 0.3 of the original size

            // Play the particle effect
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                particles.Play();
            }

            // Optionally, destroy the particle effect after it finishes
            Destroy(effect, particles.main.duration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("splash"))
        {
            // Use the contact point as the collider's position
            Vector3 spawnPosition = other.ClosestPointOnBounds(transform.position);
            Quaternion rotation = Quaternion.Euler(-90, 0, 0); // Set the X rotation to -90 degrees

            // Instantiate the particle effect
            GameObject effect = Instantiate(particleEffectPrefab, spawnPosition, rotation);

            // Scale the particle effect
            effect.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f); // Scale set to 0.3 of the original size

            // Play the particle effect
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                particles.Play();
            }

            // Optionally, destroy the particle effect after it finishes
            Destroy(effect, particles.main.duration);
        }
    }
}