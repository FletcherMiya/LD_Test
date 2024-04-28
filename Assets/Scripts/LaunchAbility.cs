using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera; //主摄像机
    public Transform holdPoint; //物体漂浮点
    public float maxDistance; //最大抓取距离
    public float sphereRadius; //判定球大小

    public float throwForce; //投掷力度
    public float destroyTime;  // 物体存在时间
    public float attractionForce; // 吸引至漂浮点时的吸力大小
    public float damping; // 阻尼系数
    public float riseHeight; //上升高度
    public float initialUpwardForce; //初始上升力度
    public float horizontalForce; // 随机水平力大小
    public float rotationForce = 10f;　// 随机旋转大小
    public float delayBetweenStages = 0.5f;  // 阶段间延迟


    private int stage = 0;  // 0 = 初始, 1 = 上升, 2 = 移向holdPoint
    private float stageChangeTime;  // 阶段改变的时间点


    private GameObject selectedObject; //复制出的物体
    private GameObject originalObject;  // 原始物体
    private Vector3 risePoint;  // 上升目标点


    private GameObject lastHighlighted = null; //当前有高亮效果的物体


    private bool isHolding = false; //玩家是否正在抓取
    private bool hasAppliedHorizontalForce = false;

    public RectTransform slotMarker;

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
                GameObject slot = FindClosestObjectByTag("Slot");
                if (slot != null)
                {
                    ThrowObjectTowardsSlot(selectedObject, slot);
                }
                else
                {
                    ThrowObject();
                }
            }
        }

        if (isHolding)
        {
            GameObject closestSlot = FindClosestObjectByTag("Slot");
            if (closestSlot != null)
            {
                ShowSlotMarker(closestSlot);
            }
            else
            {
                slotMarker.gameObject.SetActive(false);
            }
        }
        else
        {
            slotMarker.gameObject.SetActive(false);
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
        originalObject = FindClosestObjectByTag("Throwable");

        if (originalObject != null)
        {
            selectedObject = Instantiate(originalObject, originalObject.transform.position, originalObject.transform.rotation);
            Physics.IgnoreCollision(selectedObject.GetComponent<Collider>(), originalObject.GetComponent<Collider>(), true);
            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            isHolding = true;
            stage = 0;
            risePoint = new Vector3(originalObject.transform.position.x, originalObject.transform.position.y + riseHeight, originalObject.transform.position.z);
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

    void ThrowObjectTowardsSlot(GameObject obj, GameObject slot)
    {
        if (obj != null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                Vector3 direction = (slot.transform.position - obj.transform.position).normalized;
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
            }
            Destroy(obj, destroyTime);
            selectedObject = null;
            originalObject = null;
            isHolding = false;
        }
    }

    void HighlightObjectUnderCrosshair()
    {
        GameObject closestObject = FindClosestObjectByTag("Throwable");

        if (closestObject != null && lastHighlighted != closestObject)
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            }
            closestObject.GetComponent<MaterialManager>().ApplyHighlight();
            lastHighlighted = closestObject;
        }
        else if (closestObject == null && lastHighlighted != null)
        {
            lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            lastHighlighted = null;
        }
    }

    GameObject FindClosestObjectByTag(string tag)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, maxDistance);

        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag(tag))
            {
                float distance = hit.distance;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hit.collider.gameObject;
                }
            }
        }

        return closestObject;
    }

    void ShowSlotMarker(GameObject slot)
    {
        Vector3 viewportPosition = playerCamera.WorldToViewportPoint(slot.transform.position);
        if (viewportPosition.z > 0 && !slot.GetComponent<SlotTriggerHandler>().activated && selectedObject.GetComponent<MaterialManager>().isKey)
        {
            Vector2 targetPosition = new Vector2((viewportPosition.x - 0.5f) * slotMarker.parent.GetComponent<RectTransform>().sizeDelta.x, (viewportPosition.y - 0.5f) * slotMarker.parent.GetComponent<RectTransform>().sizeDelta.y);
            if (!slotMarker.gameObject.activeSelf)
            {
                slotMarker.anchoredPosition = targetPosition;
            }

            slotMarker.gameObject.SetActive(true);
            slotMarker.anchoredPosition = Vector2.Lerp(slotMarker.anchoredPosition, targetPosition, Time.deltaTime * 120);
        }
        else
        {
            slotMarker.gameObject.SetActive(false);
        }
    }
}