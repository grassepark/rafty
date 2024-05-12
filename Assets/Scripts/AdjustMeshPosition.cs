using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdjustMeshPosition : MonoBehaviour
{
    [Header("Material and Movement")]
    public Material targetMaterial;
    private float moveDuration = 15f;
    private float fadeDuration = 15f;

    [Header("Child Prefabs and Settings")]
    public GameObject[] childPrefabs;
    public Vector3 childScale = new Vector3(0.5f, 0.5f, 0.5f);
    public int objectCount = 10;
    public float spawnDelay = 0.5f;

    [Header("Wave Motion Parameters")]
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public float moveDuration2 = 1.0f;

    private bool blockMovement = false;

    void Start()
    {
        if (targetMaterial != null)
        {
            if (targetMaterial.HasProperty("_Alpha"))
            {
                targetMaterial.SetFloat("_Alpha", 0);
            }
            else
            {
                Debug.LogError("Material does not have an '_Alpha' property.");
            }
            StartCoroutine(FadeInMaterial(2f, 0.602f, fadeDuration));
        }
        else
        {
            Debug.LogError("Target material not assigned in the inspector.");
        }
    }

    IEnumerator FadeInMaterial(float delayBeforeStart, float targetAlpha, float duration)
    {
        yield return new WaitForSeconds(delayBeforeStart);

        GameObject floorEffectMesh = GameObject.Find("FLOOR_EffectMesh");
        if (floorEffectMesh != null)
        {
            floorEffectMesh.tag = "wawa";
        }
        else
        {
            Debug.LogError("FLOOR_EffectMesh not found in the scene.");
            yield break;
        }

        GameObject[] wawaLevels = GameObject.FindGameObjectsWithTag("wawalevel");
        GameObject lowestWawaLevel = null;
        float lowestYPosition = float.MaxValue;

        foreach (GameObject wawaLevel in wawaLevels)
        {
            if (wawaLevel.transform.position.y < lowestYPosition)
            {
                lowestYPosition = wawaLevel.transform.position.y;
                lowestWawaLevel = wawaLevel;
            }
        }

        if (lowestWawaLevel != null)
        {
            Renderer renderer = floorEffectMesh.GetComponent<Renderer>();
            StartCoroutine(FadeIn(renderer, targetAlpha, duration));
            StartCoroutine(MoveToTargetY(floorEffectMesh, lowestWawaLevel.transform.position.y, moveDuration));

            yield return new WaitForSeconds(duration);

        if (childPrefabs.Length > 0)
            {
                Vector3 meshBounds = floorEffectMesh.GetComponent<Renderer>().bounds.size;
                for (int i = 0; i < objectCount; i++)
                {
                    Vector3 randomPosition = new Vector3(
                        Random.Range(-meshBounds.x / 2, meshBounds.x / 2),
                        0,
                        Random.Range(-meshBounds.z / 2, meshBounds.z / 2)
                    );
                    Quaternion randomRotation = Quaternion.Euler(
                        Random.Range(-90, 90),
                        Random.Range(0, 180),
                        0
                    );
                    GameObject child = Instantiate(childPrefabs[Random.Range(0, childPrefabs.Length)], floorEffectMesh.transform.position + randomPosition, randomRotation, floorEffectMesh.transform);
                    child.transform.localScale = childScale;
                    child.name = "SpawnedChild" + i;
                    child.AddComponent<MimicWaterMotion>().SetMotion(amplitude, frequency);

                    yield return new WaitForSeconds(spawnDelay);
                }
            }
            else
            {
                Debug.LogError("Child prefab array is empty or not assigned in the inspector.");
            }
        }
        else
        {
            if (lowestWawaLevel == null)
                Debug.LogError("Wawa level game object not found in the scene.");
        }
    }

    IEnumerator FadeIn(Renderer renderer, float targetAlpha, float duration)
    {
        float currentTime = 0;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0, targetAlpha, currentTime / duration);
            block.SetFloat("_Alpha", alpha);
            renderer.SetPropertyBlock(block);
            currentTime += Time.deltaTime;
            yield return null;
        }
        block.SetFloat("_Alpha", targetAlpha);
        renderer.SetPropertyBlock(block);
    }

    IEnumerator MoveToTargetY(GameObject floorEffectMesh, float targetYPosition, float duration)
    {
        float elapsedTime = 0;
        float startY = floorEffectMesh.transform.position.y;
        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startY, targetYPosition, elapsedTime / duration);
            floorEffectMesh.transform.position = new Vector3(floorEffectMesh.transform.position.x, newY, floorEffectMesh.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        floorEffectMesh.transform.position = new Vector3(floorEffectMesh.transform.position.x, targetYPosition, floorEffectMesh.transform.position.z);
    }

    public void MoveChildren(float moveIncrement)
    {
        if (blockMovement)
        {
            Debug.Log("Movement is blocked due to collision with raft.");
            return;
        }

        GameObject floorEffectMesh = GameObject.Find("FLOOR_EffectMesh");
        if (floorEffectMesh != null)
        {
            StartCoroutine(MoveChildrenCoroutine(floorEffectMesh.transform, moveIncrement));
        }
        else
        {
            Debug.LogError("FLOOR_EffectMesh not found when trying to move children.");
        }
    }

    private IEnumerator MoveChildrenCoroutine(Transform parentTransform, float moveIncrement)
    {
        if (parentTransform == null)
        {
            Debug.LogError("Parent Transform is null.");
            yield break; // Exit if no parent transform is provided
        }

        float duration = 1f;
        float elapsed = 0f;
        List<List<Vector3>> initialPositions = new List<List<Vector3>>();

        foreach (Transform child in parentTransform)
        {
            List<Vector3> childPositions = new List<Vector3>();
            foreach (Transform grandChild in child)
            {
                childPositions.Add(grandChild.position);
            }
            initialPositions.Add(childPositions);
        }

        while (elapsed < duration)
        {
            if (blockMovement) yield break; // Stop movement if blocked

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            int i = 0;
            foreach (Transform child in parentTransform)
            {
                int j = 0;
                foreach (Transform grandChild in child)
                {
                    Vector3 startPosition = initialPositions[i][j];
                    Vector3 targetPosition = new Vector3(startPosition.x + moveIncrement, startPosition.y, startPosition.z + moveIncrement);
                    grandChild.position = Vector3.Lerp(startPosition, targetPosition, t);
                    j++;
                }
                i++;
            }

            yield return null;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("raft") || collision.gameObject.CompareTag("mainraft"))
        {
            blockMovement = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("raft") || collision.gameObject.CompareTag("mainraft"))
        {
            blockMovement = false;
        }
    }
}

public class MimicWaterMotion : MonoBehaviour
    {
        public float amplitude = 0.5f;
        public float frequency = 1f;
        private Vector3 velocity;
        private float damping = 0.9f;

        private Vector3 startPosition;
        private float phase;
        private bool canMove = true; // Control flag for enabling or disabling movement

        void Start()
        {
            startPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (canMove) // Check if movement is allowed
            {
                startPosition += velocity * Time.fixedDeltaTime;
                velocity *= damping; // Apply damping

                phase += Time.fixedDeltaTime * frequency;
                float deltaX = 0.2f * amplitude * Mathf.Sin(phase);
                float deltaY = 0.1f * amplitude * Mathf.Cos(phase);

                transform.position = startPosition + new Vector3(deltaX, 0, deltaY);
            }
        }

        public void SetMotion(float amp, float freq)
        {
            amplitude = amp;
            frequency = freq;
        }

        public void UpdateStartPosition(Vector3 increment)
        {
            velocity += increment;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if the collision object has the tag 'raft' or 'mainraft'
            if (collision.gameObject.tag == "raft" || collision.gameObject.tag == "mainraft")
            {
                canMove = false; // Disable movement
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // Check if the collision object has the tag 'raft' or 'mainraft'
            if (collision.gameObject.tag == "raft" || collision.gameObject.tag == "mainraft")
            {
                canMove = true; // Enable movement
            }
        }
    }
