using UnityEngine;

namespace Invector
{
    public class HealItemController : MonoBehaviour
    {
        public Transform playerTransform; // ��ҵ�Transform
        public float attractionForce = 10f; // ��������С
        public float damping = 0.1f; // ����ϵ��

        private Rigidbody rb; // HealItem��Rigidbody

        private void Start()
        {
            // ȷ����ȡ���Ǹ������Rigidbody
            rb = GetComponentInParent<Rigidbody>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var healthController = other.GetComponent<vHealthController>();
                if (healthController.currentHealth < healthController.maxHealth)
                {
                    MoveHealItemTowardsPlayer();
                }
            }
        }

        private void MoveHealItemTowardsPlayer()
        {
            if (playerTransform != null && rb != null)
            {
                Vector3 directionToPlayer = playerTransform.position - transform.parent.position;
                float distanceToPlayer = directionToPlayer.magnitude;
                float forceMagnitude = Mathf.Clamp(distanceToPlayer * attractionForce, 0, 500f);
                rb.AddForce(directionToPlayer.normalized * forceMagnitude);
                rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);
            }
        }
    }
}
