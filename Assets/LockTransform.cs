using UnityEngine;

public class LockTransform : MonoBehaviour
{
    private Vector3 initialPosition;   // 初始位置
    private Quaternion initialRotation; // 初始旋转
    private Vector3 initialScale;     // 初始缩放

    void Start()
    {
        // 记录物体的初始 Transform 属性
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // 锁定物体的 Transform 属性
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
    }
}
