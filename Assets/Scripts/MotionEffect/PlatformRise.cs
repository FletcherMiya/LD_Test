using System.Collections;
using UnityEngine;

public class PlatformRise : MonoBehaviour
{
    public Transform[] platforms;
    public float riseDistance; // 上升距离
    public float delayBetweenPlatforms; // 间隔base
    public float delayRandomRange; // 间隔浮动
    public float riseDuration; // 上升秒数
    public float initialDelay; // 全局等待事件
    public float initialSpeed; // 初始速度
    public float decelerationFactor;

    private bool hasRisen = false;

    private void Awake()
    {
        transform.position += Vector3.down * riseDistance;
        if (platforms.Length == 0)
        {
            platforms = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                platforms[i] = transform.GetChild(i);
            }
        }

        foreach (Transform platform in platforms)
        {
            platform.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasRisen)
        {
            StartCoroutine(StartPlatformRiseAfterDelay());
            hasRisen = true;
        }
    }

    private IEnumerator StartPlatformRiseAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);

        foreach (Transform platform in platforms)
        {
            StartCoroutine(RaisePlatform(platform));
            float delay = delayBetweenPlatforms + Random.Range(-delayRandomRange, delayRandomRange);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator RaisePlatform(Transform platform)
    {
        platform.gameObject.SetActive(true);

        float startTime = Time.time;
        Vector3 startPosition = platform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseDistance;

        while (Time.time - startTime < riseDuration)
        {
            float elapsed = (Time.time - startTime) / riseDuration;

            float linearProgress = elapsed;
            float smoothedProgress = Mathf.Pow(linearProgress, decelerationFactor);
            smoothedProgress = Mathf.SmoothStep(0.0f, 1.0f, smoothedProgress);

            platform.position = Vector3.Lerp(startPosition, endPosition, smoothedProgress);
            yield return null;
        }

        platform.position = endPosition;
    }
}