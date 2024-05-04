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
        private bool stuffActivated = false;
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && slot.GetComponent<SlotTriggerHandler>().activated)
            {
                gameManager.GetComponent<vGameController>().spawnPoint = this.gameObject.GetComponent<Transform>();
                if (level != null && !stuffActivated)
                {
                    level.SetActive(true);
                    stuffActivated = true;
                }
            }
        }
    }

}