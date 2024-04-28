using UnityEngine;

public class SlotTriggerHandler : MonoBehaviour
{
    public GameObject onSlotObject;

    public bool activated;
    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.gameObject.GetComponent<MaterialManager>().isKey)
        {
            Destroy(other.gameObject);
            onSlotObject.SetActive(true);
            activated = true;
        }
    }
}