using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Invector.vCharacterController;

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

    public int shieldHitcount; //玩家攻击护盾次数计数
    public int shieldCount; //最大护盾层数

    private vRagdoll bossRagdollScript;

    private int state = 0; //0 = 攻击，1 = 易伤 2 = 回血

    void Awake()
    {
        currentObjects = new GameObject[holdPoints.Length];
        bossRagdollScript = Boss.GetComponentInParent<vRagdoll>();
        state = 0;
        shieldHitcount = 0;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        MoveAllObjectsToHoldPoints();
        if (Player != null)
        {
            transform.LookAt(Player.transform);
        }
        if (shieldHitcount == shieldCount)
        {
            state = 1;
        }
        if (state == 0)
        {
            MoveAllObjectsToHoldPoints();
        }
        else if (state == 1)
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player == null && other.CompareTag("Player"))
        {
            Player = other.gameObject.transform.Find("3D Model").transform.Find("HealTarget").gameObject;
            Debug.Log(Player);
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

    /*
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
                rb.velocity = Vector3.zero;
                Vector3 direction = (playerTransform.position - obj.transform.position).normalized;
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
                obj.GetComponent<MaterialManager>().activateTrigger();
                obj.tag = "Throwable";
                Destroy(obj, destroyTime);
            }
            RemoveObjectFromArray(0);
        }
    }
    */

    public void ThrowFirstObjectTowardsPlayer()
    {
        if (currentObjects.Length > 0 && currentObjects[0] != null)
        {
            GameObject obj = currentObjects[0];
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            Transform playerTransform = Player.GetComponent<Transform>();

            if (rb != null)
            {
                rb.isKinematic = false; // 确保物体可以被物理引擎影响
                Vector3 direction = (playerTransform.position - obj.transform.position).normalized;

                rb.useGravity = false;
                // 设置物体的速度，确保它匀速移动
                rb.velocity = direction * velocity; // 这里的 throwForce 作为速度的标量值使用

                // 激活物体的某些效果，如材质变更
                obj.GetComponent<MaterialManager>().activateTrigger();

                // 标记物体，便于以后识别
                obj.tag = "Throwable";

                // 在到达目的地或某个时间后销毁物体
                Destroy(obj, destroyTime);
            }
            // 从数组中移除该物体
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
            if (!sequenceStarted && bossRagdollScript.isActive != true)
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
}
