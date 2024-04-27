using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;
    public float maxDistance = 10f;
    public float moveSpeed = 10f;
    public float throwForce = 40f;
    public float destroyTime = 5f;  // 物体存在的时间
    public float attractionForce = 50f; // 吸引至holdPoint时施加的力大小
    public float shakeIntensity = 10f; // 摇晃强度
    public float damping = 0.5f; // 阻尼系数，减少物体到达hold point后的振动幅度
    public float riseHeight = 3.0f; //上升高度
    public float initialUpwardForce = 10f;
    public float delayBetweenStages = 0.5f;  // 阶段间延迟
    private int stage = 0;  // 0 = 初始, 1 = 上升, 2 = 移向holdPoint
    private float stageChangeTime;  // 阶段改变的时间点
    private GameObject selectedObject;
    private bool isHolding = false;
    private Vector3 risePoint;  // 固定上升目标点

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

        if (isHolding && selectedObject != null)
        {
            MoveObjectToHoldPoint();
        }
    }

    void TryPickAndCloneObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Throwable"))
            {
                GameObject originalObject = hit.collider.gameObject;
                selectedObject = Instantiate(originalObject, originalObject.transform.position, originalObject.transform.rotation);

                Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // 确保复制的物体不是 kinematic
                }
                isHolding = true;
                stage = 0;

                // 计算并固定 risePoint 基于 originalObject 的位置
                risePoint = new Vector3(originalObject.transform.position.x, originalObject.transform.position.y + riseHeight, originalObject.transform.position.z);
            }
        }
    }

    void MoveObjectToHoldPoint()
    {
        if (selectedObject == null) return;

        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        switch (stage)
        {
            case 0:  // 初始阶段
                stage = 1;
                stageChangeTime = Time.time;
                break;

            case 1:  // 上升阶段
                Vector3 directionToRisePoint = risePoint - selectedObject.transform.position;
                float distanceToRisePoint = directionToRisePoint.magnitude;

                if (distanceToRisePoint > 0.1f)
                {
                    float riseForceMagnitude = Mathf.Clamp(distanceToRisePoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToRisePoint.normalized * riseForceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);  // 应用阻尼以平滑上升
                }
                else
                {
                    stage = 2;
                    stageChangeTime = Time.time;
                    rb.velocity = Vector3.zero;  // 清除速度，准备下一阶段
                }
                break;

            case 2:  // 移向holdPoint阶段
                Vector3 directionToHoldPoint = holdPoint.position - selectedObject.transform.position;
                float distanceToHoldPoint = directionToHoldPoint.magnitude;
                if (Time.time - stageChangeTime >= delayBetweenStages)
                {
                    float forceMagnitude = Mathf.Clamp(distanceToHoldPoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToHoldPoint.normalized * forceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);  // 继续应用阻尼以平滑移动
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

            // 安排在一定时间后销毁物体
            Destroy(selectedObject, destroyTime);

            selectedObject = null;
            isHolding = false;
        }
    }
}