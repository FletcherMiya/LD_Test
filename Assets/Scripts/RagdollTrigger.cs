using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Invector.vCharacterController.AI
{
    public class RagdollTrigger : MonoBehaviour
    {
        public float ragdollDuration;
        public vDamage damage;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                //other.GetComponent<vRagdoll>().ActivateRagdoll();
                //other.GetComponent<vHealthController>().TakeDamage(damage);
            }
        }
    }
}

