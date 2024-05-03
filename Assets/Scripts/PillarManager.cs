using UnityEngine;
using System.Collections;

public class PillarManager : MonoBehaviour
{
    public Material normalMaterial;
    public Material highlightMaterial;

    public bool isPillar;

    private Renderer objRenderer;

    public bool thrown;


    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;
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

    private void OnCollisionEnter(Collision collision)
    {

    }
}