using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10f;
    public float yOffset = 0f;

    public float horizontalRotationSpeed = 5f;
    public float verticalRotationSpeed = 3f;
    private float verticalRotation = 0f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;

    private Vector2 smoothedMouseInput = Vector2.zero;
    public float smoothTime = 0.1f;

    private TelekinesisAbility telekinesisAbility;
    private BulletTimeManager bulletTimeManager;

    private void Awake()
    {
        // ��ȡTelekinesisAbility�ű�
        telekinesisAbility = GetComponent<TelekinesisAbility>();

        // ��ȡBulletTimeManager�ű�
        bulletTimeManager = GetComponent<BulletTimeManager>();
    }

    void FixedUpdate()
    {
        // ȷ��ʹ�õ�ʱ�䲽��
        float deltaTime = (bulletTimeManager != null && bulletTimeManager.checkBulletTimeState()) ? Time.unscaledDeltaTime : Time.deltaTime;

        // ����Ŀ��λ��
        Vector3 targetPositionWithOffset = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPositionWithOffset, followSpeed * deltaTime);

        // ��ȡԭʼ�������
        Vector2 rawMouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        // ��� reversed Ϊ true����תˮƽ����
        if (telekinesisAbility != null && telekinesisAbility.reversed)
        {
            rawMouseInput.x = -rawMouseInput.x;
        }

        // ƽ���������
        smoothedMouseInput.x = Mathf.Lerp(smoothedMouseInput.x, rawMouseInput.x, 1 / smoothTime * deltaTime);
        smoothedMouseInput.y = Mathf.Lerp(smoothedMouseInput.y, rawMouseInput.y, 1 / smoothTime * deltaTime);

        // ���㴹ֱ��ת�����ƽǶ�
        verticalRotation += smoothedMouseInput.y * verticalRotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // ����ˮƽ��ת��������ҵľֲ�����ϵ
        Quaternion horizontalRotation = Quaternion.AngleAxis(smoothedMouseInput.x * horizontalRotationSpeed, target.up);

        // Ӧ����ת��ˮƽ��ת�����Ŀ������ľֲ�����ϵ
        transform.rotation = horizontalRotation * transform.rotation;

        // Ӧ�ô�ֱ��ת��pitch������Ӱ�챾�ص�X��
        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0);
    }
}