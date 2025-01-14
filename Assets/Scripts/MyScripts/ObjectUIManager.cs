using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUIManager : MonoBehaviour
{
    public GameObject objectToDuplicate;
    private Transform objectParent;

    public bool isActivated = false;
    // ���ƺ���
    public void DuplicateObject()
    {
        if (objectToDuplicate != null)
        {
            objectParent = GameObject.Find("AssetsUI").transform;
            // ������Ϸ����
            GameObject duplicatedObject = Instantiate(objectToDuplicate, objectParent);

            duplicatedObject.transform.position = objectToDuplicate.transform.position;
            // newParent = GameObject.Find("Creations").transform;
            // objectToDuplicate.transform.SetParent(newParent);
            AssetManager.Instance.AddAsset(objectToDuplicate);
            Debug.Log($"Duplicated object: {duplicatedObject.name}");
        }
        else
        {
            Debug.LogWarning("No object assigned to duplicate.");
        }
    }

    // public void ActivateObject(bool activate)
    // {
    //     if (objectToDuplicate != null)
    //     {
    //         ObjectHighlight objectHighlight = objectToDuplicate.GetComponent<ObjectHighlight>();

    //         if (activate)
    //         {
    //             isActivated = true;
    //             objectHighlight.HighlightingOn();

    //         }
    //         else
    //         {
    //             isActivated = false;
    //             objectHighlight.HighlightingOff();
    //         }
    //     }
    // }

    // public void SwitchActivateState()
    // {
    //     if (objectToDuplicate != null)
    //     {
    //         ObjectHighlight objectHighlight = objectToDuplicate.GetComponent<ObjectHighlight>();

    //         if (!isActivated)
    //         {
    //             isActivated = true;
    //             objectHighlight.HighlightingOn();

    //         }
    //         else
    //         {
    //             isActivated = false;
    //             objectHighlight.HighlightingOff();
    //         }
    //     }
    // }

    public void SelectThisObject()
    {
        AssetManager.Instance.SelectAsset(objectToDuplicate);
    }
}
