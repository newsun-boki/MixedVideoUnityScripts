using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManagerTest : MonoBehaviour
{
    public static ProjectManagerTest Instance { get; private set; } // 单例实例

    public Button newProjectButton; // 按钮引用

    public string ProjectPath { get; private set; } // 保存最后创建的文件夹路径

    private void Awake()
    {
        // 确保单例唯一性
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持对象在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 为按钮绑定点击事件
        if (newProjectButton != null)
        {
            newProjectButton.onClick.AddListener(CreateProjectFolder);
        }
        else
        {
            Debug.LogError("Button is not assigned in the Inspector!");
        }
    }

    public void CreateProjectFolder()
    {
        // 获取当前时间
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        // 设置目标文件夹路径
        string folderPath = Path.Combine(Application.dataPath, "Datas", "Project_" + currentTime);

        // 检查并创建Datas文件夹
        string datasFolderPath = Path.Combine(Application.dataPath, "Datas");
        if (!Directory.Exists(datasFolderPath))
        {
            Directory.CreateDirectory(datasFolderPath);
        }

        // 创建项目文件夹
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            ProjectPath = folderPath; // 保存路径到全局变量
            Debug.Log($"Project folder created at: {folderPath}");

            // 创建子文件夹
            CreateSubFolder(folderPath, "Videos");
            CreateSubFolder(folderPath, "Tasks");
        }
        else
        {
            ProjectPath = folderPath; // 如果已存在，也记录路径
            Debug.LogWarning($"Folder already exists: {folderPath}");
        }
    }

    private void CreateSubFolder(string parentFolder, string subFolderName)
    {
        string subFolderPath = Path.Combine(parentFolder, subFolderName);

        if (!Directory.Exists(subFolderPath))
        {
            Directory.CreateDirectory(subFolderPath);
            Debug.Log($"Subfolder created at: {subFolderPath}");
        }
        else
        {
            Debug.LogWarning($"Subfolder already exists: {subFolderPath}");
        }
    }
}
