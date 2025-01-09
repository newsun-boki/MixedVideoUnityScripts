using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.IO;

public class VideoEditorTest : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider timelineSlider;     // 视频进度条
    public Button playPauseButton;    // 播放/暂停按钮
    public Button trimButton;         // 剪辑按钮
    public Text currentTimeText;      // 当前时间文本
    public Text totalTimeText;        // 总时间文本

    // 剪辑起始和结束时间
    private float trimStartTime = 0f;
    private float trimEndTime = 0f;
    private bool isPaused = true;

    void Start()
    {
        InitializeVideoPlayer();
        SetupUI();
    }

    void InitializeVideoPlayer()
    {
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void SetupUI()
    {
        // 设置进度条事件
        timelineSlider.onValueChanged.AddListener(OnTimelineValueChanged);

        // 设置播放/暂停按钮事件
        playPauseButton.onClick.AddListener(TogglePlayPause);

        // 设置剪辑按钮事件
        trimButton.onClick.AddListener(TrimVideo);
    }

    void Update()
    {
        if (videoPlayer.isPrepared)
        {
            UpdateTimelineUI();
        }
    }

    void UpdateTimelineUI()
    {
        // 更新进度条
        timelineSlider.value = (float)(videoPlayer.time / videoPlayer.length);

        // 更新时间文本
        currentTimeText.text = FormatTime(videoPlayer.time);
        totalTimeText.text = FormatTime(videoPlayer.length);
    }

    string FormatTime(double timeInSeconds)
    {
        System.TimeSpan time = System.TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            isPaused = true;
        }
        else
        {
            videoPlayer.Play();
            isPaused = false;
        }
    }

    public void OnTimelineValueChanged(float value)
    {
        if (videoPlayer.isPrepared)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }

    void OnVideoPrepared(VideoPlayer player)
    {
        trimEndTime = (float)videoPlayer.length;
        timelineSlider.value = 0;
    }

    void OnVideoEnd(VideoPlayer player)
    {
        //isPaused = true;
    }

    public void SetTrimStartTime()
    {
        trimStartTime = (float)videoPlayer.time;
    }

    public void SetTrimEndTime()
    {
        trimEndTime = (float)videoPlayer.time;
    }

    public void TrimVideo()
    {
        if (trimStartTime >= trimEndTime)
        {
            Debug.LogError("Invalid trim times");
            return;
        }

        StartCoroutine(TrimVideoCoroutine());
    }

    private IEnumerator TrimVideoCoroutine()
    {
        // 获取原始视频路径
        string sourceVideoPath = videoPlayer.url;

        // 创建输出路径
        string outputPath = Path.Combine(
            Application.persistentDataPath,
            $"trimmed_video_{System.DateTime.Now:yyyyMMdd_HHmmss}.mp4"
        );

        // 构建 FFmpeg 命令
        string ffmpegCommand = $"-i \"{sourceVideoPath}\" -ss {trimStartTime} -t {trimEndTime - trimStartTime} " +
                             $"-c copy \"{outputPath}\"";

        // 执行 FFmpeg 命令（需要实现 FFmpeg 调用逻辑）
        // 这里需要添加实际的 FFmpeg 调用代码

        Debug.Log($"Video trimmed and saved to: {outputPath}");
        yield return null;
    }
}