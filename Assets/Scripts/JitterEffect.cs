using UnityEngine;

public class JitterEffect : MonoBehaviour
{
    public float positionJitterAmount;
    public float jitterFrequency;

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
        transform.position = originalPosition + Random.insideUnitSphere * positionJitterAmount;
    }

    void OnDisable()
    {
        transform.position = originalPosition;
    }
}