using UnityEngine;
using System.Collections;

public class LockedDoorController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public GameObject slot; // Slot��Ϸ����
    public float rotationDuration = 2.0f; // ����ȫ��������ʱ��
    private bool isOpened = false; // ����ȷ����ֻ������һ��

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            SlotTriggerHandler slotTriggerHandler = slot.GetComponent<SlotTriggerHandler>();
            if (slotTriggerHandler != null && slotTriggerHandler.activated)
            {
                isOpened = true; // ȷ����ֻ����һ��
                StartCoroutine(OpenDoors());
            }
        }
    }

    IEnumerator OpenDoors()
    {
        float timeElapsed = 0;
        Quaternion leftStartRotation = leftDoor.rotation;
        Quaternion rightStartRotation = rightDoor.rotation;
        Quaternion leftEndRotation = leftStartRotation * Quaternion.Euler(0, -90, 0); // ����������ת90��
        Quaternion rightEndRotation = rightStartRotation * Quaternion.Euler(0, 90, 0); // ����������ת90��

        while (timeElapsed < rotationDuration)
        {
            leftDoor.rotation = Quaternion.Slerp(leftStartRotation, leftEndRotation, timeElapsed / rotationDuration);
            rightDoor.rotation = Quaternion.Slerp(rightStartRotation, rightEndRotation, timeElapsed / rotationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        leftDoor.rotation = leftEndRotation;
        rightDoor.rotation = rightEndRotation;
    }
}