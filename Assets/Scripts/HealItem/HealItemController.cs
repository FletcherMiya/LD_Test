using UnityEngine;

namespace Invector
{
    public class HealItemController : MonoBehaviour
    {
        public Transform playerTransform;
        public float attractionForce = 10f;
        public float damping = 0.1f;

        public ParticleSystem beamParticle;
        public TrailRenderer trailRenderer;
        public float speedThreshold = 0.1f;

        private Rigidbody rb;

        public float groundCheckDistance = 1f;

        private void Start()
        {
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
            float currentSpeed = rb.velocity.magnitude;
            bool isOnGround = CheckIfOnGround();

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
                    trailRenderer.enabled = false;
                }
            }
            else
            {
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
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, groundCheckDistance))
            {
                return true;
            }
            return false;
        }
    }
}
