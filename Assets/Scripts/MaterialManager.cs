using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public Material normalMaterial;  // 普通状态的材质
    public Material highlightMaterial;  // 高亮状态的材质

    public bool isKey;

    private Renderer objRenderer;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;  // 初始时使用普通材质
    }

    public void ApplyHighlight()
    {
        if (highlightMaterial != null)
        {
            objRenderer.material = highlightMaterial;  // 应用高亮材质
        }
    }

    public void RemoveHighlight()
    {
        if (normalMaterial != null)
        {
            objRenderer.material = normalMaterial;  // 恢复普通材质
        }
    }
}