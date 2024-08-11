using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector;

public class BossManager : MonoBehaviour
{
    public GameObject[] CubePrefabs;
    public GameObject Player;
    public GameObject Boss;

    public Transform[] holdPoints;

    private GameObject originalObject;
    private GameObject[] currentObjects;
    private int instantiatedCount = 0;
    public float attractionForce;
    public float damping;
    public float horizontalForce;
    public float rotationForce;

    public float throwForce;
    public float destroyTime;
    public float velocity;

    public float instantiateInterval;
    public float throwInterval;
    public float delayBetweenSequences;

    public float throwCoolDown;
    private bool playerInTrigger = false;
    private bool sequenceStarted = false;

    private vRagdoll bossRagdollScript;

    private vHealthController bossHealth;

    private int state = 0;

    public Transform[] spawnpoints;
    public GameObject[] enemiesPacks;

    private bool firstBatchSpawned = false;
    private bool secondBatchSpawned = false;
    private bool lastBatchSpawned = false;
    private bool canStartSequence = true;

    void Awake()
    {
        currentObjects = new GameObject[holdPoints.Length];
        bossRagdollScript = Boss.GetComponentInParent<vRagdoll>();
        state = 0;
        bossHealth = Boss.GetComponentInParent<vHealthController>();
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        MoveAllObjectsToHoldPoints();
        if (Player != null)
        {
            if (Player.GetComponentInParent<vHealthController>().isDead)
            {
                clearObjects();
                StopCoroutine(TriggerSequenceWhenPlayerInside());
                canStartSequence = false;
                Player = null;
            }
        }

        else if(bossHealth.currentHealth <= 150 && !secondBatchSpawned)
        {
            instantiateEnemies();
            secondBatchSpawned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player == null && other.CompareTag("Player"))
        {
            Player = other.gameObject.transform.Find("3D Model").transform.Find("HealTarget").gameObject;
            Debug.Log(Player);
            canStartSequence = true;
        }
        if (Player != null && other.CompareTag("Player"))
        {
            playerInTrigger = true;
            StartCoroutine(TriggerSequenceWhenPlayerInside());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            StopCoroutine(TriggerSequenceWhenPlayerInside());
            currentObjects = new GameObject[holdPoints.Length];
        }
    }

    public void StartCombinedSequence()
    {
        Debug.Log("SequenceStarted");
        StartCoroutine(CombinedSequence());
    }

    private IEnumerator CombinedSequence()
    {
        StartInstantiateSequence();
        yield return new WaitForSeconds(delayBetweenSequences);
        StartThrowingSequence();
    }

    void resetInstantiateCount()
    {
        instantiatedCount = 0;
    }

    void InstantiateObject()
    {
        if (CubePrefabs.Length > 0 && instantiatedCount < holdPoints.Length)
        {
            originalObject = CubePrefabs[Random.Range(0, CubePrefabs.Length)];

            if (originalObject != null)
            {
                currentObjects[instantiatedCount] = Instantiate(originalObject, Boss.transform.position, Boss.transform.rotation);
                currentObjects[instantiatedCount].tag = "Untagged";
                currentObjects[instantiatedCount].GetComponent<MaterialManager>().ToggleColliderTemporarily();
                Rigidbody rb = currentObjects[instantiatedCount].GetComponent<Rigidbody>();
                DestroyObject(currentObjects[instantiatedCount], 10);
                if (rb != null)
                {
                    rb.isKinematic = false;
                    AddRandomForces(rb);
                }
                instantiatedCount++;
            }
        }
    }

    void MoveAllObjectsToHoldPoints()
    {
        for (int i = 0; i < instantiatedCount; i++)
        {
            if (currentObjects[i] != null)
            {
                Rigidbody rb = currentObjects[i].GetComponent<Rigidbody>();
                if (rb != null && holdPoints[i] != null)
                {
                    MoveObjectToHoldPoint(rb, holdPoints[i]);
                }
            }
        }
    }

    void MoveObjectToHoldPoint(Rigidbody rb, Transform holdPoint)
    {
        Vector3 directionToHoldPoint = holdPoint.position - rb.position;
        float distanceToHoldPoint = directionToHoldPoint.magnitude;
        float forceMagnitude = Mathf.Clamp(distanceToHoldPoint * attractionForce, 0, 500f);
        rb.AddForce(directionToHoldPoint.normalized * forceMagnitude);
        rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);
    }

    public void StartInstantiateSequence()
    {
        StartCoroutine(InstantiateObjects());
    }

    private IEnumerator InstantiateObjects()
    {
        currentObjects = new GameObject[holdPoints.Length];
        int count = 0;
        while (count < 3)
        {
            InstantiateObject();
            yield return new WaitForSeconds(instantiateInterval);
            count++;
        }
    }

    void AddRandomForces(Rigidbody rb)
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        rb.AddForce(randomDirection * horizontalForce, ForceMode.Impulse);

        Vector3 randomTorque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.AddTorque(randomTorque * rotationForce, ForceMode.Impulse);
    }

    public void StartThrowingSequence()
    {
        StartCoroutine(ThrowObjectsPeriodically());
    }

    private IEnumerator ThrowObjectsPeriodically()
    {
        int count = 0;
        while (count < 3)
        {
            ThrowFirstObjectTowardsPlayer();
            yield return new WaitForSeconds(throwInterval);
            count++;
        }
        resetInstantiateCount();
    }

    public void ThrowFirstObjectTowardsPlayer()
    {
        if (currentObjects.Length > 0 && currentObjects[0] != null)
        {
            GameObject obj = currentObjects[0];
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Transform playerTransform = Player.GetComponent<Transform>();

            if (rb != null)
            {
                rb.isKinematic = false;
                Vector3 direction = (playerTransform.position - obj.transform.position).normalized;
                rb.useGravity = false;
                rb.velocity = direction * velocity;
                obj.GetComponent<MaterialManager>().activateTrigger();
                obj.tag = "Throwable";
            }
            RemoveObjectFromArray(0);
        }
    }

    private void RemoveObjectFromArray(int index)
    {
        for (int i = index; i < currentObjects.Length - 1; i++)
        {
            currentObjects[i] = currentObjects[i + 1];
        }
        currentObjects[currentObjects.Length - 1] = null;
    }


    private IEnumerator TriggerSequenceWhenPlayerInside()
    {
        while (playerInTrigger)
        {
            if (!sequenceStarted && bossRagdollScript.isActive != true && canStartSequence)
            {
                StartCombinedSequence();
                sequenceStarted = true;
                yield return new WaitForSeconds(throwCoolDown);
                sequenceStarted = false;
            }
            else
            {
                yield break;
            }
        }
    }

    private void instantiateEnemies()
    {
        if (spawnpoints.Length == 4 && enemiesPacks.Length == 4)
        {
            for (int i = 0; i < spawnpoints.Length; i++)
            {
                if (spawnpoints[i] != null)
                {
                    GameObject enemyToSpawn = enemiesPacks[Random.Range(0, enemiesPacks.Length)];
                    Instantiate(enemyToSpawn, spawnpoints[i].position, Quaternion.identity);
                }
            }
        }
        else
        {
            Debug.LogError("Spawnpoints or enemiesPacks array is not properly set up.");
        }
    }

    public void terminate()
    {
        foreach(GameObject go in currentObjects)
        {
            Destroy(go);
        }
        Destroy(gameObject);
    }

    public void clearObjects()
    {
        foreach (GameObject go in currentObjects)
        {
            if (go != null)
            {
                go.GetComponent<MaterialManager>().activateTrigger();
            }
        }
        currentObjects = new GameObject[holdPoints.Length];
    }
}
