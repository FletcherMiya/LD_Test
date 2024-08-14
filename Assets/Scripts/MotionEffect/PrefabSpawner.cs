using System.Collections;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    public GameObject spawnedPrefab = null;

    public bool activated = true;

    public float spawnDelay = 2f;

    public float scaleStepDuration = 0.5f;
    public float decelerationFactor = 2f;
    public float rotationForce = 10f;
    public Vector3 targetScale = Vector3.one;

    private Transform spawnPoint;
    private bool isSpawning = false;

    private void Awake()
    {
        spawnPoint = transform.Find("spawnPoint");
    }

    private void CheckAndSpawnPrefab()
    {
        if (spawnedPrefab == null && !isSpawning)
        {
            StartCoroutine(SpawnPrefabAfterDelay());
        }
    }

    private IEnumerator SpawnPrefabAfterDelay()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnDelay);

        GameObject newObject = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        newObject.transform.localScale = Vector3.zero;

        spawnedPrefab = newObject;

        StartCoroutine(ScaleUpPlatform(newObject.transform));

        isSpawning = false;
    }

    private IEnumerator ScaleUpPlatform(Transform platform)
    {
        yield return ScaleToTarget(platform, targetScale * 0.33f);

        yield return ScaleToTarget(platform, targetScale * 0.66f);

        AddRandomRotation(platform);

        yield return ScaleToTarget(platform, targetScale);
    }

    private IEnumerator ScaleToTarget(Transform platform, Vector3 target)
    {
        Vector3 initialScale = platform.localScale;
        float startTime = Time.time;

        while (Time.time - startTime < scaleStepDuration)
        {
            float elapsed = (Time.time - startTime) / scaleStepDuration;
            float smoothedProgress = Mathf.SmoothStep(0f, 1f, Mathf.Pow(elapsed, decelerationFactor));

            platform.localScale = Vector3.Lerp(initialScale, target, smoothedProgress);
            yield return null;
        }

        platform.localScale = target;
    }

    private void AddRandomRotation(Transform platform)
    {
        Rigidbody rb = platform.gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = platform.gameObject.AddComponent<Rigidbody>();
        }

        Vector3 torque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.AddTorque(torque * rotationForce, ForceMode.Impulse);
    }

    private void Update()
    {
        if (activated && spawnedPrefab == null && !isSpawning)
        {
            CheckAndSpawnPrefab();
        }
    }
}