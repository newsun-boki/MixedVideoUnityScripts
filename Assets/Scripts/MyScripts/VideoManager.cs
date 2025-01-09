using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void PlayVideo(string videoPath)
    {
        if (File.Exists(videoPath))
        {
            videoPlayer.url = videoPath;
            videoPlayer.Play();
            Debug.Log($"Playing video: {videoPath}");
        }
        else
        {
            Debug.LogError($"Video file not found: {videoPath}");
        }
    }
}
