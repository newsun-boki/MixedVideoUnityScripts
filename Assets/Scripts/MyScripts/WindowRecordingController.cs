using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections;
using System.IO;
using uWindowCapture;
using Debug = UnityEngine.Debug;
using Oculus.Interaction;
using System;
using System.Threading;
using System.Collections.Generic;

public class WindowRecordingController : MonoBehaviour
{
    public InteractableUnityEventWrapper startRecordingButton; // 开始录制按钮
    public InteractableUnityEventWrapper stopRecordingButton;  // 停止录制按钮
    public GameObject uwcWindowTextureObject; // 指向 UwcWindowTexture 脚本的对象

    private UwcWindowTexture uwcWindowTexture;
    private bool isRecording = false;
    private float frameRate = 30f; // 每秒捕获 30 帧
    private float captureInterval;
    private string captureFolder = "CaptureFrames"; // 图像保存路径
    private string currentSessionFolder; // 当前录制会话的子文件夹路径
    private int frameCount = 0;
    private int frameWidth;
    private int frameHeight;
    private List<byte[]> frameDataList = new List<byte[]>(); // 缓存帧数据

    void Start()
    {
        captureFolder = Path.Combine("Sessions", "Session1");
        captureFolder = Path.Combine(captureFolder, "Assets");
        uwcWindowTexture = uwcWindowTextureObject.GetComponent<UwcWindowTexture>();
        captureInterval = 1f / frameRate;

        startRecordingButton.WhenUnhover.AddListener(StartRecording);
        stopRecordingButton.WhenUnselect.AddListener(StopRecording);
    }
    void StartRecording()
    {
        if (!uwcWindowTexture.isValid)
        {
            Debug.LogError("UwcWindowTexture is not valid.");
            return;
        }

        isRecording = true;
        frameDataList.Clear(); // 清空缓存
        frameWidth = uwcWindowTexture.window.width;
        frameHeight = uwcWindowTexture.window.height;
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        currentSessionFolder = Path.Combine(captureFolder, timestamp);

        if (!Directory.Exists(currentSessionFolder))
        {
            Directory.CreateDirectory(currentSessionFolder);
        }
        StartCoroutine(RecordFrames());
        Debug.Log("Recording started.");
    }

    void StopRecording()
    {
        isRecording = false;
        Debug.Log("Recording stopped.");
        StartCoroutine(CreateVideoFromMemory()); // 使用内存中的数据生成视频
    }

    IEnumerator RecordFrames()
    {
        while (isRecording)
        {
            yield return new WaitForSeconds(captureInterval);
            CaptureFrame();
        }
    }

    void CaptureFrame()
    {
        if (!uwcWindowTexture.isValid) return;

        RenderTexture renderTexture = new RenderTexture(frameWidth, frameHeight, 24);
        Graphics.Blit(uwcWindowTexture.window.texture, renderTexture);

        Texture2D texture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, frameWidth, frameHeight), 0, 0);
        texture.Apply();

        byte[] rawData = texture.GetRawTextureData();
        frameDataList.Add(rawData); // 将帧数据存储到内存中

        RenderTexture.active = null;
        Destroy(texture);
        Destroy(renderTexture);
    }

    IEnumerator CreateVideoFromMemory()
    {
        string tempFolder = Path.Combine(currentSessionFolder, "output_video.mp4");

        if (!Directory.Exists(tempFolder))
        {
            Directory.CreateDirectory(tempFolder);
        }

        // 将内存中的数据转为 PNG 文件（必要时）
        for (int i = 0; i < frameDataList.Count; i++)
        {
            string filePath = Path.Combine(tempFolder, $"frame_{i}.png");
            Texture2D tempTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, false);
            tempTexture.LoadRawTextureData(frameDataList[i]);
            tempTexture.Apply();

            byte[] pngData = tempTexture.EncodeToPNG();
            File.WriteAllBytes(filePath, pngData);

            Destroy(tempTexture);
            yield return null; // 避免主线程阻塞
        }

        // 调用 FFmpeg 生成视频
        string inputPattern = Path.Combine(tempFolder, "frame_%d.png");
        string outputVideo = Path.Combine(tempFolder, "output_video.mp4");
        string ffmpegCommand = $"-framerate {frameRate} -i {inputPattern} -c:v libx264 -pix_fmt yuv420p {outputVideo}";

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = ffmpegCommand,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process { StartInfo = processStartInfo };
        process.Start();

        while (!process.HasExited)
        {
            yield return null;
        }

        Debug.Log($"Video creation finished. Saved to {outputVideo}");

        // 清理临时文件
        Directory.Delete(tempFolder, true);
    }
}
