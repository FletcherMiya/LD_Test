using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int trackedKeyCount;
    public int keyLimit;
    public GameObject slot;
    private bool slotCounted;
    public GameObject shatterEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SlotTriggerHandler currentSlot = slot.GetComponent<SlotTriggerHandler>();
        if (currentSlot.activated && !slotCounted)
        {
            trackedKeyCount++;
            slotCounted = true;
            Debug.Log("Slot Counted");
        }
        if (!currentSlot.activated && slotCounted)
        {
            trackedKeyCount--;
            slotCounted = false;
            Debug.Log("Slot Decounted");
        }
        if (trackedKeyCount > keyLimit)
        {
            slot.GetComponent<SlotTriggerHandler>().activated = false;
            Debug.Log("Slot deactivated");
            Instantiate(shatterEffect, slot.transform.Find("OnSlotObject").transform.position, slot.transform.Find("OnSlotObject").transform.rotation);
        }
    }

    public void increaseCount()
    {
        trackedKeyCount++;
    }

    public void decreaseCount()
    {
        trackedKeyCount--;
    }
}
