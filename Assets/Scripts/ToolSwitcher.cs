using UnityEngine;
using System.Collections;

public class ToolSwitcher : MonoBehaviour
{
    public GameObject toolHolder; // Assign the parent GameObject for tools
    public GameObject toolUI;     // Assign the parent GameObject for tool UI elements
    public float fadeDuration = 5.0f; // Time in seconds to fade out

    private int currentToolIndex = 0;
    private int currentUIIndex = 0;

    void Start()
    {
        InitializeTools(toolHolder, true);
        InitializeTools(toolUI, false);
    }

    private void InitializeTools(GameObject holder, bool isForward)
    {
        if (holder == null) return;

        int index = isForward ? 0 : holder.transform.childCount - 1;
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            holder.transform.GetChild(i).gameObject.SetActive(i == index);
            if (holder == toolUI) // Reset alpha for toolUI at the start
                SetMaterialAlpha(holder.transform.GetChild(i).gameObject, 1f);
        }
    }

    public void SwitchTool()
    {
        if (toolHolder == null || toolUI == null)
        {
            Debug.LogError("ToolSwitcher: Tool holder or tool UI is not assigned!");
            return;
        }

        // Switch tools in the toolHolder
        SwitchToolInHolder(toolHolder, ref currentToolIndex, true);
        // Switch UI in the toolUI
        SwitchToolInHolder(toolUI, ref currentUIIndex, false);

        // Reset the alpha of the new tool UI to 100
        SetMaterialAlpha(toolUI.transform.GetChild(currentUIIndex).gameObject, 1f);
        // Start fading the current tool UI after delay
        StartCoroutine(FadeOutMaterial(toolUI.transform.GetChild(currentUIIndex).gameObject));
    }

    private void SwitchToolInHolder(GameObject holder, ref int index, bool isForward)
    {
        int nextIndex = isForward ? (index + 1) % holder.transform.childCount :
                                    index - 1 < 0 ? holder.transform.childCount - 1 : index - 1;

        // Deactivate all children
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            holder.transform.GetChild(i).gameObject.SetActive(false);
        }

        // Activate the next tool
        holder.transform.GetChild(nextIndex).gameObject.SetActive(true);
        index = nextIndex;
    }

    private IEnumerator FadeOutMaterial(GameObject tool)
    {
        Debug.Log("Fading!!");
        yield return new WaitForSeconds(fadeDuration); // Wait before starting the fade
        float alpha = 1.0f; // Start at full opacity
        float time = 0;

        while (alpha > 0)
        {
            time += Time.deltaTime;
            alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            SetMaterialAlpha(tool, alpha);
            yield return null;
        }
    }

    private void SetMaterialAlpha(GameObject obj, float alpha)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }
}
