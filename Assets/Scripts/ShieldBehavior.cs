using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    public GameObject shatterPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Thrown"))
        {
            MaterialManager mm = collision.gameObject.GetComponent<MaterialManager>();
            if (mm.isKey == false & mm.isPillar == false)
            {
                Instantiate(shatterPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}
