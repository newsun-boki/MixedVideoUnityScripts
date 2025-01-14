using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBehaviour : MonoBehaviour
{
    public bool IsHided { get; private set; } = false;
    public bool IsLocked { get; private set; } = true;
    public bool IsAlign { get; set; } = false;
    public Vector3 ScaleFactor { get; private set; } = Vector3.one; // Default to no scaling
    public float ScaleAll = 1f;
    private Vector3 initialScale;
    private bool isSelected = false;

    private Vector3 lastPosition;
    private Quaternion lastRotation;
    // Start is called before the first frame update
    void Start()
    {
        initialScale = transform.localScale; // Store the initial scale
        CheckAttributes();
    }


    void CheckAttributes()
    {
        IsLocked = this.gameObject.GetComponent<LockTransform>().enabled;
    }

    public void LockTransform(bool Lock)
    {
        if (Lock)
        {
            IsLocked = true;
            this.gameObject.GetComponent<LockTransform>().enabled = true;
        }
        else
        {
            IsLocked = false;
            this.gameObject.GetComponent<LockTransform>().enabled = false;
        }
    }

    public void SetScaleFactor(Vector3 scaleFactor, float scaleAll)
    {
        ScaleFactor = scaleFactor;
        ScaleAll = scaleAll;
        ApplyScale();
    }

    private void ApplyScale()
    {
        transform.localScale = Vector3.Scale(initialScale, ScaleFactor) * ScaleAll;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (!selected && IsAlign)
        {
            StartCoroutine(DelayedResetTransform());
        }
    }

    private IEnumerator DelayedResetTransform()
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = lastPosition;
        transform.rotation = lastRotation;
    }
    void Update()
    {
        if (IsAlign && isSelected)
        {
            transform.position = MRUKTest.Instance.GetBestPosition();
            transform.rotation = MRUKTest.Instance.GetBestRotation();
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

}