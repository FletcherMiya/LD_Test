using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class SpawnPointManager : MonoBehaviour
    {
        public GameObject slot;
        public GameObject gameManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && slot.GetComponent<SlotTriggerHandler>().activated)
            {
                gameManager.GetComponent<vGameController>().spawnPoint = this.gameObject.GetComponent<Transform>();
            }
        }
    }

}