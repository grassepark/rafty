using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    [Header("Target Game Objects")]
    public GameObject gameObjectA;
    public GameObject gameObjectB;
    public GameObject gameObjectC;

    [Header("Additional Game Objects")]
    public GameObject toolUI;
    public GameObject music;
    public GameObject dialogueBox;

    [Header("Animation Control")]
    public Animator animator;
    private string currentAnimation;

    [Header("Movement Settings")]
    public float speed = 5.0f;
    private bool hasReachedB = false;
    private int animationState = 0;

    [Header("Tutorial Status")]
    public bool tutorialFinished = false;

    [Header("Audio Clips")]
    public AudioClip call1;
    public AudioClip call2;
    public AudioClip call3;
    public AudioClip call4;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Attach an AudioSource component if not already attached
        transform.position = gameObjectA.transform.position;
        ChangeAnimation("Idle", 0.2f);
        Debug.LogWarning("Starting Tutorial");
    }

    private void Update()
    {
        if (!hasReachedB)
        {
            MoveToPointB();
        }
    }

    private void MoveToPointB()
    {
        transform.position = Vector3.MoveTowards(transform.position, gameObjectB.transform.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, gameObjectB.transform.position) < 0.1f)
        {
            hasReachedB = true;
            ChangeAnimation("Talking", 0.2f);
            PlayAudioClip(0); 
        }
        else
        {
            ChangeAnimation("Diving", 0.2f);
        }
    }

    public void OnButtonPress()
    {
        if (!hasReachedB) return;

        switch (animationState)
        {
            case 0:
                PlayAudioClip(1);
                StartCoroutine(PlayTalkingThenIdle(animationState));
                break;
            case 1:
                PlayAudioClip(2);
                StartCoroutine(PlayTalkingThenIdle(animationState));
                break;
            case 2:
                PlayAudioClip(3);
                StartCoroutine(PlayTalkingThenIdle(animationState));
                break;
            case 3:
                PlayAudioClip(0);
                StartCoroutine(PlayTalkingThenIdle(animationState));
                break;
            case 4:
                ChangeAnimation("Exiting", 0.2f);
                PlayAudioClip(1);
                StartCoroutine(MoveToPointC());
                break;
            case 5:
                toolUI.SetActive(true);
                music.SetActive(true);
                dialogueBox.GetComponent<ScaleTransition>().Shrink();
                tutorialFinished = true;
                break;
        }

        animationState = (animationState + 1) % 6;
    }

    private IEnumerator PlayTalkingThenIdle(int stateIndex)
    {
        ChangeAnimation("Talking", 0.2f);
        yield return new WaitForSeconds(3f);
        ChangeAnimation("Idle", 0.2f);
    }

    private void PlayAudioClip(int index)
    {
        AudioClip clip = index switch
        {
            0 => call1,
            1 => call2,
            2 => call3,
            3 => call4,
            _ => null
        };
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator MoveToPointC()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0;
        float duration = 2f;
        Vector3 startPosition = transform.position;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, gameObjectC.transform.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = gameObjectC.transform.position;
    }

    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
            Debug.Log("Animation changed to: " + animation);
        }
    }
}
