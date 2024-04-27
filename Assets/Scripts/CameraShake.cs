using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0f;  // ����������ʱ�䣬0 ��ʾ��������ֱ����ʽֹͣ
    public float shakeAmount = 0.7f;  // ��������
    public float decreaseFactor = 1.0f;  // ����˥���ٶ�

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