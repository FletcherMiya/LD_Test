using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    public GameObject shatterPrefab;
    private GameObject[] siblingObjects;

    private void Awake()
    {
        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            siblingObjects = new GameObject[parentTransform.childCount];
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                siblingObjects[i] = parentTransform.GetChild(i).gameObject;
            }
        }
        else
        {
            siblingObjects = new GameObject[1];
            siblingObjects[0] = gameObject;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Thrown"))
        {
            MaterialManager mm = collision.gameObject.GetComponent<MaterialManager>();
            if (mm.isKey == false && mm.isPillar == false)
            {
                Instantiate(shatterPrefab, transform.position, transform.rotation);

                foreach (GameObject obj in siblingObjects)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }
        }
    }
}