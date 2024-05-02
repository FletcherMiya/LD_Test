using UnityEngine;
using System.Collections;

public class LockedDoorController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public GameObject slot; // Slot游戏对象
    public float rotationDuration = 2.0f; // 门完全开启所需时间
    private bool isOpened = false; // 用来确保门只被开启一次

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            SlotTriggerHandler slotTriggerHandler = slot.GetComponent<SlotTriggerHandler>();
            if (slotTriggerHandler != null && slotTriggerHandler.activated)
            {
                isOpened = true; // 确保门只开启一次
                StartCoroutine(OpenDoors());
            }
        }
    }

    IEnumerator OpenDoors()
    {
        float timeElapsed = 0;
        Quaternion leftStartRotation = leftDoor.rotation;
        Quaternion rightStartRotation = rightDoor.rotation;
        Quaternion leftEndRotation = leftStartRotation * Quaternion.Euler(0, -90, 0); // 左门向外旋转90度
        Quaternion rightEndRotation = rightStartRotation * Quaternion.Euler(0, 90, 0); // 右门向外旋转90度

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