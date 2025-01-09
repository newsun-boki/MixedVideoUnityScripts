using UnityEngine;
using UnityEngine.UI;

public class ScreenCaptureDisplay : MonoBehaviour
{
    public RawImage displayImage; // 用于显示截图的 UI 组件
    private Texture2D screenTexture; // 用于保存截图的纹理
    private RenderTexture renderTexture; // 用于捕获屏幕的 RenderTexture

    private int captureWidth = Screen.width;  // 截图宽度
    private int captureHeight = Screen.height; // 截图高度

    void Start()
    {
        // 初始化 RenderTexture
        renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        renderTexture.Create();

        // 初始化 Texture2D
        screenTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        InvokeRepeating("CaptureScreenAndUpdate", 0f, 0.1f); // 每隔 0.1 秒截一次图
    }

    void CaptureScreenAndUpdate()
    {
        // 捕获屏幕到 RenderTexture
        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);

        // 将 RenderTexture 转换为 Texture2D
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenTexture.Apply();
        RenderTexture.active = null;

        // 更新 RawImage 组件的纹理
        displayImage.texture = screenTexture;
    }

    void OnDestroy()
    {
        // 清理资源
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
