using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllObjectsManager : MonoBehaviour
{
    // ��̬ʵ��������ȫ�ַ���
    public static AllObjectsManager Instance { get; private set; }

    // ʾ�������Դ洢һЩȫ�ֹ���Ķ����״̬
    public List<GameObject> allObjects = new List<GameObject>();

    void Awake()
    {
        // ȷ��ֻ��һ��ʵ������
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��ѡ���ö����ڳ����л�ʱ��������
        }
        else
        {
            Destroy(gameObject); // �������ʵ�����ڣ������ظ���ʵ��
        }
    }

    // ʾ������ȫ�ֶ����б�����Ӷ���
    public void AddObject(GameObject obj)
    {
        if (!allObjects.Contains(obj))
        {
            allObjects.Add(obj);
        }
    }

    // ʾ�����Ƴ�ȫ�ֶ����б��еĶ���
    public void RemoveObject(GameObject obj)
    {
        if (allObjects.Contains(obj))
        {
            allObjects.Remove(obj);
        }
    }
}
