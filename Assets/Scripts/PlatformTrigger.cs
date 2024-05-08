using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameObject platform;  // 指向平台的引用
    public float moveDistance;  // 沿X轴移动的距离
    public float moveDuration;  // 移动所需的时间

    private Vector3 originalPosition;  // 平台的原始位置
    private bool isOut = false;
    private bool isIn = true;

    void Start()
    {
        if (platform != null)
        {
            originalPosition = platform.transform.position;  // 记录平台的初始位置
        }
        else
        {
            Debug.LogError("Platform reference not set in the inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOut)
        {
            StopAllCoroutines();  // 停止所有当前协程以避免重叠运动
            StartCoroutine(MovePlatform(originalPosition, originalPosition + Vector3.right * moveDistance));  // 向右移动平台
            isOut = true;
            isIn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isIn)
        {
            StopAllCoroutines();  // 停止所有当前协程以避免重叠运动
            StartCoroutine(MovePlatform(platform.transform.position, originalPosition));  // 移动平台回到原始位置
            isIn = true;
            isOut = false;
        }
    }

    private IEnumerator MovePlatform(Vector3 startPosition, Vector3 endPosition)
    {
        float startTime = Time.time;

        while (Time.time - startTime < moveDuration)
        {
            float elapsed = (Time.time - startTime) / moveDuration;
            float smoothedProgress = Mathf.SmoothStep(0.0f, 1.0f, elapsed);
            platform.transform.position = Vector3.Lerp(startPosition, endPosition, smoothedProgress);
            yield return null;
        }

        platform.transform.position = endPosition;  // 确保平台完全移动到结束位置
    }
}
