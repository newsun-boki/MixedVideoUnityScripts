using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InspectorManager : MonoBehaviour
{
    public static InspectorManager Instance { get; private set; }
    [Header("InspectorManager")]
    public TextMeshProUGUI modelName;
    public Toggle hideToggle;       // 隐藏开关
    public Toggle lockToggle;       // 锁定开关
    public Toggle alignToggle;       // 对齐开关
    private GameObject currentSelectedObject;
    private AssetBehaviour assetBehaviour;
    public Slider xSlider;
    public Slider ySlider;
    public Slider zSlider;
    public Slider allSlider;
    private void OnEnable()
    {
        UpdateInspector();
    }
    public void UpdateInspector()
    {
        Instance = this;
        currentSelectedObject = AssetManager.Instance.GetCurrentSelectedAsset();
        Debug.Log($"currentSelectedObject:{currentSelectedObject.name}");
        assetBehaviour = currentSelectedObject.GetComponent<AssetBehaviour>();

        GetAssetAttributes();
        BindToggleEvents();
    }
    void GetAssetAttributes()
    {
        modelName.text = currentSelectedObject.name;
        hideToggle.isOn = assetBehaviour.IsHided;
        lockToggle.isOn = assetBehaviour.IsLocked;
        alignToggle.isOn = assetBehaviour.IsAlign;
        xSlider.value = assetBehaviour.ScaleFactor.x;
        ySlider.value = assetBehaviour.ScaleFactor.y;
        zSlider.value = assetBehaviour.ScaleFactor.z;
        allSlider.value = assetBehaviour.ScaleAll;
        // hideToggle.isOn
    }

    void BindToggleEvents()
    {
        // 添加监听器，当 Toggle 变化时触发
        // hideToggle.onValueChanged.AddListener(OnHideToggleChanged);
        lockToggle.onValueChanged.AddListener(OnLockToggleChanged);
        alignToggle.onValueChanged.AddListener(OnAlignToggleChanged);
        xSlider.onValueChanged.AddListener(OnSliderValueChanged);
        ySlider.onValueChanged.AddListener(OnSliderValueChanged);
        zSlider.onValueChanged.AddListener(OnSliderValueChanged);
        allSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // void OnHideToggleChanged(bool isOn)
    // {
    //     // 调用 assetBehaviour 的方法处理隐藏逻辑
    //     assetBehaviour.SetHidden(isOn);
    // }

    void OnLockToggleChanged(bool isOn)
    {
        // 调用 assetBehaviour 的方法处理锁定逻辑
        assetBehaviour.LockTransform(isOn);
    }
    void OnAlignToggleChanged(bool isOn)
    {
        assetBehaviour.IsAlign = isOn;
    }
    void OnSliderValueChanged(float value)
    {
        // Update the scale factor in AssetBehaviour
        Vector3 newScaleFactor = new Vector3(xSlider.value, ySlider.value, zSlider.value);

        assetBehaviour.SetScaleFactor(newScaleFactor, allSlider.value);
    }
    private void OnDestroy()
    {
        // 解绑事件，避免内存泄漏
        // hideToggle.onValueChanged.RemoveListener(OnHideToggleChanged);
        lockToggle.onValueChanged.RemoveListener(OnLockToggleChanged);
    }

}
