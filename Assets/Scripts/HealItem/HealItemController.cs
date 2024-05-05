using UnityEngine;

namespace Invector
{
    public class HealItemController : MonoBehaviour
    {
        public Transform playerTransform; // 玩家的Transform
        public float attractionForce = 10f; // 吸引力大小
        public float damping = 0.1f; // 阻尼系数

        public ParticleSystem beamParticle; // Beam 粒子系统
        public TrailRenderer trailRenderer; // Trail Renderer
        public float speedThreshold = 0.1f; // 速度阈值

        private Rigidbody rb; // HealItem的Rigidbody

        public float groundCheckDistance = 1f; // 地面检测距离

        private void Start()
        {
            // 确保获取的是父对象的Rigidbody
            rb = GetComponentInParent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody component is missing on this GameObject.");
            }

            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("HealTarget");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            // 确保粒子系统已被正确链接
            if (beamParticle == null || trailRenderer == null)
            {
                Debug.LogError("Particle systems are not properly assigned.");
            }
        }

        void Update()
        {
            if (rb != null && beamParticle != null && trailRenderer != null)
            {
                CheckSpeedAndControlEffects();
            }
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("HealTarget");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
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

        void CheckSpeedAndControlEffects()
        {
            float currentSpeed = rb.velocity.magnitude; // 获取当前速度
            bool isOnGround = CheckIfOnGround(); // 检查是否接触地面

            if (currentSpeed <= speedThreshold && isOnGround)
            {
                // 速度几乎为0，播放 Beam 粒子系统
                if (!beamParticle.isPlaying)
                {
                    beamParticle.gameObject.SetActive(true);
                    beamParticle.Play();
                }
                if (trailRenderer.enabled)
                {
                    trailRenderer.enabled = false; // 禁用 Trail Renderer
                }
            }
            else
            {
                // 速度大于阈值，启用 Trail Renderer
                if (!trailRenderer.enabled)
                {
                    trailRenderer.enabled = true;
                }
                if (beamParticle.isPlaying)
                {
                    beamParticle.gameObject.SetActive(false);
                    beamParticle.Stop();
                }
            }
        }

        bool CheckIfOnGround()
        {
            RaycastHit hit;
            // 向下投射一个射线
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, groundCheckDistance))
            {
                return true; // 接触地面
            }
            return false; // 不接触地面
        }
    }
}
