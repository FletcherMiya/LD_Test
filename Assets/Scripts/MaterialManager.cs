using UnityEngine;
using System.Collections;

public class MaterialManager : MonoBehaviour
{
    public Material normalMaterial;
    public Material highlightMaterial;

    public bool isKey;
    public bool isPillar;

    private Renderer objRenderer;

    public GameObject trigger;

    public bool thrown;
    public bool towardsSlot;
    private int collisionCount;
    public bool shattered;

    public GameObject levelManager;
    private LevelManager currentLevel;

    public GameObject pillarPrefab;
    private bool hasRisen;


    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;
        trigger.SetActive(false);
        thrown = false;
        shattered = false;
        hasRisen = false;
        if (levelManager != null)
        {
            currentLevel = levelManager.GetComponent<LevelManager>();
        }
        if (isKey && currentLevel != null)
        {
            currentLevel.increaseCount();
            Debug.Log("CountIncreased");
        }
    }

    public void ApplyHighlight()
    {
        if (highlightMaterial != null)
        {
            objRenderer.material = highlightMaterial;
        }
    }

    public void RemoveHighlight()
    {
        if (normalMaterial != null)
        {
            objRenderer.material = normalMaterial;
        }
    }

    public void activateTrigger()
    {
        trigger.SetActive(true);
        thrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrown)
        {
            collisionCount++;
        }
        if (thrown && !towardsSlot)
        {
            if (isPillar)
            {
                if (collision.gameObject.CompareTag("Pit") && !hasRisen)
                {
                    Vector3 spawnPosition = collision.contacts[0].point;
                    GameObject spawnedObject = Instantiate(pillarPrefab, spawnPosition, Quaternion.identity);

                    PillarManager pm = spawnedObject.GetComponent<PillarManager>();
                    if (pm != null)
                    {
                        pm.StartRising();
                    }
                    hasRisen = true;
                }
            }
            if (!shattered)
            {
                trigger.GetComponent<DamageSphereManager>().shatter();
                shattered = true;
                Debug.Log("shatter by box not slot");
                //Debug.Log(collision.gameObject);
            }
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            Destroy(gameObject, 0.05f);
        }
        else if (thrown && towardsSlot)
        {
            if (collisionCount > 1)
            {
                trigger.GetComponent<DamageSphereManager>().shatter();
                Debug.Log("shatter by box slot");
                Debug.Log(collision.gameObject);
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                Destroy(gameObject, 0.05f);
            }
        }
    }

    private void OnDestroy()
    {
        if (isKey && levelManager != null)
        {
            currentLevel.decreaseCount();
            Debug.Log("Count Decreased");
        }
    }
}