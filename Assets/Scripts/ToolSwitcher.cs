using UnityEngine;
using UnityEngine.UI; // Required for the Image type
using System.Collections;

public class ToolSwitcher : MonoBehaviour
{
    [Header("Tool Management")]
    public GameObject toolHolder;
    public GameObject secondaryToolHolder;
    public GameObject toolUI;     

    [Header("UI Settings")]
    public Image[] uiImages;      

    [Header("Timing Settings")]
    public float preFadeDelay = 5.0f; 
    private float fadeTime = 0.5f; 


    private int currentToolIndex = 0;
    private int currentUIIndex = 0;

    void Start()
    {
        InitializeTools(toolHolder, true);
        InitializeTools(secondaryToolHolder, true);
        InitializeTools(toolUI, false);
        InitializeUIImages(); 
    }

    private void InitializeTools(GameObject holder, bool isForward)
    {
        if (holder == null) return;

        int index = isForward ? 0 : holder.transform.childCount - 1;
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            holder.transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    private void InitializeUIImages()
    {
        if (uiImages == null) return;

        foreach (Image image in uiImages)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f); // Set full opacity
        }
    }

    public void SwitchToolRight()
    {
        if (toolHolder == null || toolUI == null)
        {
            Debug.LogError("ToolSwitcher: Tool holder or tool UI is not assigned!");
            return;
        }

        SwitchToolInHolder(toolHolder, ref currentToolIndex, true);
        SwitchToolInHolder(secondaryToolHolder, ref currentToolIndex, true);
        SwitchToolInHolder(toolUI, ref currentUIIndex, true);
        SetImageAlpha(uiImages[currentUIIndex], 1f);
        StartCoroutine(FadeOutImage(uiImages[currentUIIndex]));
    }

    public void SwitchToolLeft()
    {
        if (toolHolder == null || toolUI == null)
        {
            Debug.LogError("ToolSwitcher: Tool holder or tool UI is not assigned!");
            return;
        }
        SwitchToolInHolder(toolHolder, ref currentToolIndex, false);
        SwitchToolInHolder(secondaryToolHolder, ref currentToolIndex, false);
        SwitchToolInHolder(toolUI, ref currentUIIndex, false);
        SetImageAlpha(uiImages[currentUIIndex], 1f);
        StartCoroutine(FadeOutImage(uiImages[currentUIIndex]));
    }

    private void SwitchToolInHolder(GameObject holder, ref int index, bool isForward)
    {
        int nextIndex = isForward ? (index + 1) % holder.transform.childCount :
                                    index - 1 < 0 ? holder.transform.childCount - 1 : index - 1;

        for (int i = 0; i < holder.transform.childCount; i++)
        {
            holder.transform.GetChild(i).gameObject.SetActive(false);
        }

        holder.transform.GetChild(nextIndex).gameObject.SetActive(true);
        index = nextIndex;
    }


    private IEnumerator FadeOutImage(Image image)
    {
        yield return new WaitForSeconds(preFadeDelay); // Wait for the specified delay before starting the fade
        float alpha = 1.0f; // Start at full opacity
        float time = 0;

        while (alpha > 0)
        {
            time += Time.deltaTime;
            alpha = Mathf.Lerp(1f, 0f, time / fadeTime);
            SetImageAlpha(image, alpha);
            yield return null;
        }
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
}
