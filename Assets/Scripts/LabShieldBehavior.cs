using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabShieldBehavior : MonoBehaviour
{
    public GameObject key;

    private void OnDestroy()
    {
        key.tag = ("Throwable");
    }
}
