using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public Material normalMaterial;
    public Material highlightMaterial;

    public bool isKey;

    private Renderer objRenderer;

    public GameObject trigger;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;
        trigger.SetActive(false);
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
    }
}