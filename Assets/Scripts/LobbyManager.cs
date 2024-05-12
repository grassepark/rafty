using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private static readonly string SCENE_PERMISSION = "com.oculus.permission.USE_SCENE";
    [SerializeField] private GameObject PermissionParent;
    [SerializeField] private GameObject NoSceneModelParent;
    [SerializeField] public OVRSceneManager ovrSceneManagerPrefab;
    private OVRSceneManager ovrSceneManager;


    void Start()
    {
        if (ovrSceneManagerPrefab != null)
        {
            ovrSceneManager = Instantiate(ovrSceneManagerPrefab);
        }
        else
        {
            Debug.LogError("OVRSceneManager prefab is not assigned in the inspector");
        }
    }

    public void Request()
    {
        RequestScenePermission();
        Debug.Log("pressed");
    }

    public void RequestScenePermission()
    {
        if (Permission.HasUserAuthorizedPermission(SCENE_PERMISSION))
        {
            ActivateGame();
        }
        else
        {
            PermissionParent.SetActive(true);
            PermissionCallbacks callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += (permissionName) => { ActivateGame(); };
 
            Permission.RequestUserPermission(SCENE_PERMISSION, callbacks);
        }
    }

    private void ActivateGame()
    {
        Debug.Log("Permission granted - Activating game.");
        //SwitchToNextScene();
    }

    public void OnNoSceneModelAvailable()
    {
        Debug.Log("No scene model available - Activating rescan option.");
        NoSceneModelParent.SetActive(true);
        Rescan();
 
    }

    public void Rescan()
    {
        Debug.Log("rescan");
        ovrSceneManager.RequestSceneCapture();
    }

    public void SwitchToNextScene()
    {

        Debug.Log("switching scene");
        // Get the current active scene's build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the next scene index
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        // Load the next scene
        SceneManager.LoadScene(nextSceneIndex);
    }
}
