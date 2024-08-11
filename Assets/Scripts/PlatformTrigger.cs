using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameObject platform;
    public float moveDistance;
    public float moveDuration;

    private Vector3 originalPosition;
    private bool isOut = false;
    private bool isIn = true;

    void Start()
    {
        if (platform != null)
        {
            originalPosition = platform.transform.position;
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
            StopAllCoroutines();
            StartCoroutine(MovePlatform(originalPosition, originalPosition + Vector3.right * moveDistance));
            isOut = true;
            isIn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isIn)
        {
            StopAllCoroutines();
            StartCoroutine(MovePlatform(platform.transform.position, originalPosition));
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

        platform.transform.position = endPosition;
    }
}
