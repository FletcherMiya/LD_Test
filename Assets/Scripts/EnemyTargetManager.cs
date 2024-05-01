using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{
    public bool isdead = false;
    public GameObject healItemPrefab;
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
    }

    void SpawnHealItems()
    {
        int count = Random.Range(1, 4); // 生成1到3个HealItem

        for (int i = 0; i < count; i++)
        {
            // 在敌人的位置生成HealItem
            GameObject healItem = Instantiate(healItemPrefab, transform.position, Quaternion.identity);
        }
    }
}
