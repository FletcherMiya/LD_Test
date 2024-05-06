using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level25Trigger : MonoBehaviour
{
    public GameObject[] go;
    public GameObject key;
    public GameObject slot;
    public GameObject shatterEffect;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject box in go)
            {
                if (box.activeSelf != false)
                {
                    box.SetActive(false);
                    Instantiate(shatterEffect, box.transform.position, box.transform.rotation);
                }
            }
            if (!slot.GetComponent<SlotTriggerHandler>().activated)
            {
                key.SetActive(false);
            }
        }
    }
}
