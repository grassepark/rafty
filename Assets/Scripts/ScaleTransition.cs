using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class ScaleTransition : MonoBehaviour
{
    public GameObject canvas;
    public float scaleA = 0.0006f;
    public float scaleB = 0.005f;
    public float speed = 0.5f;

    void Start()
    {
        StartCoroutine(ScaleObject(canvas, scaleA, scaleB, speed));
    }

    public void ShrinkTransition()
    {
        StartCoroutine(ScaleObject(canvas, scaleB, scaleA, speed, () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }));
    }

    public void Shrink()
    {
        StartCoroutine(ScaleObject(canvas, scaleB, scaleA, speed, () => {
            gameObject.SetActive(false);
        }));
    }

    private IEnumerator ScaleObject(GameObject obj, float startScale, float endScale, float duration, System.Action onComplete = null)
    {
        float currentTime = 0.0f;
        Vector3 initialScale = Vector3.one * startScale;
        Vector3 targetScale = Vector3.one * endScale;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;
            obj.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            yield return null;
        }
        obj.transform.localScale = targetScale;
        onComplete?.Invoke();
    }
}
