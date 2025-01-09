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
    public string serverAddress = "127.0.0.1"; // Python �ű���������ַ
    public int serverPort = 65432;            // Python �ű��������˿�
    public VideoPlayer videoPlayer;          // ���� VideoPlayer ���

    private IPEndPoint serverEndPoint;

    void Start()
    {
        try
        {
            // ��ʼ�� UDP �ͻ��˺ͷ������˵�
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

                // ������Ƶ�ļ���
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

        // ���ó�ʱʱ�䣨��ֹ���޵ȴ���
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
            // ���·���Ƿ����
            if (!File.Exists(videoPath))
            {
                Debug.LogError($"Video file not found: {videoPath}");
                return;
            }

            // ������Ƶ·��������
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
