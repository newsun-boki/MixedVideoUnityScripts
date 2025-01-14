using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManagerTest : MonoBehaviour
{
    public static ProjectManagerTest Instance { get; private set; } // ����ʵ��

    public Button newProjectButton; // ��ť����

    public string ProjectPath { get; private set; } // ������󴴽����ļ���·��

    private void Awake()
    {
        // ȷ������Ψһ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���ֶ����ڳ����л�ʱ��������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ϊ��ť�󶨵���¼�
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
        // ��ȡ��ǰʱ��
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        // ����Ŀ���ļ���·��
        string folderPath = Path.Combine(Application.dataPath, "Datas", "Project_" + currentTime);

        // ��鲢����Datas�ļ���
        string datasFolderPath = Path.Combine(Application.dataPath, "Datas");
        if (!Directory.Exists(datasFolderPath))
        {
            Directory.CreateDirectory(datasFolderPath);
        }

        // ������Ŀ�ļ���
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            ProjectPath = folderPath; // ����·����ȫ�ֱ���
            Debug.Log($"Project folder created at: {folderPath}");

            // �������ļ���
            CreateSubFolder(folderPath, "Videos");
            CreateSubFolder(folderPath, "Tasks");
        }
        else
        {
            ProjectPath = folderPath; // ����Ѵ��ڣ�Ҳ��¼·��
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
