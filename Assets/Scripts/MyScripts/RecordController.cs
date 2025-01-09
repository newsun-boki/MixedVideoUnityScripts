
using Oculus.Interaction;

using Oculus.Interaction;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class RecordController : MonoBehaviour
{
    private UdpClient udpClient;
    private string baseUrl = "http://localhost:12345";
    public InteractableUnityEventWrapper startRecordingButton; // 开始录制按钮
    public InteractableUnityEventWrapper stopRecordingButton;  // 停止录制按钮
    public VideoPlayer videoPlayer;          // ���� VideoPlayer ���
    public string currentVideoFilename;
    void Start()
    {
        udpClient = new UdpClient();
        startRecordingButton.WhenUnhover.AddListener(StartRecording);
        stopRecordingButton.WhenUnselect.AddListener(StopRecording); 
    }
    //playRecordingButton.onClick.AddListener(DownloadVideo);
    public void StartRecording()
    {
        Debug.Log("start recording");
        StartCoroutine(SendPostRequest("/start_recording"));
    }
    public void StopRecording()
    {
        StartCoroutine(SendPostRequest("/stop_recording", DownloadVideo));
    }
    private void DownloadVideo(string videoFilename)
    {
        StartCoroutine(DownloadVideoFile("/download_video", videoFilename));
    }



    private IEnumerator SendPostRequest(string endpoint, Action<string> onSuccess = null)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(baseUrl + endpoint, ""))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                if (onSuccess != null)
                {
                    // Parse the JSON response to get the video path
                    var jsonResponse = JsonUtility.FromJson<VideoResponse>(request.downloadHandler.text);
                    onSuccess(jsonResponse.video_path);
                }
            }
        }
    }



    private IEnumerator DownloadVideoFile(string endpoint, string videoFilename)
    {
        string url = baseUrl + "/download_video?filename=" + UnityWebRequest.EscapeURL(videoFilename);
        bool videoDownloaded = false;
        int maxRetries = 5;
        int retryCount = 0;
        float retryDelay = 1.0f; // 2 seconds delay between retries

        while (!videoDownloaded && retryCount < maxRetries)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.responseCode == 400)
                {
                    Debug.Log("Video not yet saved, retrying...");
                    retryCount++;
                    yield return new WaitForSeconds(retryDelay); // Wait before retrying
                }
                else
                {
                    currentVideoFilename = Path.GetFileName(videoFilename);
                    string filePath = Application.dataPath + "/Videos/" + currentVideoFilename;
                    // Save the video file to disk
                    //string filePath = Application.dataPath + "/downloaded_video.mp4";
                    System.IO.File.WriteAllBytes(filePath, request.downloadHandler.data);
                    Debug.Log("Video downloaded and saved to: " + filePath);
                    PlayVideo(filePath);
                    videoDownloaded = true;
                }
            }
        }

        if (!videoDownloaded)
        {
            Debug.LogError("Failed to download video after multiple attempts.");
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
        udpClient.Close();
    }
    [Serializable]
    private class VideoResponse
    {
        public string video_path;
    }
}
