using UnityEngine;

namespace Invector
{
    public class HealItemController : MonoBehaviour
    {
        public Transform playerTransform; // 玩家的Transform
        public float attractionForce = 10f; // 吸引力大小
        public float damping = 0.1f; // 阻尼系数

        private Rigidbody rb; // HealItem的Rigidbody

        private void Start()
        {
            // 确保获取的是父对象的Rigidbody
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
