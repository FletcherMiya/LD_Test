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