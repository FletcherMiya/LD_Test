using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.UI;
using Invector.vCharacterController;

public class TelekinesisAbility : MonoBehaviour
{
    public Camera playerCamera;
    public Transform player;
    public GameObject playerandCamera;
    public GameObject sceneHolder;
    private Rigidbody playerRigidbody;
    public Transform holdPoint;
    public float maxDistance;
    public float sphereRadius;

    public float throwForce;
    public float destroyTime;
    public float attractionForce;
    public float damping;
    public float riseHeight;
    public float initialUpwardForce;
    public float horizontalForce;
    public float rotationForce = 10f;
    public float delayBetweenStages = 0.5f;


    private int stage = 0;
    private float stageChangeTime;


    private GameObject selectedObject;
    private GameObject originalObject;
    private Vector3 risePoint;


    private GameObject lastHighlighted = null;


    private bool isHolding = false;
    private bool hasAppliedHorizontalForce = false;

    public RectTransform slotMarker;

    public CinemachineVirtualCamera cm;
    private CinemachineBasicMultiChannelPerlin perlinNoise;

    public NoiseSettings holdingNoise;
    public NoiseSettings launchNoise;
    public NoiseSettings receiveNoise;
    private bool cameraShaked = false;

    public float energy;
    public float maxEnergy;
    public float energyRecoveryRate;
    public float energyCost;
    public float timeSinceLastAction = 0; 
    private bool startedRecovery = false;
    public float startRecoveryDelay;

    public Slider energySlider;
    public float staminaSliderValueSmooth = 10;

    public float rotationDuration = 1f;
    public bool reversed = false;
    public float gravityAdjustmentDelay = 1f;
    public float playerMoveSpeedDuringDelay = 2f;

    private void Awake()
    {
        perlinNoise = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlinNoise.m_NoiseProfile = null;
        playerRigidbody = player.GetComponent<Rigidbody>();
        reversed = false;
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isHolding)
            {
                if (energy >= energyCost)
                {
                    TryPickAndCloneObject();
                }
            }
            else
            {
                GameObject target = FindClosestObjectByTags(new string[] { "Slot", "EnemyTarget" });
                if (target != null)
                {
                    if (target.CompareTag("Slot"))
                    {
                        if (!target.GetComponent<SlotTriggerHandler>().activated && selectedObject.GetComponent<MaterialManager>().isKey)
                        {
                            ThrowObjectTowardsSlot(selectedObject, target);
                        }
                        else
                        {
                            ThrowObject();
                        }
                    }
                    else
                    {
                        if (!target.GetComponent<EnemyTargetManager>().isdead)
                        {
                            ThrowObjectTowardsSlot(selectedObject, target);
                        }
                        else
                        {
                            ThrowObject();
                        }
                    }
                }
                else
                {
                    ThrowObject();
                }
            }
        }

        if (isHolding)
        {
            if (selectedObject.GetComponent<MaterialManager>().isKey)
            {
                GameObject closestObject = FindClosestObjectByTags(new string[] { "Slot", "EnemyTarget" });
                if (closestObject != null)
                {
                    if (closestObject.CompareTag("Slot"))
                    {
                        if (!closestObject.GetComponent<SlotTriggerHandler>().activated)
                        {
                            ShowSlotMarker(closestObject);
                        }
                        else
                        {
                            slotMarker.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (!closestObject.GetComponent<EnemyTargetManager>().isdead)
                        {
                            ShowSlotMarker(closestObject);
                        }
                        else
                        {
                            slotMarker.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    slotMarker.gameObject.SetActive(false);
                }
            }
            else
            {
                GameObject closestObject = FindClosestObjectByTags(new string[] {"EnemyTarget"});
                if (closestObject != null && !closestObject.GetComponent<EnemyTargetManager>().isdead)
                {
                    ShowSlotMarker(closestObject);
                }
                else
                {
                    slotMarker.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            slotMarker.gameObject.SetActive(false);
        }


        if (!isHolding)
        {
            HighlightObjectUnderCrosshair();
        }
        else if (lastHighlighted != null)
        {
            lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            lastHighlighted = null;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(AdjustGravityAndRotatePlayer());
        }

    }

    private void FixedUpdate()
    {
        if (isHolding && selectedObject != null)
        {
            MoveObjectToHoldPoint();
        }
        if (!isHolding && startedRecovery && energy < maxEnergy)
        {
            timeSinceLastAction += Time.fixedDeltaTime;
            if (timeSinceLastAction >= startRecoveryDelay)
            {
                energy += energyRecoveryRate * Time.fixedDeltaTime;
            }
        }
        UpdateEnergyUI();
    }

    IEnumerator AdjustGravityAndRotatePlayer()
    {
        GameObject gravityPlatform = null;

        if (!reversed)
        {
            gravityPlatform = FindClosestObjectByTags(new string[] { "GravityPlatform" });

            if (gravityPlatform == null)
            {
                yield break;
            }
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        Vector3 targetDownDirection = reversed ? Vector3.down : -gravityPlatform.transform.up;
        Vector3 currentDownDirection = -player.transform.up;

        Physics.gravity = targetDownDirection * Mathf.Abs(Physics.gravity.magnitude);

        float moveSpeed = playerMoveSpeedDuringDelay;
        float elapsedTime = 0f;

        while (elapsedTime < gravityAdjustmentDelay)
        {
            player.gameObject.GetComponent<vThirdPersonMotor>().snapFactor = 0;
            elapsedTime += Time.deltaTime;

            player.transform.Translate(player.transform.up * moveSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / rotationDuration);

            Vector3 newDownDirection = Vector3.Slerp(currentDownDirection, targetDownDirection, t);

            Quaternion targetRotation = Quaternion.FromToRotation(-player.transform.up, newDownDirection) * player.transform.rotation;

            player.transform.rotation = targetRotation;

            yield return null;
        }

        player.transform.rotation = Quaternion.FromToRotation(-player.transform.up, targetDownDirection) * player.transform.rotation;

        reversed = !reversed;
        player.gameObject.GetComponent<vThirdPersonMotor>().snapFactor = 15;
    }


    void UpdateEnergyUI()
    {
        if (energySlider != null)
        {
            energySlider.value = Mathf.Lerp(energySlider.value, energy, staminaSliderValueSmooth * Time.fixedDeltaTime);
        }
    }

    void TryPickAndCloneObject()
    {
        originalObject = FindClosestObjectByTags(new string[] { "Throwable" });

        if (originalObject != null)
        {
            energy -= energyCost;
            startedRecovery = false;
            cm.Priority = 11;
            selectedObject = Instantiate(originalObject, originalObject.transform.position, originalObject.transform.rotation);
            selectedObject.GetComponent<MaterialManager>().ToggleColliderTemporarily();
            //Physics.IgnoreCollision(selectedObject.GetComponent<Collider>(), originalObject.GetComponent<Collider>(), true);
            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            perlinNoise.m_NoiseProfile = holdingNoise;
            perlinNoise.m_AmplitudeGain = 0.5f;
            isHolding = true;
            stage = 0;
            risePoint = new Vector3(originalObject.transform.position.x, originalObject.transform.position.y + riseHeight, originalObject.transform.position.z);
        }
    }

    void MoveObjectToHoldPoint()
    {
        if (selectedObject == null) return;
        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        selectedObject.layer = 7;
        switch (stage)
        {
            case 0:
                stage = 1;
                stageChangeTime = Time.time;
                hasAppliedHorizontalForce = false;
                break;
            case 1:
                Vector3 directionToRisePoint = risePoint - selectedObject.transform.position;
                float distanceToRisePoint = directionToRisePoint.magnitude;
                if (!hasAppliedHorizontalForce)
                {
                    Vector3 horizontalDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                    rb.AddForce(horizontalDir * horizontalForce, ForceMode.Impulse);
                    Vector3 torque = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    rb.AddTorque(torque * rotationForce, ForceMode.Impulse);
                    hasAppliedHorizontalForce = true;
                }
                if (distanceToRisePoint > 0.1f)
                {
                    float riseForceMagnitude = Mathf.Clamp(distanceToRisePoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToRisePoint.normalized * riseForceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);
                }
                else
                {
                    stage = 2;
                    stageChangeTime = Time.time;
                    rb.velocity = Vector3.zero;
                    //Physics.IgnoreCollision(selectedObject.GetComponent<Collider>(), originalObject.GetComponent<Collider>(), false);
                }
                break;
            case 2:
                Vector3 directionToHoldPoint = holdPoint.position - selectedObject.transform.position;
                float distanceToHoldPoint = directionToHoldPoint.magnitude;
                if (Time.time - stageChangeTime >= delayBetweenStages)
                {
                    float forceMagnitude = Mathf.Clamp(distanceToHoldPoint * attractionForce, 0, 500f);
                    rb.AddForce(directionToHoldPoint.normalized * forceMagnitude);
                    rb.velocity *= 1 - Mathf.Clamp01(damping * Time.deltaTime);

                    if (!cameraShaked && distanceToHoldPoint < 1f)
                    {
                        perlinNoise.m_AmplitudeGain = 5f;
                        cameraShaked = true;
                    }
                    if (distanceToHoldPoint < 0.1f)
                    {
                        perlinNoise.m_AmplitudeGain = 1f;
                    }
                }
                break;
        }
    }

    void ThrowObject()
    {
        if (selectedObject != null)
        {
            Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
            MaterialManager mm = selectedObject.GetComponent<MaterialManager>();
            if (rb != null)
            {
                int layerMask = 1 << LayerMask.NameToLayer("Triggers");
                layerMask = ~layerMask;

                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
                Ray ray = playerCamera.ScreenPointToRay(screenCenter);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawLine(ray.origin, hit.point, Color.red, 2.0f);

                    Vector3 targetDirection = hit.point - selectedObject.transform.position;
                    mm.activateTrigger();
                    mm.isThrownByPlayer = true;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.AddForce(targetDirection.normalized * throwForce, ForceMode.Impulse);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.blue, 2.0f);
                    rb.useGravity = true;
                    rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
                }

                StartCoroutine(CameraShakeAndReset());
                Destroy(selectedObject, destroyTime);
                selectedObject.tag = "Thrown";
                startedRecovery = true;
                timeSinceLastAction = 0;
                selectedObject = null;
                originalObject = null;
                isHolding = false;
                cameraShaked = false;
            }
        }
    }


    void ThrowObjectTowardsSlot(GameObject obj, GameObject slot)
    {
        if (obj != null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            MaterialManager mm = selectedObject.GetComponent<MaterialManager>();
            if (rb != null)
            {
                mm.activateTrigger();
                mm.isThrownByPlayer = true;
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.useGravity = true;
                Vector3 direction = (slot.transform.position - obj.transform.position).normalized;
                rb.AddForce(direction * throwForce, ForceMode.Impulse);
                if (slot.CompareTag("Slot"))
                {
                    mm.towardsSlot = true;
                }
            }
            StartCoroutine(CameraShakeAndReset());
            Destroy(obj, destroyTime);
            selectedObject.tag = "Thrown";
            startedRecovery = true;
            timeSinceLastAction = 0;
            selectedObject = null;
            originalObject = null;
            isHolding = false;
            cameraShaked = false;
        }
    }

    void HighlightObjectUnderCrosshair()
    {
        GameObject closestObject = FindClosestObjectByTags(new string[] { "Throwable" });

        if (closestObject != null && lastHighlighted != closestObject)
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            }
            closestObject.GetComponent<MaterialManager>().ApplyHighlight();
            lastHighlighted = closestObject;
        }
        else if (closestObject == null && lastHighlighted != null)
        {
            lastHighlighted.GetComponent<MaterialManager>().RemoveHighlight();
            lastHighlighted = null;
        }
    }

    GameObject FindClosestObjectByTags(string[] tags)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = playerCamera.ScreenPointToRay(screenCenter);
        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, maxDistance);

        GameObject closestObject = null;
        float closestDistance = float.MaxValue;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        foreach (RaycastHit hit in hits)
        {
            if (!GeometryUtility.TestPlanesAABB(planes, hit.collider.bounds))
                continue;

            foreach (string tag in tags)
            {
                if (hit.collider.CompareTag(tag))
                {
                    float distance = hit.distance;
                    if (distance < closestDistance)
                    {
                        if (tag == "Slot")
                        {
                            if (hit.collider.GetComponent<SlotTriggerHandler>().activated)
                            {
                                continue;
                            }
                        }

                        closestDistance = distance;
                        closestObject = hit.collider.gameObject;
                    }
                }
            }
        }

        return closestObject;
    }
    
    void ShowSlotMarker(GameObject slot)
    {
        Vector3 viewportPosition = playerCamera.WorldToViewportPoint(slot.transform.position);
        if (viewportPosition.z > 0)
        {
            Vector2 targetPosition = new Vector2((viewportPosition.x - 0.5f) * slotMarker.parent.GetComponent<RectTransform>().sizeDelta.x, (viewportPosition.y - 0.5f) * slotMarker.parent.GetComponent<RectTransform>().sizeDelta.y);
            if (!slotMarker.gameObject.activeSelf)
            {
                slotMarker.anchoredPosition = targetPosition;
            }

            slotMarker.gameObject.SetActive(true);
            slotMarker.anchoredPosition = Vector2.Lerp(slotMarker.anchoredPosition, targetPosition, Time.deltaTime * 120);
        }
        else
        {
            slotMarker.gameObject.SetActive(false);
        }
    }

    IEnumerator CameraShakeAndReset()
    {
        if (perlinNoise != null)
        {
            perlinNoise.m_NoiseProfile = launchNoise;
            perlinNoise.m_AmplitudeGain = 10;

            float duration = 0.3f;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                perlinNoise.m_AmplitudeGain = Mathf.Lerp(10, 0, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            perlinNoise.m_AmplitudeGain = 0;

            perlinNoise.m_NoiseProfile = null;
            cm.Priority = 1;
        }
    }
}