using System.Collections;
using UnityEngine;

public class PlatformScaleUp : MonoBehaviour
{
    public Transform[] platforms;
    public Vector3 targetScale = Vector3.one; // Ŀ��ߴ�
    public float delayBetweenPlatforms; // ���base
    public float delayRandomRange; // �������
    public float scaleStepDuration;
    public float decelerationFactor;
    public float initialDelay; // ȫ�ֵȴ�ʱ��
    public float rotationForce;

    private bool hasScaled = false;

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

        foreach (Transform platform in platforms)
        {
            platform.localScale = Vector3.zero;
            platform.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasScaled)
        {
            StartCoroutine(StartPlatformScaleUpAfterDelay());
            hasScaled = true;
        }
    }

    private IEnumerator StartPlatformScaleUpAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);

        foreach (Transform platform in platforms)
        {
            StartCoroutine(ScaleUpPlatform(platform));
            float delay = delayBetweenPlatforms + Random.Range(-delayRandomRange, delayRandomRange);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator ScaleUpPlatform(Transform platform)
    {
        platform.gameObject.SetActive(true);

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
        /*
        if (rb == null)
        {
            rb = platform.gameObject.AddComponent<Rigidbody>(); // �������û��Rigidbody�����һ��
        }
        */

        Vector3 torque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.AddTorque(torque * rotationForce, ForceMode.Impulse);
    }
}