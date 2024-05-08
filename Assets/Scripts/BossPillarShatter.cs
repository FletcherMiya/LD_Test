using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPillarShatter : MonoBehaviour
{
    public int maxHitCount;
    [SerializeField] private int currentHitCount;
    private GameObject lastHitObject;
    public GameObject shatterEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (currentHitCount >= maxHitCount)
        {
            Instantiate(shatterEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Throwable") | collision.gameObject.CompareTag("Thrown"))
        {
            if (lastHitObject != null)
            {
                if (collision.gameObject != lastHitObject)
                {
                    currentHitCount++;
                    lastHitObject = collision.gameObject;
                }
            }
            else
            {
                currentHitCount++;
                lastHitObject = collision.gameObject;
            }
        }
    }
}
