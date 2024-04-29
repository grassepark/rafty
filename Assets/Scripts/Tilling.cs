using UnityEngine;

public class Tilling : MonoBehaviour
{
    public Material targetMaterial;  // Assign this material in the Unity Inspector
    public float tilingIncrement = 0.1f;  // Increment to adjust tiling each frame
    public float smoothFactor = 0.1f;  // Smoothing factor for Lerp

    private void OnTriggerStay(Collider other)
    {
        // Check if the object in the trigger has the tag "Wawa"
        if (other.CompareTag("wawa"))
        {
            // Smoothly adjust the tiling using Lerp
            AdjustTiling(tilingIncrement);
        }
    }

    private void AdjustTiling(float tilingIncrement)
    {
        if (targetMaterial != null && targetMaterial.HasProperty("_SurfaceFoamTilingAndOffset"))
        {
            Vector4 currentTilingAndOffset = targetMaterial.GetVector("_SurfaceFoamTilingAndOffset");
            Vector4 targetTilingAndOffset = new Vector4(
                currentTilingAndOffset.x + tilingIncrement,
                currentTilingAndOffset.y + tilingIncrement,
                currentTilingAndOffset.z,
                currentTilingAndOffset.w);

            // Use Lerp to smoothly transition to the new tiling values
            targetMaterial.SetVector("_SurfaceFoamTilingAndOffset", Vector4.Lerp(currentTilingAndOffset, targetTilingAndOffset, smoothFactor));
            Debug.Log("Tiling adjusted smoothly to: " + targetMaterial.GetVector("_SurfaceFoamTilingAndOffset"));
        }
        else
        {
            Debug.LogError("Material does not have '_SurfaceFoamTilingAndOffset' property or material is not assigned.");
        }
    }
}
