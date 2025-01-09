using UnityEngine;

[System.Serializable]
public class VideoTransformData
{
    public string video_name;
    public int structure_transformation;
    public string text_prompt;
}

[System.Serializable]
public class VideoTransformResponse
{
    public string generated_video;
    public int transformation;
    public string prompt;
}

public class VideoTransformer : MonoBehaviour
{
    public string videoFileNameTobeTransformed = null;

    private NetworkManager networkManager;
    private VideoManager videoManager;
    private VideoRecorder videoRecorder;
    private bool isProcessing = false;
    void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        videoManager = FindObjectOfType<VideoManager>();
        videoRecorder = FindObjectOfType<VideoRecorder>();
    }

    public void ProcessRecordVideo()
    {
        if (isProcessing)
        {
            Debug.LogWarning("A video transformation is already in progress.");
            return;
        }

        isProcessing = true; // ���ñ��Ϊ������
        TransformVideo(
            videoRecorder.currentVideoFileName,
            3,  // structure_transformation
            "make everything like LEGO"
        );
    }
    public void ProcessCustomVideo()
    {
        if (isProcessing)
        {
            Debug.LogWarning("A video transformation is already in progress.");
            return;
        }

        isProcessing = true; // ���ñ��Ϊ������
        TransformVideo(
            videoFileNameTobeTransformed,
            3,  // structure_transformation
            "make everything like LEGO"
        );
    }
    public void TransformVideo(string videoName, int transformation, string prompt)
    {
        VideoTransformData data = new VideoTransformData
        {
            video_name = videoName,
            structure_transformation = transformation,
            text_prompt = prompt
        };

        Debug.Log($"Sending video transformation request...");
        StartCoroutine(networkManager.SendJsonPostRequest("/transform_video", data, HandleTransformResponse));
    }

    private void HandleTransformResponse(string jsonResponse)
    {
        isProcessing = false; // ������ɺ����ñ��
        VideoTransformResponse response = JsonUtility.FromJson<VideoTransformResponse>(jsonResponse);
        Debug.Log($"Transformation complete: {response.generated_video}");
        string localPath = $"{Application.dataPath}/Videos/{System.IO.Path.GetFileName(response.generated_video)}";
        StartCoroutine(networkManager.DownloadFile("/download_generated_video?filename=", response.generated_video, localPath, () =>
        {
            Debug.Log($"Transformed video downloaded: {localPath}");
            videoManager.PlayVideo(localPath);
        }));
    }
}
