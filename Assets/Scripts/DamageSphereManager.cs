using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSphereManager : MonoBehaviour
{
    public GameObject shatterEffect;
    private Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        this.GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MaterialManager mm = GetComponentInParent<MaterialManager>();
            if (!mm.shattered)
            {
                shatter();
                Debug.Log("shatter by sphere");
                mm.shattered = true;
            }
            StartCoroutine(disableSphere());
        }
    }
    IEnumerator disableSphere()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<Collider>().enabled = false;
    }

    public void shatter()
    {
        GameObject shatter = Instantiate(shatterEffect, transform.position, transform.rotation);

        Rigidbody effectRigidbody = shatter.GetComponent<Rigidbody>();
        effectRigidbody.velocity = rb.velocity;
    }
}
