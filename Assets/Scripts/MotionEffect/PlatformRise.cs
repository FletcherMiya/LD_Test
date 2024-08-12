using System.Collections;
using UnityEngine;

public class PlatformRise : MonoBehaviour
{
    public Transform[] platforms;
    public float riseDistance = 2f; // ��������
    public float delayBetweenPlatforms = 0.2f; // ���base
    public float delayRandomRange = 0.05f; // �������
    public float riseDuration = 1f; // ��������
    public float initialDelay = 0f; // ȫ�ֵȴ��¼�
    public float initialSpeed = 1f; // ��ʼ�ٶ�

    private bool hasRisen = false;

    private void Start()
    {
        if (platforms.Length == 0)
        {
            platforms = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                platforms[i] = transform.GetChild(i);
            }
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
        float startTime = Time.time;
        Vector3 startPosition = platform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseDistance;

        while (Time.time - startTime < riseDuration)
        {
            float elapsed = (Time.time - startTime) / riseDuration;

            float linearProgress = elapsed;
            float smoothedProgress = Mathf.SmoothStep(0.0f, 1.0f, linearProgress);

            platform.position = Vector3.Lerp(startPosition, endPosition, smoothedProgress);
            yield return null;
        }

        platform.position = endPosition;
    }
}