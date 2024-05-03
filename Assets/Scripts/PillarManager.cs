using UnityEngine;
using System.Collections;

public class PillarManager : MonoBehaviour
{
    public float riseHeight; // 上升的高度
    public float riseDuration; // 完成上升所需的总时间

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

        transform.position = endPosition; // 确保对象到达最终位置
    }
}