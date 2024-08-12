using UnityEngine;

public class AlignGravityWithTransform : MonoBehaviour
{
    void Update()
    {
        // 将物理引擎的重力方向设置为当前物体Transform的下方向
        Physics.gravity = -transform.up * Mathf.Abs(Physics.gravity.magnitude);
    }
}