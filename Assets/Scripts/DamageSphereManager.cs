using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSphereManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        this.GetComponent<SphereCollider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(disableSphere());
        }
    }
    IEnumerator disableSphere()
    {
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<SphereCollider>().enabled = false;
    }
}
