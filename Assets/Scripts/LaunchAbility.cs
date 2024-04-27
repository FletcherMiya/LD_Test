using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;
    public float maxDistance = 10f;
    public float moveSpeed = 10f;
    public float throwForce = 40f;
    public float destroyTime = 5f;  // ������ڵ�ʱ��
    public float attractionForce = 50f; // ������holdPointʱʩ�ӵ�����С
    public float shakeIntensity = 10f; // ҡ��ǿ��
    public float damping = 0.5f; // ����ϵ�����������嵽��hold point����񶯷���
    public float riseHeight = 3.0f; //�����߶�
    public float initialUpwardForce = 10f;
    public float delayBetweenStages = 0.5f;  // �׶μ��ӳ�
    private int stage = 0;  // 0 = ��ʼ, 1 = ����, 2 = ����holdPoint
    private float stageChangeTime;  // �׶θı��ʱ���
    private GameObject selectedObject;
    private bool isHolding = false;
    private Vector3 risePoint;  // �̶�����Ŀ���

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
                    rb.isKinematic = false; // ȷ�����Ƶ����岻�� kinematic
                }
                isHolding = true;
                stage = 0;

                // ���㲢�̶� risePoint ���� originalObject ��λ��
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
            case 0:  // ��ʼ�׶�
                stage = 1;
                stageChangeTime = Time.time;
                break;

            case 1:  // �����׶�
                Vector3 directionToRisePoint = risePoint - selectedObject.transform.position;
                float distanceToRisePoint = directionToRisePoint.magnitude;

                if (distanceToRisePoint > 0.1f)
                {
                    float riseForceMagnitude = Mathf.Clamp(distanceToRisePoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToRisePoint.normalized * riseForceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);  // Ӧ��������ƽ������
                }
                else
                {
                    stage = 2;
                    stageChangeTime = Time.time;
                    rb.velocity = Vector3.zero;  // ����ٶȣ�׼����һ�׶�
                }
                break;

            case 2:  // ����holdPoint�׶�
                Vector3 directionToHoldPoint = holdPoint.position - selectedObject.transform.position;
                float distanceToHoldPoint = directionToHoldPoint.magnitude;
                if (Time.time - stageChangeTime >= delayBetweenStages)
                {
                    float forceMagnitude = Mathf.Clamp(distanceToHoldPoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToHoldPoint.normalized * forceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);  // ����Ӧ��������ƽ���ƶ�
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

            // ������һ��ʱ�����������
            Destroy(selectedObject, destroyTime);

            selectedObject = null;
            isHolding = false;
        }
    }
}