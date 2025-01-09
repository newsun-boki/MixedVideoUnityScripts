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
        this.gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SwitchPanels(string UIPanelName)
    {
        for(int i = 0; i < UIPanels.Count; i++)
        {
            if (UIPanels[i].name == UIPanelName)
            {
                UIPanels[i].SetActive(true);
            }
            else
            {
                UIPanels[i].SetActive(false);

            }
        }
    }
}
