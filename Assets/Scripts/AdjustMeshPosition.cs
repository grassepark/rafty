using UnityEngine;
using System.Collections;

public class AdjustMeshPosition : MonoBehaviour
{
    public Material targetMaterial; // Assign this in the inspector
    private GameObject floorEffectMesh; // This will be found dynamically
    private GameObject wawaLevel;
    private float moveDuration = 15f; // Duration over which to move to the targetY
    private float fadeDuration = 15f; // Duration over which to fade the material

    void Start()
    {
  

        if (targetMaterial != null)
        {
            StartCoroutine(FadeInMaterial(5f, 0.802f, fadeDuration)); 
        }
        else
        {
            Debug.LogError("Target material not assigned in the inspector.");
        }
    }

    public void SetAlpha(Material material, float alpha)
    {
        if (material.HasProperty("_Alpha")) // Check if the property exists to avoid errors
        {
            material.SetFloat("_Alpha", alpha);
        }
        else
        {
            Debug.LogError("Material does not have an '_Alpha' property.");
        }
    }


        IEnumerator FadeInMaterial(float delayBeforeStart, float targetAlpha, float duration)
    {
        yield return new WaitForSeconds(delayBeforeStart);
        floorEffectMesh = GameObject.Find("FLOOR_EffectMesh");
        wawaLevel = GameObject.Find("wawalevel");

        if (floorEffectMesh != null && wawaLevel != null)
        {
            float targetY = wawaLevel.transform.position.y; 
            StartCoroutine(MoveToTargetY(5f, targetY, moveDuration));
        }
        else
        {
            if (floorEffectMesh == null)
                Debug.LogError("FLOOR_EffectMesh not found in the scene.");
            if (wawaLevel == null)
                Debug.LogError("wawalevel not found in the scene.");
        }

        float currentTime = 0;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0, targetAlpha, currentTime / duration);
            SetAlpha(targetMaterial, alpha);  // Use the helper function
            Debug.Log($"Setting material alpha to {alpha}");

            currentTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(targetMaterial, targetAlpha);
        Debug.Log("Final alpha set");
    }

    IEnumerator MoveToTargetY(float delayBeforeStart, float targetYPosition, float duration)
    {
        Debug.Log("water rising");
        yield return new WaitForSeconds(delayBeforeStart);

        float elapsedTime = 0;
        float startY = floorEffectMesh.transform.position.y;
        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startY, targetYPosition, (elapsedTime / duration));
            Vector3 newPosition = new Vector3(floorEffectMesh.transform.position.x, newY, floorEffectMesh.transform.position.z);
            floorEffectMesh.transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        floorEffectMesh.transform.position = new Vector3(floorEffectMesh.transform.position.x, targetYPosition, floorEffectMesh.transform.position.z);
    }
}