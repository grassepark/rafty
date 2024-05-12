using UnityEngine;

public class FishAnimator : MonoBehaviour
{
    private Animator animator;
    private string currentAnimation;
    public ParticleSystem caughtEffect;  
    public GameObject fishModel; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (transform.parent.tag == "grabbed")
        {
            ChangeAnimation("grabbed");
            PlayCaughtEffect();
        }
        else if (transform.parent.tag == "caught")
        {
            //Debug.LogWarning("Disabling Fish Model");
  
            if (fishModel != null)  
            {
                fishModel.SetActive(false);
                Debug.Log(fishModel.name + " has been deactivated because it is caught.");
            }
            else
            {
                Debug.LogError("Fish Model is not assigned in the inspector.");
            }
        }
    }

    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
            //Debug.Log("Animation changed to: " + animation);
        }
    }

    private void PlayCaughtEffect()
    {
        if (caughtEffect != null)
        {
            caughtEffect.Play();
            //Debug.Log("Caught particle effect played.");
        }
        else
        {
            //Debug.LogError("CaughtEffect Particle System is not assigned on " + gameObject.name);
        }
    }
}
