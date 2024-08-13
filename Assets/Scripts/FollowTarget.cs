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
        // 获取TelekinesisAbility脚本
        telekinesisAbility = GetComponent<TelekinesisAbility>();

        // 获取BulletTimeManager脚本
        bulletTimeManager = GetComponent<BulletTimeManager>();
    }

    void FixedUpdate()
    {
        // 确定使用的时间步长
        float deltaTime = (bulletTimeManager != null && bulletTimeManager.checkBulletTimeState()) ? Time.unscaledDeltaTime : Time.deltaTime;

        // 跟随目标位置
        Vector3 targetPositionWithOffset = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPositionWithOffset, followSpeed * deltaTime);

        // 获取原始鼠标输入
        Vector2 rawMouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        // 如果 reversed 为 true，则反转水平输入
        if (telekinesisAbility != null && telekinesisAbility.reversed)
        {
            rawMouseInput.x = -rawMouseInput.x;
        }

        // 平滑鼠标输入
        smoothedMouseInput.x = Mathf.Lerp(smoothedMouseInput.x, rawMouseInput.x, 1 / smoothTime * deltaTime);
        smoothedMouseInput.y = Mathf.Lerp(smoothedMouseInput.y, rawMouseInput.y, 1 / smoothTime * deltaTime);

        // 计算垂直旋转并限制角度
        verticalRotation += smoothedMouseInput.y * verticalRotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // 计算水平旋转，基于玩家的局部坐标系
        Quaternion horizontalRotation = Quaternion.AngleAxis(smoothedMouseInput.x * horizontalRotationSpeed, target.up);

        // 应用旋转，水平旋转相对于目标物体的局部坐标系
        transform.rotation = horizontalRotation * transform.rotation;

        // 应用垂直旋转（pitch），仅影响本地的X轴
        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0);
    }
}