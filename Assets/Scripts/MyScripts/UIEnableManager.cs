using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnableManager : MonoBehaviour
{
    public Transform UIFollowTarget;
    public List<GameObject> UIPanels;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        transform.position = UIFollowTarget.position;
        transform.eulerAngles = new Vector3(0f, UIFollowTarget.eulerAngles.y, 0f);
    }
    // Update is called once per frame
    public void SwitchActive()
    {
        bool newState = !gameObject.activeSelf;
        this.gameObject.SetActive(newState);

        // 当UI被禁用时，启用选择功能
        if (!newState)
        {
            AssetManager.Instance.EnableSelection();
        }
        else
        {
            AssetManager.Instance.DisableSelection();
        }
    }

    public void SwitchPanels(string UIPanelName)
    {
        for (int i = 0; i < UIPanels.Count; i++)
        {
            if (UIPanels[i].name == UIPanelName)
            {
                UIPanels[i].SetActive(true);
                if (UIPanelName == "AssetsUI")
                {
                    AssetManager.Instance.DisableSelection();
                }
                else
                {
                    AssetManager.Instance.EnableSelection();
                }
            }
            else
            {
                UIPanels[i].SetActive(false);

            }
        }
    }
}
