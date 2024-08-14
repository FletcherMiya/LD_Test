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
        telekinesisAbility = GetComponent<TelekinesisAbility>();

        bulletTimeManager = GetComponent<BulletTimeManager>();
    }

    void FixedUpdate()
    {
        float deltaTime = (bulletTimeManager != null && bulletTimeManager.checkBulletTimeState()) ? Time.unscaledDeltaTime : Time.deltaTime;

        Vector3 targetPositionWithOffset = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPositionWithOffset, followSpeed * deltaTime);

        Vector2 rawMouseInput = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

        if (telekinesisAbility != null && telekinesisAbility.reversed)
        {
            rawMouseInput.x = -rawMouseInput.x;
        }

        smoothedMouseInput.x = Mathf.Lerp(smoothedMouseInput.x, rawMouseInput.x, 1 / smoothTime * deltaTime);
        smoothedMouseInput.y = Mathf.Lerp(smoothedMouseInput.y, rawMouseInput.y, 1 / smoothTime * deltaTime);

        verticalRotation += smoothedMouseInput.y * verticalRotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        Quaternion horizontalRotation = Quaternion.AngleAxis(smoothedMouseInput.x * horizontalRotationSpeed, target.up);

        transform.rotation = horizontalRotation * transform.rotation;

        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0);
    }
}