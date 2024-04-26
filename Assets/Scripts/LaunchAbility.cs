using UnityEngine;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPoint;
    public float maxDistance = 10f;
    public float moveSpeed = 10f;
    public float throwForce = 40f;
    public float destroyTime = 5f;  // 物体存在的时间，可以在Unity编辑器中调整

    private GameObject selectedObject;
    private bool isHolding = false;

    void Update()
    {
        // 检测玩家按键，决定复制或投掷物体
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

        // 如果正在持有物体，将其移动到指定点
        if (isHolding && selectedObject != null)
        {
            MoveObjectToHoldPoint();
        }
    }

    void TryPickAndCloneObject()
    {
        RaycastHit hit;
        // 射线检测判断前方是否有物体
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxDistance))
        {
            // 检查物体是否具有 "Throwable" 标签
            if (hit.collider.CompareTag("Throwable"))
            {
                // 复制物体
                GameObject originalObject = hit.collider.gameObject;
                selectedObject = Instantiate(originalObject, originalObject.transform.position, originalObject.transform.rotation);

                // 确保复制的物体的 Rigidbody 是 isKinematic 的，以便我们可以控制它
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
        // 物体移向持有点
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

            // 安排在一定时间后销毁物体
            Destroy(selectedObject, destroyTime);

            selectedObject = null;
            isHolding = false;
        }
    }
}