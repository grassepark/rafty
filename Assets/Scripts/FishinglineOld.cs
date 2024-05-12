using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishinglineOld : MonoBehaviour
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
    private float fishAppearTime = 0;
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

         // Instantiate fish objects at the position of the EndPoint
        fishObjects = new GameObject[fishPrefabs.Length];
        for (int i = 0; i < fishPrefabs.Length; i++)
        {
            Vector3 objectPosition = Vector3.Lerp(StartPoint.position, EndPoint.position, 0.75f); // Adjust the ratio as needed
            fishObjects[i] = Instantiate(fishPrefabs[i], EndPoint.position, Quaternion.identity);
            fishObjects[i].SetActive(false); 
            fishObjects[i].transform.parent = transform;
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
    }

    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            fishAnimator.CrossFade(animation, crossfade);
        }
    }

    public void CastOutlineFromController()
    {
        float randomDelay = Random.Range(0.5f, 20f); // Generate a random delay
        StartCoroutine(IncreaseLengthAfterDelay(randomDelay));
        StartCoroutine(ShowFishAfterDelay(randomDelay + 3.0f)); 

        if (deactivateFishCoroutine != null)
        {
            StopCoroutine(deactivateFishCoroutine);
        }

        deactivateFishCoroutine = StartCoroutine(DeactivateFishAfterDelay(randomDelay + 7.0f));
    }

    public void ReelInLine()//reelin the fishing line
    {

        currentTargetLength = startSegmentLength;
        isChangingLength = true;

        if (deactivateFishCoroutine != null)
        {
            StopCoroutine(deactivateFishCoroutine);
        }

        if (Time.time - fishAppearTime <= 5f)
        {
       
            fishCount++;
        }


    }

    private IEnumerator ShowFishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);


        if (!isFishActive)
        {
            int randomIndex = Random.Range(0, fishObjects.Length);
            fishObjects[randomIndex].SetActive(true);
            isFishActive = true;

            AudioSource.PlayClipAtPoint(fishSound, transform.position);
            ChangeAnimation("spinning");

            fishAppearTime = Time.time;
        }
    }


    private IEnumerator IncreaseLengthAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentTargetLength = maxSegmentLength;
        isChangingLength = true;
    }

    //call nono fish
    private IEnumerator DeactivateFishAfterDelay(float delay) 
    {
        yield return new WaitForSeconds(delay);

        // Deactivate all fish objects
        DeactivateAllFish();
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

    // Update the line with Verlet Physics
    void FixedUpdate()
    {
    
        for (int i = 0; i < fishObjects.Length; i++)
        {
            fishObjects[i].transform.position = EndPoint.position;
        }

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
    }

    // Performs Verlet integration to update the position of a particle
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





}