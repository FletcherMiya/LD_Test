using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f;  // 持续抖动的时间，0 表示持续抖动直到显式停止
    public float shakeAmount = 0.7f;  // 抖动幅度
    public float decreaseFactor = 1.0f;  // 抖动衰减速度

    Vector3 originalPos;
    float initialDuration;

    void OnEnable()
    {
        originalPos = transform.localPosition;
        initialDuration = shakeDuration;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else if (shakeDuration <= 0 && initialDuration != 0)
        {
            transform.localPosition = originalPos;
            shakeDuration = 0;
        }
    }

    public void StartShake(float duration, float amount)
    {
        shakeDuration = duration;
        shakeAmount = amount;
        initialDuration = duration;  // Update initial duration for one-time shakes
    }

    public void StopShake()
    {
        shakeDuration = 0;
        transform.localPosition = originalPos;
    }
}