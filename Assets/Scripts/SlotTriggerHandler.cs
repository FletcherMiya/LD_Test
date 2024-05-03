using UnityEngine;

public class SlotTriggerHandler : MonoBehaviour
{
    public GameObject onSlotObject;

    public bool activated;

    private void Update()
    {
        if (!activated)
        {
            onSlotObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Thrown") && !activated && other.gameObject.GetComponent<MaterialManager>().isKey)
        {
            Destroy(other.gameObject);
            onSlotObject.SetActive(true);
            activated = true;
        }
    }
}