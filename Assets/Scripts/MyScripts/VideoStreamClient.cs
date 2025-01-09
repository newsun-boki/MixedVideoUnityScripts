using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class VideoStreamClient : MonoBehaviour
{
    public RawImage displayImage; // 用于显示画面的 UI 组件
    public string serverIP = "127.0.0.1"; // 服务端 IP 地址
    public int serverPort = 65433; // 服务端端口

    private UdpClient udpClient;
    private Texture2D texture;
    private ConcurrentQueue<byte[]> imageQueue; // 用于存储接收到的图像数据

    void Start()
    {
        // 初始化 UDP 客户端
        udpClient = new UdpClient(serverPort);
        udpClient.Client.ReceiveBufferSize = 65536; // 设置接收缓冲区大小

        // 创建一个初始纹理
        texture = new Texture2D(2, 2); // 默认分辨率（会根据接收内容动态调整）

        // 初始化队列
        imageQueue = new ConcurrentQueue<byte[]>();

        // 开始接收数据
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
            // 接收数据
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, serverPort);
            byte[] data = udpClient.EndReceive(result, ref remoteEP);

            // 将接收到的图像数据存入队列
            if (data != null && data.Length > 0)
            {
                imageQueue.Enqueue(data);
            }

            // 继续接收
            BeginReceive();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e.Message}");
        }
    }

    void Update()
    {
        // 主线程中处理队列中的图像数据
        if (imageQueue.TryDequeue(out byte[] imageData))
        {
            try
            {
                // 解码 JPEG 数据
                texture.LoadImage(imageData);
                texture.Apply();

                // 更新显示的纹理
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
