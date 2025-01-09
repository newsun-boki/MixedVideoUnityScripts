using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class VideoStreamClient : MonoBehaviour
{
    public RawImage displayImage; // ������ʾ����� UI ���
    public string serverIP = "127.0.0.1"; // ����� IP ��ַ
    public int serverPort = 65433; // ����˶˿�

    private UdpClient udpClient;
    private Texture2D texture;
    private ConcurrentQueue<byte[]> imageQueue; // ���ڴ洢���յ���ͼ������

    void Start()
    {
        // ��ʼ�� UDP �ͻ���
        udpClient = new UdpClient(serverPort);
        udpClient.Client.ReceiveBufferSize = 65536; // ���ý��ջ�������С

        // ����һ����ʼ����
        texture = new Texture2D(2, 2); // Ĭ�Ϸֱ��ʣ�����ݽ������ݶ�̬������

        // ��ʼ������
        imageQueue = new ConcurrentQueue<byte[]>();

        // ��ʼ��������
        BeginReceive();
    }

    void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    void OnReceive(IAsyncResult result)
    {
        try
        {
            Debug.Log("received");
            // ��������
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, serverPort);
            byte[] data = udpClient.EndReceive(result, ref remoteEP);

            // �����յ���ͼ�����ݴ������
            if (data != null && data.Length > 0)
            {
                imageQueue.Enqueue(data);
            }

            // ��������
            BeginReceive();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e.Message}");
        }
    }

    void Update()
    {
        // ���߳��д�������е�ͼ������
        if (imageQueue.TryDequeue(out byte[] imageData))
        {
            try
            {
                // ���� JPEG ����
                texture.LoadImage(imageData);
                texture.Apply();

                // ������ʾ������
                displayImage.texture = texture;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing image data: {e.Message}");
            }
        }
    }

    void OnDestroy()
    {
        udpClient.Close();
    }
}
