using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera; //�������
    public Transform holdPoint; //����Ư����
    public float maxDistance; //���ץȡ����
    public float sphereRadius; //�ж����С

    public float throwForce; //Ͷ������
    public float destroyTime;  // �������ʱ��
    public float attractionForce; // ������Ư����ʱ��������С
    public float damping; // ����ϵ��
    public float riseHeight; //�����߶�
    public float initialUpwardForce; //��ʼ��������
    public float horizontalForce; // ���ˮƽ����С
    public float rotationForce = 10f;��// �����ת��С
    public float delayBetweenStages = 0.5f;  // �׶μ��ӳ�


    private int stage = 0;  // 0 = ��ʼ, 1 = ����, 2 = ����holdPoint
    private float stageChangeTime;  // �׶θı��ʱ���


    private GameObject selectedObject; //���Ƴ�������
    private GameObject originalObject;  // ԭʼ����
    private Vector3 risePoint;  // ����Ŀ���


    private GameObject lastHighlighted = null; //��ǰ�и���Ч��������


    private bool isHolding = false; //����Ƿ�����ץȡ
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