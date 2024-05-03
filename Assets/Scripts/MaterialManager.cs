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
    public float riseHeight;
    public float riseSpeed;


    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;
        trigger.SetActive(false);
        thrown = false;
        shattered = false;
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
            if (!shattered)
            {
                trigger.GetComponent<DamageSphereManager>().shatter();
                shattered = true;
                Debug.Log("shatter by box not slot");
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
        else if (thrown && isPillar)
        {
            if (collision.gameObject.CompareTag("Pit"))
            {
                trigger.GetComponent<DamageSphereManager>().shatter();
                Vector3 spawnPosition = collision.contacts[0].point;
                GameObject pillar = Instantiate(pillarPrefab, spawnPosition, Quaternion.identity);
                StartCoroutine(Rise(pillar));
            }
        }
    }

    private void OnDestroy()
    {
        if (isKey)
        {
            currentLevel.decreaseCount();
            Debug.Log("Count Decreased");
        }
    }

    private IEnumerator Rise(GameObject obj)
    {
        float startTime = Time.time;
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseHeight;

        while (Time.time - startTime < riseHeight / riseSpeed)
        {
            float fracComplete = (Time.time - startTime) * riseSpeed / riseHeight;
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, fracComplete);
            yield return null;
        }

        obj.transform.position = endPosition;  // 确保预制体准确到达最终位置
    }
}