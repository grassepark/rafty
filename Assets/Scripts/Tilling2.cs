using UnityEngine;
using System.Collections;

public class Tilling2 : MonoBehaviour
{
    public Material targetMaterial;  // Assign this material in the Unity Inspector
    public float forwardTilingIncrement = 0.1f;  // Increment for forward interaction
    public float backwardTilingIncrement = -0.1f;  // Increment for backward interaction
    public float leftTilingIncrement = 0.05f;  // Increment for left interaction
    public float rightTilingIncrement = -0.05f;  // Increment for right interaction
    public float smoothFactor = 0.1f;  // Smoothing factor for Lerp
    public float duration = 3.0f;  // Duration for the tiling effect

    void OnTriggerEnter(Collider other)
    {
        // Check if the other object has the tag "wawa"
        if (other.CompareTag("wawa"))
        {
            Vector3 relativePosition = other.transform.position - transform.position;

            // Determine the direction of interaction based on relative position
            if (Vector3.Dot(relativePosition.normalized, transform.forward) > 0.5)
            {
                Debug.Log("Forward");
                StartCoroutine(AdjustTiling(forwardTilingIncrement));
            }
            else if (Vector3.Dot(relativePosition.normalized, -transform.forward) > 0.5)
            {
                Debug.Log("Backward");
                StartCoroutine(AdjustTiling(backwardTilingIncrement));
            }
            else if (Vector3.Dot(relativePosition.normalized, -transform.right) > 0.5)
            {
                Debug.Log("Left");
                StartCoroutine(AdjustTiling(leftTilingIncrement));
            }
            else if (Vector3.Dot(relativePosition.normalized, transform.right) > 0.5)
            {
                Debug.Log("Right");
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

            // Smooth transition to new tiling values
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float lerpFactor = elapsedTime / duration;
                float smoothLerpFactor = Mathf.SmoothStep(0.0f, 1.0f, lerpFactor); // Creates a smooth start and finish
                targetMaterial.SetVector("_SurfaceFoamTilingAndOffset", Vector4.Lerp(initialTilingAndOffset, targetTilingAndOffset, smoothLerpFactor));
                yield return null;
            }

            // Optionally reset to initial values after duration
         yield return new WaitForSeconds(duration);
         targetMaterial.SetVector("_SurfaceFoamTilingAndOffset", initialTilingAndOffset);

            //Debug.Log("Tiling adjusted smoothly to: " + targetMaterial.GetVector("_SurfaceFoamTilingAndOffset"));
        }
        else
        {
            //Debug.LogError("Material does not have '_SurfaceFoamTilingAndOffset' property or material is not assigned.");
        }
    }
}
