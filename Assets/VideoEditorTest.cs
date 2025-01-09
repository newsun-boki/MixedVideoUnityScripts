using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.IO;

public class VideoEditorTest : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider timelineSlider;     // ��Ƶ������
    public Button playPauseButton;    // ����/��ͣ��ť
    public Button trimButton;         // ������ť
    public Text currentTimeText;      // ��ǰʱ���ı�
    public Text totalTimeText;        // ��ʱ���ı�

    // ������ʼ�ͽ���ʱ��
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
        // ���ý������¼�
        timelineSlider.onValueChanged.AddListener(OnTimelineValueChanged);

        // ���ò���/��ͣ��ť�¼�
        playPauseButton.onClick.AddListener(TogglePlayPause);

        // ���ü�����ť�¼�
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
        // ���½�����
        timelineSlider.value = (float)(videoPlayer.time / videoPlayer.length);

        // ����ʱ���ı�
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
        // ��ȡԭʼ��Ƶ·��
        string sourceVideoPath = videoPlayer.url;

        // �������·��
        string outputPath = Path.Combine(
            Application.persistentDataPath,
            $"trimmed_video_{System.DateTime.Now:yyyyMMdd_HHmmss}.mp4"
        );

        // ���� FFmpeg ����
        string ffmpegCommand = $"-i \"{sourceVideoPath}\" -ss {trimStartTime} -t {trimEndTime - trimStartTime} " +
                             $"-c copy \"{outputPath}\"";

        // ִ�� FFmpeg �����Ҫʵ�� FFmpeg �����߼���
        // ������Ҫ���ʵ�ʵ� FFmpeg ���ô���

        Debug.Log($"Video trimmed and saved to: {outputPath}");
        yield return null;
    }
}