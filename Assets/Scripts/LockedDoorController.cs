using UnityEngine;
using System.Collections;

public class LockedDoorController : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public GameObject slot;
    public GameObject greenLight;
    public GameObject redLight;
    public float rotationDuration = 2.0f;
    private bool isOpened = false;
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
                isOpened = true;
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
        Quaternion leftEndRotation = leftStartRotation * Quaternion.Euler(0, -90, 0);
        Quaternion rightEndRotation = rightStartRotation * Quaternion.Euler(0, 90, 0);

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
        Quaternion leftEndRotation = leftDoor.rotation * Quaternion.Euler(0, 90, 0);
        Quaternion rightEndRotation = rightDoor.rotation * Quaternion.Euler(0, -90, 0);

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
            greenLightRenderer.material = greenLightActiveMaterial;
            redLightRenderer.material = redLightInactiveMaterial;
        }
        else
        {
            greenLightRenderer.material = greenLightInactiveMaterial;
            redLightRenderer.material = redLightActiveMaterial;
        }
    }
}