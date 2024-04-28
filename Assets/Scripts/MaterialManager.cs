using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public Material normalMaterial;  // ��ͨ״̬�Ĳ���
    public Material highlightMaterial;  // ����״̬�Ĳ���

    public bool isKey;

    private Renderer objRenderer;

    public GameObject trigger;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        objRenderer.material = normalMaterial;  // ��ʼʱʹ����ͨ����
        trigger.SetActive(false);
    }

    public void ApplyHighlight()
    {
        if (highlightMaterial != null)
        {
            objRenderer.material = highlightMaterial;  // Ӧ�ø�������
        }
    }

    public void RemoveHighlight()
    {
        if (normalMaterial != null)
        {
            objRenderer.material = normalMaterial;  // �ָ���ͨ����
        }
    }

    public void activateTrigger()
    {
        trigger.SetActive(true);
    }
}