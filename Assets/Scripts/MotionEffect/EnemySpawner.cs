using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] Enemies;
    public GameObject[] connectingObjects;
   public int deathCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject child in Enemies)
        {
            if (child != null)
            {
                child.SetActive(false);
            }
        }

        foreach (GameObject level in connectingObjects)
        {
            level.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (deathCount == Enemies.Length)
        {
            foreach (GameObject level in connectingObjects)
            {
                level.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateEnemies();
        }
    }

    private void ActivateEnemies()
    {
        foreach (GameObject child in Enemies)
        {
            if (child != null)
            {
                child.SetActive(true);
            }
        }
    }

    public void deathCountUp()
    {
        deathCount++;
    }
}
