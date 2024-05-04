using UnityEngine;

public class JitterEffect : MonoBehaviour
{
    public float positionJitterAmount = 0.1f; // 位置抖动幅度
    public float jitterFrequency = 0.1f;      // 抖动频率，较小的值会使抖动更频繁

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
        // 在原始位置的基础上添加随机偏移
        transform.position = originalPosition + Random.insideUnitSphere * positionJitterAmount;
    }

    void OnDisable()
    {
        // 确保禁用时恢复到原始位置
        transform.position = originalPosition;
    }
}