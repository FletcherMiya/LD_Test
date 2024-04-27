using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target; // 跟随的目标物体
    public float followSpeed = 10f; // 跟随速度
    public float yOffset = 0f; // Y轴的偏移量

    public float horizontalRotationSpeed = 5f; // 水平旋转速度
    public float verticalRotationSpeed = 3f; // 垂直旋转速度
    private float verticalRotation = 0f; // 储存垂直旋转角度
    public float minVerticalAngle = -30f; // 垂直旋转的最小角度
    public float maxVerticalAngle = 60f; // 垂直旋转的最大角度

    private Vector2 smoothedMouseInput = Vector2.zero; // 平滑后的鼠标输入
    public float smoothTime = 0.1f; // 输入平滑的时间常数

    void FixedUpdate()
    {
        // 平滑地跟随目标，并添加Y轴的偏移量
        Vector3 targetPositionWithOffset = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPositionWithOffset, followSpeed * Time.deltaTime);

        // 原始鼠标输入
        Vector2 rawMouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        // 使用滑动平均方法平滑鼠标输入
        smoothedMouseInput.x = Mathf.Lerp(smoothedMouseInput.x, rawMouseInput.x, 1 / smoothTime * Time.deltaTime);
        smoothedMouseInput.y = Mathf.Lerp(smoothedMouseInput.y, rawMouseInput.y, 1 / smoothTime * Time.deltaTime);

        // 更新垂直旋转角度，并限制在允许的范围内
        verticalRotation += smoothedMouseInput.y * verticalRotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // 计算旋转
        Quaternion horizontalRotation = Quaternion.AngleAxis(smoothedMouseInput.x * horizontalRotationSpeed, Vector3.up);

        // 应用旋转到当前物体
        transform.rotation = Quaternion.Lerp(transform.rotation, horizontalRotation * transform.rotation, Time.deltaTime * horizontalRotationSpeed);
        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0);
    }
}