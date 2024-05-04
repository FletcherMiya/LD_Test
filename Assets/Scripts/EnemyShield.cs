using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    public GameObject shatterPrefab;
    public GameObject targetEnemy;
    // Start is called before the first frame update
    private void Awake()
    {
        Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), targetEnemy.GetComponent<Collider>());

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Thrown"))
        {
            Debug.Log(collision.gameObject);
            MaterialManager mm = collision.gameObject.GetComponent<MaterialManager>();
            if (mm.isKey == false)
            {
                Instantiate(shatterPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}