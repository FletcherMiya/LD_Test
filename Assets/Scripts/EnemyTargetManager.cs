using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{
    public bool isdead = false;
    public GameObject healItemPrefab;
    public float forceMagnitude;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDead()
    {
        isdead = true;
        SpawnHealItems();
        EnemySpawner es = GetComponentInParent<EnemySpawner>();
        if (es != null)
        {
            es.deathCountUp();
        }
        Destroy(gameObject);
    }

    void SpawnHealItems()
    {
        int count = Random.Range(1, 4);

        for (int i = 0; i < count; i++)
        {
            GameObject healItem = Instantiate(healItemPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = healItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                rb.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
