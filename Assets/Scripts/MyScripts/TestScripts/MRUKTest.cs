using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Meta.XR.MRUtilityKit;
using Meta.XR.Util;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class MRUKTest : MonoBehaviour
{
    private OVRCameraRig _cameraRig;
    public GameObject _debugCube;
    void Awake()
    {
        _cameraRig = FindObjectOfType<OVRCameraRig>();
    }

    private Ray GetControllerRay()
    {
        Vector3 rayOrigin;
        Vector3 rayDirection;
        if (OVRInput.activeControllerType == OVRInput.Controller.Touch
            || OVRInput.activeControllerType == OVRInput.Controller.RTouch)
        {
            rayOrigin = _cameraRig.rightHandOnControllerAnchor.position;
            rayDirection = _cameraRig.rightHandOnControllerAnchor.forward;
        }
        else if (OVRInput.activeControllerType == OVRInput.Controller.LTouch)
        {
            rayOrigin = _cameraRig.leftHandOnControllerAnchor.position;
            rayDirection = _cameraRig.leftHandOnControllerAnchor.forward;
        }
        else // hands
        {
            var rightHand = _cameraRig.rightHandAnchor.GetComponentInChildren<OVRHand>();
            // can be null if running in Editor with Meta Linq app and the headset is put off
            if (rightHand != null)
            {
                rayOrigin = rightHand.PointerPose.position;
                rayDirection = rightHand.PointerPose.forward;
            }
            else
            {
                rayOrigin = _cameraRig.centerEyeAnchor.position;
                rayDirection = _cameraRig.centerEyeAnchor.forward;
            }
        }

        return new Ray(rayOrigin, rayDirection);
    }
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        var ray = GetControllerRay();
        MRUKAnchor sceneAnchor = null;
        var positioningMethod = MRUK.PositioningMethod.DEFAULT;

        var bestPose = MRUK.Instance?.GetCurrentRoom()?.GetBestPoseFromRaycast(ray, Mathf.Infinity,
            new LabelFilter(), out sceneAnchor, positioningMethod);
        if (bestPose.HasValue && sceneAnchor && _debugCube)
        {
            _debugCube.transform.position = bestPose.Value.position;
            _debugCube.transform.rotation = bestPose.Value.rotation;
            _debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
           
        }
    }
}
