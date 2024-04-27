using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // �����Ŀ������
    public float followSpeed = 10f; // �����ٶ�
    public float yOffset = 0f; // Y���ƫ����

    public float horizontalRotationSpeed = 5f; // ˮƽ��ת�ٶ�
    public float verticalRotationSpeed = 3f; // ��ֱ��ת�ٶ�
    private float verticalRotation = 0f; // ���洹ֱ��ת�Ƕ�
    public float minVerticalAngle = -30f; // ��ֱ��ת����С�Ƕ�
    public float maxVerticalAngle = 60f; // ��ֱ��ת�����Ƕ�

    private Vector2 smoothedMouseInput = Vector2.zero; // ƽ������������
    public float smoothTime = 0.1f; // ����ƽ����ʱ�䳣��

    void FixedUpdate()
    {
        // ƽ���ظ���Ŀ�꣬�����Y���ƫ����
        Vector3 targetPositionWithOffset = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPositionWithOffset, followSpeed * Time.deltaTime);

        // ԭʼ�������
        Vector2 rawMouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        // ʹ�û���ƽ������ƽ���������
        smoothedMouseInput.x = Mathf.Lerp(smoothedMouseInput.x, rawMouseInput.x, 1 / smoothTime * Time.deltaTime);
        smoothedMouseInput.y = Mathf.Lerp(smoothedMouseInput.y, rawMouseInput.y, 1 / smoothTime * Time.deltaTime);

        // ���´�ֱ��ת�Ƕȣ�������������ķ�Χ��
        verticalRotation += smoothedMouseInput.y * verticalRotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // ������ת
        Quaternion horizontalRotation = Quaternion.AngleAxis(smoothedMouseInput.x * horizontalRotationSpeed, Vector3.up);

        // Ӧ����ת����ǰ����
        transform.rotation = Quaternion.Lerp(transform.rotation, horizontalRotation * transform.rotation, Time.deltaTime * horizontalRotationSpeed);
        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0);
    }
}