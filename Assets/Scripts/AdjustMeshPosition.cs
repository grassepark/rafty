using UnityEngine;
using System.Collections;

public class AdjustMeshPosition : MonoBehaviour
{
    public Material targetMaterial; // Assign this in the inspector
    private float moveDuration = 15f; // Duration over which to move to the targetY
    private float fadeDuration = 15f; // Duration over which to fade the material

    void Start()
    {
        // Immediately set the alpha to 0 at start if the material is assigned and supports the property
        if (targetMaterial != null)
        {
            if (targetMaterial.HasProperty("_Alpha"))
            {
                targetMaterial.SetFloat("_Alpha", 0);
            }
            else
            {
                Debug.LogError("Material does not have an '_Alpha' property.");
            }

            StartCoroutine(FadeInMaterial(5f, 0.602f, fadeDuration));
        }
        else
        {
            Debug.LogError("Target material not assigned in the inspector.");
        }
    }

    IEnumerator FadeInMaterial(float delayBeforeStart, float targetAlpha, float duration)
    {
        yield return new WaitForSeconds(delayBeforeStart);

        GameObject floorEffectMesh = GameObject.Find("FLOOR_EffectMesh");
        GameObject wawaLevel = GameObject.Find("wawalevel");

        if (floorEffectMesh != null)
        {
            floorEffectMesh.tag = "wawa";
        }
        else
        {
            Debug.LogError("FLOOR_EffectMesh not found in the scene.");
            yield break; // Exit the coroutine if the object is not found
        }

        if (wawaLevel == null)
        {
            Debug.LogError("Wawa level game object not found in the scene.");
            yield break; // Exit the coroutine if the object is not found
        }

        StartCoroutine(MoveToTargetY(5f, wawaLevel.transform.position.y, moveDuration, floorEffectMesh));

        float currentTime = 0;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0, targetAlpha, currentTime / duration);
            SetAlpha(targetMaterial, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(targetMaterial, targetAlpha);
    }

    IEnumerator MoveToTargetY(float delayBeforeStart, float targetYPosition, float duration, GameObject floorEffectMesh)
    {
        yield return new WaitForSeconds(delayBeforeStart);
        float elapsedTime = 0;
        float startY = floorEffectMesh.transform.position.y;
        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startY, targetYPosition, elapsedTime / duration);
            floorEffectMesh.transform.position = new Vector3(floorEffectMesh.transform.position.x, newY, floorEffectMesh.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        floorEffectMesh.transform.position = new Vector3(floorEffectMesh.transform.position.x, targetYPosition, floorEffectMesh.transform.position.z);
    }

    public void SetAlpha(Material material, float alpha)
    {
        if (material.HasProperty("_Alpha"))
        {
            material.SetFloat("_Alpha", alpha);
        }
        else
        {
            Debug.LogError("Material does not have an '_Alpha' property.");
        }
    }
}
