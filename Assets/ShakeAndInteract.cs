using UnityEngine;

public class ShakeAndInteract : MonoBehaviour
{
    [Header("Shake Detection Settings")]
    public float shakeThreshold = 1.0f;
    public float resetTimer = 2.0f;

    [Header("Interactive Elements")]
    public GameObject cork; // Assign in inspector
    public GameObject bottleParent; // Assign in inspector
    public ParticleSystem particleEffect; // Assign in inspector

    private Vector3 lastPosition;
    private float shakeTimer = 0f;
    private bool isShaking = false;

    void Update()
    {
        // Check if the parent has the "Grabbed" tag before checking for shaking
        if (transform.parent != null && transform.parent.CompareTag("Grabbed"))
        {
            float ySpeed = Mathf.Abs((transform.position.y - lastPosition.y) / Time.deltaTime);

            if (ySpeed > shakeThreshold)
            {
                if (!isShaking)
                {
                    isShaking = true;
                    Debug.Log("Shaking!");
                    HandleShakingEffects();
                }
                shakeTimer = resetTimer;
            }
            else
            {
                if (shakeTimer > 0)
                {
                    shakeTimer -= Time.deltaTime;
                }
                else if (isShaking)
                {
                    isShaking = false;
                    Debug.Log("Stopped shaking.");
                }
            }
        }

        lastPosition = transform.position;
    }

    private void HandleShakingEffects()
    {
        // Disable cork GameObject
        if (cork != null)
            cork.SetActive(false);

        // Play particle effect
        if (particleEffect != null)
        {
            ParticleSystem effectInstance = Instantiate(particleEffect, transform.position, Quaternion.identity);
            StartCoroutine(DestroyParticleEffect(effectInstance));
        }
    }

    private System.Collections.IEnumerator DestroyParticleEffect(ParticleSystem effect)
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        Destroy(effect.gameObject); // Destroy the particle effect

        // Also disable the bottleParent GameObject
        if (bottleParent != null)
            bottleParent.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
  
        if (other.gameObject.CompareTag("mainraft") || other.gameObject.CompareTag("raft"))
        { 
            if (!cork || !cork.activeSelf)
            {
                foreach (Renderer renderer in other.GetComponentsInChildren<Renderer>())
                {
                    foreach (Material mat in renderer.materials)
                    {
                        Color newColor = new Color(mat.color.r, mat.color.g, Mathf.Min(mat.color.b + 0.15f, 1.0f));
                        StartCoroutine(LerpColor(mat, mat.color, newColor, 0.5f));
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator LerpColor(Material mat, Color startColor, Color endColor, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            mat.color = Color.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        mat.color = endColor;
    }
}