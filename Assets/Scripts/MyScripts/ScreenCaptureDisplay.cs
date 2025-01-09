using UnityEngine;
using UnityEngine.UI;

public class ScreenCaptureDisplay : MonoBehaviour
{
    public RawImage displayImage; // ������ʾ��ͼ�� UI ���
    private Texture2D screenTexture; // ���ڱ����ͼ������
    private RenderTexture renderTexture; // ���ڲ�����Ļ�� RenderTexture

    private int captureWidth = Screen.width;  // ��ͼ���
    private int captureHeight = Screen.height; // ��ͼ�߶�

    void Start()
    {
        // ��ʼ�� RenderTexture
        renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        renderTexture.Create();

        // ��ʼ�� Texture2D
        screenTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        InvokeRepeating("CaptureScreenAndUpdate", 0f, 0.1f); // ÿ�� 0.1 ���һ��ͼ
    }

    void CaptureScreenAndUpdate()
    {
        // ������Ļ�� RenderTexture
        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);

        // �� RenderTexture ת��Ϊ Texture2D
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenTexture.Apply();
        RenderTexture.active = null;

        // ���� RawImage ���������
        displayImage.texture = screenTexture;
    }

    void OnDestroy()
    {
        // ������Դ
        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        if (screenTexture != null)
        {
            Destroy(screenTexture);
        }
    }
}
