using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;
    public float maxDistance = 10f;
    public float sphereRadius = 1.0f;

    public float throwForce = 40f;
    public float destroyTime = 5f;  // 物体存在的时间
    public float attractionForce = 50f; // 吸引至holdPoint时施加的力大小
    public float damping = 0.5f; // 阻尼系数，减少物体到达hold point后的振动幅度
    public float riseHeight = 3.0f; //上升高度
    public float initialUpwardForce = 10f;
    public float horizontalForce = 5f; // 控制上升阶段开始时施加的水平方向力的大小
    public float rotationForce = 10f;　// 控制旋转力的大小
    public float delayBetweenStages = 0.5f;  // 阶段间延迟


    private int stage = 0;  // 0 = 初始, 1 = 上升, 2 = 移向holdPoint
    private float stageChangeTime;  // 阶段改变的时间点


    private GameObject selectedObject;
    private GameObject originalObject;  // 原始被复制的物体
    private Vector3 risePoint;  // 固定上升目标点


    private GameObject lastHighlighted = null;


    private bool isHolding = false;
    private bool hasAppliedHorizontalForce = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
            {
                TryPickAndCloneObject();
            }
            else
            {
                ThrowObject();
            }
        }

        if (!isHolding)
        {
            HighlightObjectUnderCrosshair();
        }
                else if (lastHighlighted != null)
        {
            lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            lastHighlighted = null;
        }
    }

    private void FixedUpdate()
    {
        if (isHolding && selectedObject != null)
        {
            MoveObjectToHoldPoint();
        }
    }

    void TryPickAndCloneObject()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);

        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, maxDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Throwable"))
            {
                originalObject = hit.collider.gameObject;
                selectedObject = Instantiate(originalObject, originalObject.transform.position, originalObject.transform.rotation);
                Physics.IgnoreCollision(selectedObject.GetComponent<Collider>(), originalObject.GetComponent<Collider>(), true);
                Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // 确保复制的物体不是kinematic
                }
                isHolding = true;
                stage = 0;
                risePoint = new Vector3(originalObject.transform.position.x, originalObject.transform.position.y + riseHeight, originalObject.transform.position.z);
                break; // 找到第一个合适的物体后停止搜索
            }
        }
    }

    void MoveObjectToHoldPoint()
    {
        if (selectedObject == null) return;

        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        switch (stage)
        {
            case 0:
                stage = 1;
                stageChangeTime = Time.time;
                hasAppliedHorizontalForce = false;
                break;
            case 1:
                Vector3 directionToRisePoint = risePoint - selectedObject.transform.position;
                float distanceToRisePoint = directionToRisePoint.magnitude;
                if (!hasAppliedHorizontalForce)
                {
                    Vector3 horizontalDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                    rb.AddForce(horizontalDir * horizontalForce, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    rb.AddTorque(torque * rotationForce, ForceMode.Impulse);
                    hasAppliedHorizontalForce = true;
                }
                if (distanceToRisePoint > 0.1f)
                {
                    float riseForceMagnitude = Mathf.Clamp(distanceToRisePoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToRisePoint.normalized * riseForceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);
                }
                else
                {
                    stage = 2;
                    stageChangeTime = Time.time;
                    rb.velocity = Vector3.zero;
                    Physics.IgnoreCollision(selectedObject.GetComponent<Collider>(), originalObject.GetComponent<Collider>(), false);
                }
                break;
            case 2:
                Vector3 directionToHoldPoint = holdPoint.position - selectedObject.transform.position;
                float distanceToHoldPoint = directionToHoldPoint.magnitude;
                if (Time.time - stageChangeTime >= delayBetweenStages)
                {
                    float forceMagnitude = Mathf.Clamp(distanceToHoldPoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToHoldPoint.normalized * forceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);
                }
                break;
        }
    }

    void ThrowObject()
    {
        if (selectedObject != null)
        {
            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            }
            Destroy(selectedObject, destroyTime);
            selectedObject = null;
            originalObject = null;
            isHolding = false;
        }
    }

    void HighlightObjectUnderCrosshair()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, maxDistance);

        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Throwable"))
            {
                float distance = hit.distance; // 获取当前物体距离摄像机的距离
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.collider.gameObject;
                }
            }
        }

        if (closestObject != null && lastHighlighted != closestObject)
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();  // 移除上一个物体的高亮
            }
            closestObject.GetComponent<MaterialManager>().ApplyHighlight();  // 应用高亮效果
            lastHighlighted = closestObject; // 更新最后一个高亮的物体
        }
        else if (closestObject == null && lastHighlighted != null)
        {
            lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            lastHighlighted = null;
        }
    }
}