using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlight : MonoBehaviour
{
    protected HighlightableObject _HighlightableObject;
    void Awake()
    {
        //初始化组件
        _HighlightableObject = gameObject.AddComponent<HighlightableObject>();

    }
    public void HighlightingOn()
    {
        _HighlightableObject.ConstantOn(Color.cyan);
        Debug.Log("_HighlightableObject.ConstantOn(Color.cyan);");
    }
    public void HighlightingOff()
    {
        _HighlightableObject.ConstantOff();
    }

}
