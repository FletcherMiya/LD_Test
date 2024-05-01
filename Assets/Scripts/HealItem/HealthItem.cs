using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class HealthItem : MonoBehaviour
    {
        public GameObject parent;
        public float healAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var healthController = other.GetComponent<vHealthController>();
                if (healthController != null)
                {
                    if(healthController.currentHealth < healthController.maxHealth)
                    {
                        healthController.AddHealth((int)healAmount);
                        Destroy(parent);
                        Debug.Log("Healed");
                    }
                }
            }
        }
    }
}
