using UnityEngine;

public class LockTransform : MonoBehaviour
{
    private Vector3 initialPosition;   // ��ʼλ��
    private Quaternion initialRotation; // ��ʼ��ת
    private Vector3 initialScale;     // ��ʼ����

    void OnEnable()
    {
        // ��¼����ĳ�ʼ Transform ����
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;
    }

    void Update()
    {
        // ��������� Transform ����
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
    }
}
