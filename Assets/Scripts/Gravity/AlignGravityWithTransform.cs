using UnityEngine;

public class AlignGravityWithTransform : MonoBehaviour
{
    void Update()
    {
        // �����������������������Ϊ��ǰ����Transform���·���
        Physics.gravity = -transform.up * Mathf.Abs(Physics.gravity.magnitude);
    }
}