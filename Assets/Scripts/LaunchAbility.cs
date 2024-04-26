using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;
    public float maxDistance = 10f;
    public float moveSpeed = 10f;
    public float throwForce = 40f;
    public float destroyTime = 5f;  // ������ڵ�ʱ�䣬������Unity�༭���е���

    private GameObject selectedObject;
    private bool isHolding = false;

    void Update()
    {
        // �����Ұ������������ƻ�Ͷ������
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

        // ������ڳ������壬�����ƶ���ָ����
        if (isHolding && selectedObject != null)
        {
            MoveObjectToHoldPoint();
        }
    }

    void TryPickAndCloneObject()
    {
        RaycastHit hit;
        // ���߼���ж�ǰ���Ƿ�������
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance))
        {
            // ��������Ƿ���� "Throwable" ��ǩ
            if (hit.collider.CompareTag("Throwable"))
            {
                // ��������
                GameObject originalObject = hit.collider.gameObject;
                selectedObject = Instantiate(originalObject, originalObject.transform.position, originalObject.transform.rotation);

                // ȷ�����Ƶ������ Rigidbody �� isKinematic �ģ��Ա����ǿ��Կ�����
                Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                isHolding = true;
            }
        }
    }

    void MoveObjectToHoldPoint()
    {
        // ����������е�
        selectedObject.transform.position = Vector3.Lerp(selectedObject.transform.position, holdPoint.position, Time.deltaTime * moveSpeed);
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