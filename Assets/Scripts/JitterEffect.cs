using UnityEngine;

public class JitterEffect : MonoBehaviour
{
    public float positionJitterAmount = 0.1f; // λ�ö�������
    public float jitterFrequency = 0.1f;      // ����Ƶ�ʣ���С��ֵ��ʹ������Ƶ��

    private Vector3 originalPosition;
    private float nextJitterTime;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Time.time >= nextJitterTime)
        {
            Jitter();
            nextJitterTime = Time.time + jitterFrequency;
        }
    }

    void Jitter()
    {
        // ��ԭʼλ�õĻ�����������ƫ��
        transform.position = originalPosition + Random.insideUnitSphere * positionJitterAmount;
    }

    void OnDisable()
    {
        // ȷ������ʱ�ָ���ԭʼλ��
        transform.position = originalPosition;
    }
}