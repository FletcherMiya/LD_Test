using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int keyLimit;
    public GameObject slot;
    public GameObject shatterEffect;
    public GameObject nextSlot;
    private bool shattered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextSlot.GetComponent<SlotTriggerHandler>().activated)
        {
            slot.GetComponent<SlotTriggerHandler>().activated = false;
            Debug.Log("Slot deactivated");
            if (!shattered)
            {
                Instantiate(shatterEffect, slot.transform.Find("OnSlotObject").transform.position, slot.transform.Find("OnSlotObject").transform.rotation);
                shattered = true;
            }
        }
    }

    public void increaseCount()
    {
    }

    public void decreaseCount()
    {
    }
}
