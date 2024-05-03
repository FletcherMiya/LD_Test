using UnityEngine;
using System.Collections;

public class LockedDoorController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public GameObject slot; // Slot��Ϸ����
    public GameObject greenLight;
    public GameObject redLight;
    public float rotationDuration = 2.0f; // ����ȫ��������ʱ��
    private bool isOpened = false; // ����ȷ����ֻ������һ��
    private bool isClosed = true;

    private MeshRenderer greenLightRenderer;
    private MeshRenderer redLightRenderer;
    private SlotTriggerHandler slotTriggerHandler;

    public Material greenLightActiveMaterial;
    public Material greenLightInactiveMaterial;
    public Material redLightActiveMaterial;
    public Material redLightInactiveMaterial;

    public float closeDelay;

    void Start()
    {
        slotTriggerHandler = slot.GetComponent<SlotTriggerHandler>();
        greenLightRenderer = greenLight.GetComponent<MeshRenderer>();
        redLightRenderer = redLight.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        UpdateLightMaterials(slotTriggerHandler.activated);
        if(!slotTriggerHandler.activated && !isClosed)
        {
            isClosed = true;
            StartCoroutine(CloseDoors());
            isOpened = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            if (slotTriggerHandler != null && slotTriggerHandler.activated)
            {
                isOpened = true; // ȷ����ֻ����һ��
                StartCoroutine(OpenDoors());
                isClosed = false;
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

    IEnumerator CloseDoors()
    {
        yield return new WaitForSeconds(closeDelay);
        float timeElapsed = 0;
        Quaternion leftStartRotation = leftDoor.rotation;
        Quaternion rightStartRotation = rightDoor.rotation;
        Quaternion leftEndRotation = leftDoor.rotation * Quaternion.Euler(0, 90, 0); // ����������ת�س�ʼλ��
        Quaternion rightEndRotation = rightDoor.rotation * Quaternion.Euler(0, -90, 0); // ����������ת�س�ʼλ��

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

    void UpdateLightMaterials(bool activated)
    {
        if (activated)
        {
            greenLightRenderer.material = greenLightActiveMaterial; // ʹ�ü�����̵Ʋ���
            redLightRenderer.material = redLightInactiveMaterial; // ʹ��δ����ĺ�Ʋ���
        }
        else
        {
            greenLightRenderer.material = greenLightInactiveMaterial; // ʹ��δ������̵Ʋ���
            redLightRenderer.material = redLightActiveMaterial; // ʹ�ü���ĺ�Ʋ���
        }
    }
}