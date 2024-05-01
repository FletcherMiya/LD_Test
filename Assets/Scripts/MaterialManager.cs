using UnityEngine;
using System.Collections;

public class MaterialManager : MonoBehaviour
{
    public Material normalMaterial;
    public Material highlightMaterial;

    public bool isKey;

    private Renderer objRenderer;

    public GameObject trigger;

    private bool thrown;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;
        trigger.SetActive(false);
        thrown = false;
    }

    public void ApplyHighlight()
    {
        if (highlightMaterial != null)
        {
            objRenderer.material = highlightMaterial;
        }
    }

    public void RemoveHighlight()
    {
        if (normalMaterial != null)
        {
            objRenderer.material = normalMaterial;
        }
    }

    public void activateTrigger()
    {
        trigger.SetActive(true);
        thrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrown && collision.gameObject.CompareTag("Enemy"))
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            Destroy(gameObject, 0.05f);
        }
    }
}