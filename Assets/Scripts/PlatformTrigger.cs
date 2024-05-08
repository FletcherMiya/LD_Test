using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public GameObject platform;  // ָ��ƽ̨������
    public float moveDistance;  // ��X���ƶ��ľ���
    public float moveDuration;  // �ƶ������ʱ��

    private Vector3 originalPosition;  // ƽ̨��ԭʼλ��
    private bool isOut = false;
    private bool isIn = true;

    void Start()
    {
        if (platform != null)
        {
            originalPosition = platform.transform.position;  // ��¼ƽ̨�ĳ�ʼλ��
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
            StopAllCoroutines();  // ֹͣ���е�ǰЭ���Ա����ص��˶�
            StartCoroutine(MovePlatform(originalPosition, originalPosition + Vector3.right * moveDistance));  // �����ƶ�ƽ̨
            isOut = true;
            isIn = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isIn)
        {
            StopAllCoroutines();  // ֹͣ���е�ǰЭ���Ա����ص��˶�
            StartCoroutine(MovePlatform(platform.transform.position, originalPosition));  // �ƶ�ƽ̨�ص�ԭʼλ��
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

        platform.transform.position = endPosition;  // ȷ��ƽ̨��ȫ�ƶ�������λ��
    }
}
