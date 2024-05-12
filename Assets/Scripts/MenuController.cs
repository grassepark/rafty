using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject MenuUI;
    public GameObject Settings; 
    public GameObject AdditionalUI;

    private bool isMenuActive = false;

    public void ToggleMenu()
    {
        
        isMenuActive = !isMenuActive;
        MenuUI.SetActive(isMenuActive);
        AdditionalUI.SetActive(!isMenuActive);

        if (isMenuActive && Settings.activeSelf)
        {
            Settings.SetActive(false);
        }
    }
}
