using UnityEngine;
using System.Collections;

public class TutorialSequence : MonoBehaviour
{
    [Header("Part 1")]
    public Collider leftSafetyCollider;
    public Collider rightSafetyCollider;
    public GameObject WarningUI;
    private bool isLeftColliderTriggered = false;
    private bool isRightColliderTriggered = false;

    [Header("Part 2")]
    public GameObject Bird;
    private GameObject lastTriggeredGameObject = null;
    public GameObject animationControllerObject;
    private AnimationController animControllerScript;

    [Header("Configuration")]
    public float delayInSeconds = 5f;

    void Awake()
    {
        if (animationControllerObject != null)
        {
            animControllerScript = animationControllerObject.GetComponent<AnimationController>();
        }

        if (leftSafetyCollider != null)
        {
            leftSafetyCollider.gameObject.AddComponent<ColliderEventHandler>().OnTriggerEnterEvent += HandleLeftTriggerEnter;
            leftSafetyCollider.gameObject.AddComponent<ColliderEventHandler>().OnTriggerExitEvent += HandleLeftTriggerExit;
        }

        if (rightSafetyCollider != null)
        {
            rightSafetyCollider.gameObject.AddComponent<ColliderEventHandler>().OnTriggerEnterEvent += HandleRightTriggerEnter;
            rightSafetyCollider.gameObject.AddComponent<ColliderEventHandler>().OnTriggerExitEvent += HandleRightTriggerExit;
        }
    }

    private void HandleLeftTriggerEnter(Collider other)
    {
        if (other.CompareTag("raft"))
        {
            isLeftColliderTriggered = true;
            lastTriggeredGameObject = other.gameObject; 
            CheckCollidersAndAct();
            Debug.Log("Left collider triggered by raft.");
        }
    }

    private void HandleRightTriggerEnter(Collider other)
    {
        if (other.CompareTag("raft"))
        {
            isRightColliderTriggered = true;
            lastTriggeredGameObject = other.gameObject; 
            CheckCollidersAndAct();
            Debug.Log("Right collider triggered by raft.");
        }
    }

    private void HandleLeftTriggerExit(Collider other)
    {
        if (other.CompareTag("raft"))
        {
            isLeftColliderTriggered = false; Debug.Log("what");
        }
    }

    private void HandleRightTriggerExit(Collider other)
    {
        if (other.CompareTag("raft"))
        {
            isRightColliderTriggered = false; Debug.Log("what");
        }
    }

    private void CheckCollidersAndAct()
    {
        if (isLeftColliderTriggered && isRightColliderTriggered)
        {
            WarningUI?.SendMessage("Shrink", SendMessageOptions.DontRequireReceiver);
            StartCoroutine(ActivateBirdAfterDelay());
            //isLeftColliderTriggered = false;
            //isRightColliderTriggered = false;
            Debug.LogWarning(lastTriggeredGameObject);

            if (animControllerScript.tutorialFinished)
            {
                Debug.LogWarning("1");
             
                    lastTriggeredGameObject.tag = "mainraft";
                    Debug.Log("Tutorial finished - GameObject tag changed to 'mainraft'.");
   
            }
        }
    }

    IEnumerator ActivateBirdAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        if (Bird != null)
        {
            Bird.SetActive(true);
        }
        else
        {
            Debug.LogError("Bird GameObject is not assigned!");
        }
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        if (leftSafetyCollider != null)
        {
            var handler = leftSafetyCollider.gameObject.GetComponent<ColliderEventHandler>();
            handler.OnTriggerEnterEvent -= HandleLeftTriggerEnter;
            handler.OnTriggerExitEvent -= HandleLeftTriggerExit;
        }

        if (rightSafetyCollider != null)
        {
            var handler = rightSafetyCollider.gameObject.GetComponent<ColliderEventHandler>();
            handler.OnTriggerEnterEvent -= HandleRightTriggerEnter;
            handler.OnTriggerExitEvent -= HandleRightTriggerExit;
        }
    }
}

// Helper class to handle collider events via Unity Events
public class ColliderEventHandler : MonoBehaviour
{
    public event System.Action<Collider> OnTriggerEnterEvent;
    public event System.Action<Collider> OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}
