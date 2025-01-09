using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllObjectsManager : MonoBehaviour
{
    // 静态实例，用于全局访问
    public static AllObjectsManager Instance { get; private set; }

    // 示例：可以存储一些全局管理的对象或状态
    public List<GameObject> allObjects = new List<GameObject>();

    void Awake()
    {
        // 确保只有一个实例存在
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选：让对象在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject); // 如果已有实例存在，销毁重复的实例
        }
    }

    // 示例：向全局对象列表中添加对象
    public void AddObject(GameObject obj)
    {
        if (!allObjects.Contains(obj))
        {
            allObjects.Add(obj);
        }
    }

    // 示例：移除全局对象列表中的对象
    public void RemoveObject(GameObject obj)
    {
        if (allObjects.Contains(obj))
        {
            allObjects.Remove(obj);
        }
    }
}
