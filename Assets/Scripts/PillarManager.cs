using UnityEngine;
using System.Collections;

public class PillarManager : MonoBehaviour
{
    public float riseHeight;
    public float riseDuration;

    public void StartRising()
    {
        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseHeight;

        while (Time.time - startTime < riseDuration)
        {
            float elapsed = (Time.time - startTime) / riseDuration;
            float smoothedProgress = Mathf.SmoothStep(0.0f, 1.0f, elapsed);
            transform.position = Vector3.Lerp(startPosition, endPosition, smoothedProgress);
            yield return null;
        }

        transform.position = endPosition;
    }
}