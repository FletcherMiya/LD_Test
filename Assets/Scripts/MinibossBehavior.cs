using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossBehavior : MonoBehaviour
{
    public GameObject key;
    public GameObject shields;
    public GameObject target;

    public void minibossDeath()
    {
        disableShields();
        dropKey();

    }

    private void disableShields()
    {
        shields.SetActive(false);
    }

    private void dropKey()
    {
        GameObject droppedKey = Instantiate(key, target.transform.position, target.transform.rotation);
        droppedKey.GetComponent<Rigidbody>().isKinematic = false;
    }
}
