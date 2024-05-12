using UnityEngine;
using System.Collections;

public class Tilling2 : MonoBehaviour
{
    [Header("Material Settings")]
    public Material targetMaterial;
    public float forwardTilingIncrement = 0.1f;
    public float backwardTilingIncrement = -0.1f;
    public float leftTilingIncrement = 0.05f;
    public float rightTilingIncrement = -0.05f;
    public float duration = 3.0f;

    [Header("Audio Settings")]
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    private AudioSource audioSource;

    [Header("Script References")]
    public AdjustMeshPosition adjustMeshPositionScript;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wawa"))
        {
            Vector3 relativePosition = other.transform.position - transform.position;

            if (Vector3.Dot(relativePosition.normalized, transform.forward) > 0.5)
            {
                audioSource.clip = audioClip1;
                audioSource.Play();
                StartCoroutine(AdjustTiling(forwardTilingIncrement));
                adjustMeshPositionScript?.MoveChildren(forwardTilingIncrement);
            }
            else if (Vector3.Dot(relativePosition.normalized, -transform.forward) > 0.5)
            {
                audioSource.clip = audioClip2;
                audioSource.Play();
                StartCoroutine(AdjustTiling(backwardTilingIncrement));
                adjustMeshPositionScript?.MoveChildren(backwardTilingIncrement);
            }
            else if (Vector3.Dot(relativePosition.normalized, -transform.right) > 0.5)
            {
                audioSource.clip = audioClip1;
                audioSource.Play();
                StartCoroutine(AdjustTiling(leftTilingIncrement));
                adjustMeshPositionScript?.MoveChildren(leftTilingIncrement);
            }
            else if (Vector3.Dot(relativePosition.normalized, transform.right) > 0.5)
            {
                audioSource.clip = audioClip2;
                audioSource.Play();
                StartCoroutine(AdjustTiling(rightTilingIncrement));
                adjustMeshPositionScript?.MoveChildren(rightTilingIncrement);
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
