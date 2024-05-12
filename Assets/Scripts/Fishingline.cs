using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishingline : MonoBehaviour
{
    [Header("Fishing Functionality")]
    public Transform StartPoint;
    public Transform EndPoint;
    public GameObject[] fishPrefabs;
    private GameObject[] fishObjects; 
    private Coroutine deactivateFishCoroutine;

    [Header("Line stuff")]
    public int Segments = 10;
    public LineRenderer lineRenderer;
    public float SegmentLength = 0.02f;
    public float startSegmentLength = 0.01f;
    public float currentTargetLength = 0.00f;
    public float maxSegmentLength = 0.4f;
    public Vector3 Gravity = new Vector3(0, -9.81f, 0);
   
    public int Iterations = 1000;
    // higher is stiffer, lower is stretchier
    public float tensionConstant = 1000f;
    public bool SecondHasRigidbody = true;
    public float LerpSpeed = 1f;

    private bool isChangingLength = false;

    [Header("Fish Count")]
    public int fishCount = 0;
    private bool isFishActive = false;

    [Header("Indicators")]
    public AudioClip fishSound; 
    public Animator fishAnimator;
    private string currentAnimation;


    private class LineParticle
    {
        public Vector3 Pos;
        public Vector3 OldPos;
        public Vector3 Acceleration;
    }

    private List<LineParticle> particles;
        void Start()
    {
        particles = new List<LineParticle>();
        for (int i = 0; i < Segments; i++)
        {
            Vector3 point = Vector3.Lerp(StartPoint.position, EndPoint.position, i / (float)(Segments - 1));
            particles.Add(new LineParticle { Pos = point, OldPos = point, Acceleration = Gravity });
        }
        lineRenderer.positionCount = particles.Count;

        fishObjects = new GameObject[fishPrefabs.Length];
        for (int i = 0; i < fishPrefabs.Length; i++)
        {
            Vector3 objectPosition = Vector3.Lerp(StartPoint.position, EndPoint.position, 0.75f); 
            fishObjects[i] = Instantiate(fishPrefabs[i], EndPoint.position, Quaternion.identity);
            fishObjects[i].SetActive(false);
        }
    }

    void Update()
    {
        if (isChangingLength)
        {
            SegmentLength = Mathf.Lerp(SegmentLength, currentTargetLength, LerpSpeed * Time.deltaTime);
            if (Mathf.Abs(SegmentLength - currentTargetLength) < 0.01f)
            {
                SegmentLength = currentTargetLength;
                isChangingLength = false;
            }
        }

        for (int i = 0; i < fishObjects.Length; i++)
        {
            if (fishObjects[i].activeSelf && HasTaggedChild(fishObjects[i], "caught"))
            {
            

                StartCoroutine(DeactivateFishAfterDelay(0));
                isFishActive = false;
                break;
            }
        }
    }


    public void CastOutlineFromController()
    {
        if (deactivateFishCoroutine != null)
        {
            StopCoroutine(deactivateFishCoroutine);
        }
        StartCoroutine(ActivateFishSequence());
    }


    private IEnumerator ActivateFishSequence()
    {
        foreach (GameObject fish in fishObjects)
        {
            // Wait for the condition where no fish is active
            yield return new WaitUntil(() => !isFishActive);

            float randomDelay = Random.Range(20f, 30f);  // Generate random delay only when no fish is active
            yield return StartCoroutine(ShowFishAfterDelay(fish, randomDelay));
        }
    }


    private IEnumerator IncreaseLengthAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentTargetLength = maxSegmentLength;
        isChangingLength = true;
    }

    public void ReelInLine()
    {

        currentTargetLength = startSegmentLength;
        isChangingLength = true;

        if (deactivateFishCoroutine != null)
        {
            StopCoroutine(deactivateFishCoroutine);
        }


    }

    private IEnumerator ShowFishAfterDelay(GameObject fish, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isFishActive && !HasTaggedChild(fish, "grabbed") && !HasTaggedChild(fish, "caught"))
        {
            fish.SetActive(true);
            isFishActive = true; 
            AudioSource.PlayClipAtPoint(fishSound, fish.transform.position);
            ChangeAnimation("spinning");

            yield return new WaitWhile(() => fish.activeSelf);
            isFishActive = false;
        }
    }



    private IEnumerator DeactivateFishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("DeactivateFishAfterDelay - Before Deactivation - isFishActive: " + isFishActive);


        for (int i = 0; i < fishObjects.Length; i++)
        {
            if (HasTaggedChild(fishObjects[i], "caught"))
            {
                //fishObjects[i].SetActive(false);
          
            }
        }

        ChangeAnimation("BaseState");
        //Debug.Log("DeactivateFishAfterDelay - After Deactivation - isFishActive: " + isFishActive);

    }

    private void DeactivateAllFish()
    {
        foreach (var fishObject in fishObjects)
        {
            fishObject.SetActive(false);
        }
        ChangeAnimation("BaseState");
        isFishActive = false; 
    }

    private bool HasTaggedChild(GameObject obj, string tag)
    {
        if (obj.tag == tag)
        {
            return true;
        }

        foreach (Transform child in obj.transform)
        {
            if (HasTaggedChild(child.gameObject, tag))
            {
                return true;
            }
        }

        return false;
    }


    void FixedUpdate()
    {
        //Debug.Log("FixedUpdate - isFishActive: " + isFishActive);

        foreach (var p in particles)
        {
            Verlet(p, Time.fixedDeltaTime);
        }

        for (int i = 0; i < Iterations; i++)
        {
            for (int j = 0; j < particles.Count - 1; j++)
            {
                PoleConstraint(particles[j], particles[j + 1], SegmentLength);
            }
        }
        particles[0].Pos = StartPoint.position;
        if (SecondHasRigidbody)
        {
            Vector3 force = (particles[particles.Count - 1].Pos - EndPoint.position) * tensionConstant;
            EndPoint.GetComponent<Rigidbody>().AddForce(force);
        }

        particles[particles.Count - 1].Pos = EndPoint.position;

        var positions = new Vector3[particles.Count];
        for (int i = 0; i < particles.Count; i++)
        {
            positions[i] = particles[i].Pos;
        }
        lineRenderer.SetPositions(positions);

        for (int i = 0; i < fishObjects.Length; i++)
        {
            if (!HasTaggedChild(fishObjects[i], "grabbed") && !HasTaggedChild(fishObjects[i], "caught"))
            {
                fishObjects[i].transform.position = EndPoint.position;
            }
        }
    }


    private void Verlet(LineParticle p, float dt)
    {
        var temp = p.Pos;
        p.Pos += p.Pos - p.OldPos + (p.Acceleration * dt * dt);
        p.OldPos = temp;
    }

    // Applies a pole constraint to a pair of particles
    //the distance between each particle to be a specific length
    private void PoleConstraint(LineParticle p1, LineParticle p2, float restLength)
    {
        var delta = p2.Pos - p1.Pos;
        var deltaLength = delta.magnitude;
        var diff = (deltaLength - restLength) / deltaLength;
        p1.Pos += delta * diff * 0.5f;
        p2.Pos -= delta * diff * 0.5f;
    }

        private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            fishAnimator.CrossFade(animation, crossfade);
        }
    }



}