using UnityEngine;

public class FishController : MonoBehaviour
{
    public GameObject mainraft;
    public GameObject bucketPrefab; 
    public GameObject fish; 

    private int lastActivatedFishIndex = -1; 

    void Start()
    {
      
        if (bucketPrefab != null)
            bucketPrefab.SetActive(false);

       
        for (int i = 0; i < fish.transform.childCount; i++)
        {
            fish.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update()
    {
       
        if (mainraft != null && mainraft.activeInHierarchy && mainraft.tag == "mainraft")
        {
            if (bucketPrefab != null)
                bucketPrefab.SetActive(true); 

            int caughtCount = GameObject.FindGameObjectsWithTag("caught").Length;

          
            int fishIndexToActivate = caughtCount - 1; 
            if (fishIndexToActivate >= 0 && fishIndexToActivate < fish.transform.childCount &&
                fishIndexToActivate > lastActivatedFishIndex)
            {
                fish.transform.GetChild(fishIndexToActivate).gameObject.SetActive(true);
                lastActivatedFishIndex = fishIndexToActivate;
            }
        }
        else
        {
            if (bucketPrefab != null)
                bucketPrefab.SetActive(false); 
        }
    }
}
