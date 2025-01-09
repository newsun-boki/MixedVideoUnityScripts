using UnityEngine;
using UnityEngine.UI;

public class VideoRecorder : MonoBehaviour
{
    private NetworkManager networkManager;
    public string currentVideoFileName;
    private VideoManager videoManager;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        videoManager = FindObjectOfType<VideoManager>();

    }

    public void StartRecording()
    {
        Debug.Log("Starting recording...");
        StartCoroutine(networkManager.SendPostRequest("/start_recording"));
    }

    public void StopRecording()
    {
        Debug.Log("Stopping recording...");
        StartCoroutine(networkManager.SendPostRequest("/stop_recording", DownloadVideo));
    }

    private void DownloadVideo(string response)
    {
        string videoName = JsonUtility.FromJson<VideoResponse>(response).video_name;
        currentVideoFileName = System.IO.Path.GetFileName(videoName);
        string localPath = $"{Application.dataPath}/Videos/{System.IO.Path.GetFileName(videoName)}";
        StartCoroutine(networkManager.DownloadFile("/download_video?filename=", videoName, localPath, () =>
        {
            Debug.Log($"Video downloaded: {localPath}");
            videoManager.PlayVideo(localPath);
        }));
    }

    [System.Serializable]
    private class VideoResponse
    {
        public string video_name;
    }
}
