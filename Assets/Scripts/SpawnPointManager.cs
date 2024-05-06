using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class SpawnPointManager : MonoBehaviour
    {
        public GameObject slot;
        public GameObject gameManager;
        public GameObject level;
        public GameObject[] stufftoDeactivate;
        public GameObject[] stufftoReactivate;
        private bool stuffActivated = false;
        public GameObject rightDoorSlot;
        public GameObject shatterEffect;
        public GameObject levelTrigger;

        private void Update()
        {
            if (rightDoorSlot != null)
            {
                if (slot.GetComponent<SlotTriggerHandler>().activated)
                {
                    if (rightDoorSlot.GetComponent<SlotTriggerHandler>().activated && levelTrigger.GetComponent<LevelStuffTrigger>().stuffActivated)
                    {
                        slot.GetComponent<SlotTriggerHandler>().activated = false;
                        slot.SetActive(false);
                        Instantiate(shatterEffect, slot.transform.Find("OnSlotObject").transform.position, slot.transform.Find("OnSlotObject").transform.rotation);
                    }
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && slot.GetComponent<SlotTriggerHandler>().activated)
            {
                gameManager.GetComponent<vGameController>().spawnPoint = this.gameObject.GetComponent<Transform>();
                if (level != null)
                {
                    level.SetActive(true);
                    //stuffActivated = true;
                }
                if (stufftoDeactivate != null)
                {
                    foreach (GameObject go in stufftoDeactivate)
                    {
                        go.SetActive(false);
                    }
                }
                if (stufftoReactivate != null)
                {
                    foreach (GameObject go in stufftoReactivate)
                    {
                        go.SetActive(true);
                    }
                }
            }
        }
    }

}