using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStuffTrigger : MonoBehaviour
{
    public GameObject stuff;
    public GameObject slot;
    private bool stuffActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (slot != null)
            {
                if (slot.GetComponent<SlotTriggerHandler>().activated && !stuffActivated)
                {
                    stuff.SetActive(true);
                    stuffActivated = true;
                }
            }
            else
            {
                if ((!stuffActivated))
                {
                    stuff.SetActive(true);
                    stuffActivated = true;
                }
            }
        }
    }
}
