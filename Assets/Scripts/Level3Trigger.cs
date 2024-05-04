using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Trigger : MonoBehaviour
{
    public GameObject toFall;
    private bool hasFallen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasFallen)
        {
            toFall.GetComponent<Rigidbody>().isKinematic = false;
            toFall.GetComponent<MaterialManager>().thrown = true;
            Debug.Log("Scene Pillar Dropped");
            hasFallen = true;
        }
    }
}
