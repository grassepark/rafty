using UnityEngine;
using System.Collections;

public class DebugTilling : MonoBehaviour
{
    public Material targetMaterial;
    public float forwardTilingIncrement = 0.1f;
    public float backwardTilingIncrement = -0.1f;
    public float leftTilingIncrement = 0.05f;
    public float rightTilingIncrement = -0.05f;
    public float smoothFactor = 0.1f; // Note: This variable is still defined but not used.
    public float duration = 3.0f;
    public AudioClip audioClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wawa"))
        {
            audioSource.Play();
            Vector3 relativePosition = other.transform.position - transform.position;

            // Debug Rays for visualization
            Debug.DrawRay(transform.position, transform.forward * 5, Color.green, 2.0f);
            Debug.DrawRay(transform.position, -transform.forward * 5, Color.red, 2.0f);
            Debug.DrawRay(transform.position, transform.right * 5, Color.blue, 2.0f);
            Debug.DrawRay(transform.position, -transform.right * 5, Color.yellow, 2.0f);
            Debug.DrawRay(transform.position, relativePosition, Color.magenta, 2.0f);

            if (Vector3.Dot(relativePosition.normalized, transform.forward) > 0.5)
            {
                StartCoroutine(AdjustTiling(forwardTilingIncrement));
            }
            else if (Vector3.Dot(relativePosition.normalized, -transform.forward) > 0.5)
            {
                StartCoroutine(AdjustTiling(backwardTilingIncrement));
            }
            else if (Vector3.Dot(relativePosition.normalized, -transform.right) > 0.5)
            {
                StartCoroutine(AdjustTiling(leftTilingIncrement));
            }
            else if (Vector3.Dot(relativePosition.normalized, transform.right) > 0.5)
            {
                StartCoroutine(AdjustTiling(rightTilingIncrement));
            }
        }
    }

    IEnumerator AdjustTiling(float tilingIncrement)
    {
        if (targetMaterial != null && targetMaterial.HasProperty("_SurfaceFoamTilingAndOffset"))
        {
            Vector4 currentTilingAndOffset = targetMaterial.GetVector("_SurfaceFoamTilingAndOffset");
            Vector4 initialTilingAndOffset = new Vector4(
                currentTilingAndOffset.x,
                currentTilingAndOffset.y,
                currentTilingAndOffset.z,
                currentTilingAndOffset.w);

            Vector4 targetTilingAndOffset = new Vector4(
                currentTilingAndOffset.x + tilingIncrement,
                currentTilingAndOffset.y + tilingIncrement,
                currentTilingAndOffset.z,
                currentTilingAndOffset.w);

            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float lerpFactor = elapsedTime / duration;
                float smoothLerpFactor = Mathf.SmoothStep(0.0f, 1.0f, lerpFactor);
                targetMaterial.SetVector("_SurfaceFoamTilingAndOffset", Vector4.Lerp(initialTilingAndOffset, targetTilingAndOffset, smoothLerpFactor));
                yield return null;
            }
        }
    }
}
