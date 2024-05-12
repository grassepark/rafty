using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DynamicScaleTransition : MonoBehaviour
{
    public float scaleA = 0.005f;
    public float scaleB = 1.0f;
    public float speed = 2.0f;

    void Start()
    {
        // Check if the current scene is "Check"
        if (SceneManager.GetActiveScene().name == "Check")
        {
            // Start the scale transition
            StartCoroutine(ScaleObject(this.gameObject, scaleA, scaleB, speed));
        }
    }

 

    private IEnumerator ScaleObject(GameObject obj, float startScale, float endScale, float duration)
    {
        float currentTime = 0.0f;
        Vector3 initialScale = Vector3.one * startScale;
        Vector3 targetScale = Vector3.one * endScale;

        while (currentTime <= duration)
        {
            // Calculate current time of the transition
            currentTime += Time.deltaTime;
            float t = currentTime / duration;
            // Use SmoothStep for ease in, ease out effect
            t = Mathf.SmoothStep(0.0f, 1.0f, t);
            // Apply the smoothed transition
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }
        obj.transform.localScale = targetScale;
    }
}
