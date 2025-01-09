using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class WindowCaptureController : MonoBehaviour
{
    private UdpClient udpClient;
    public string serverAddress = "127.0.0.1"; // Python 脚本服务器地址
    public int serverPort = 65432;            // Python 脚本服务器端口
    public VideoPlayer videoPlayer;          // 关联 VideoPlayer 组件

    private IPEndPoint serverEndPoint;

    void Start()
    {
        try
        {
            // 初始化 UDP 客户端和服务器端点
            udpClient = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            Debug.Log("Initialized UDP client.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize UDP client: {e.Message}");
        }
    }

    public void StartRecording()
    {
        if (udpClient != null)
        {
            try
            {
                SendMessage("start");
                Debug.Log("Sent 'start' signal to Python server.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send 'start' signal: {e.Message}");
            }
        }
    }

    public void StopRecording()
    {
        if (udpClient != null)
        {
            try
            {
                SendMessage("stop");
                Debug.Log("Sent 'stop' signal to Python server.");

                // 接收视频文件名
                string videoFilename = ReceiveMessage();
                if (!string.IsNullOrEmpty(videoFilename))
                {
                    Debug.Log($"Received video file: {videoFilename}");
                    PlayVideo(videoFilename);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send 'stop' signal: {e.Message}");
            }
        }
    }

    public void ExitServer()
    {
        if (udpClient != null)
        {
            try
            {
                SendMessage("exit");
                Debug.Log("Sent 'exit' signal to Python server.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send 'exit' signal: {e.Message}");
            }
        }

        Cleanup();
    }

    private void Cleanup()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
            Debug.Log("UDP client closed.");
        }
    }

    private void SendMessage(string message)
    {
        if (udpClient == null) return;

        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, serverEndPoint);
    }

    private string ReceiveMessage()
    {
        if (udpClient == null) return null;

        // 设置超时时间（防止无限等待）
        udpClient.Client.ReceiveTimeout = 5000;

        try
        {
            IPEndPoint remoteEndPoint = null;
            byte[] data = udpClient.Receive(ref remoteEndPoint);
            return Encoding.UTF8.GetString(data);
        }
        catch (SocketException e)
        {
            Debug.LogWarning($"No response received: {e.Message}");
            return null;
        }
    }

    private void PlayVideo(string videoPath)
    {
        if (videoPlayer != null)
        {
            // 检查路径是否存在
            if (!File.Exists(videoPath))
            {
                Debug.LogError($"Video file not found: {videoPath}");
                return;
            }

            // 设置视频路径并播放
            videoPlayer.url = videoPath;
            videoPlayer.Play();
            Debug.Log($"Playing video: {videoPath}");
        }
        else
        {
            Debug.LogError("VideoPlayer not assigned.");
        }
    }

    void OnApplicationQuit()
    {
        ExitServer();
    }
}
