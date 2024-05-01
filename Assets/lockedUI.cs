using UnityEngine;

public class lockedUI : MonoBehaviour
{
    public Transform sourceTransform; 

    void Update()
    {
        if (sourceTransform != null)
        {
            transform.position = new Vector3(sourceTransform.position.x, sourceTransform.position.y, sourceTransform.position.z + 1);
            
            
        }
    }
}
