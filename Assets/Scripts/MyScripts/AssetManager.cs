using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance { get; private set; }
    public Transform AssetParent;

    // List to store all the assets in the scene
    private List<GameObject> assets = new List<GameObject>();

    // Currently selected asset
    private GameObject currentSelectedAsset;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Keep this across scenes
    }

    // Add a new asset to the scene
    public void AddAsset(GameObject newAsset)
    {
        if (newAsset != null)
        {
            newAsset.transform.SetParent(AssetParent);
            assets.Add(newAsset);

            Debug.Log($"Asset added: {newAsset.name}");
        }
        else
        {
            Debug.LogWarning("Asset prefab is null. Cannot add asset.");
        }
    }

    // Remove an asset from the scene
    public void RemoveAsset(GameObject asset)
    {
        if (assets.Contains(asset))
        {
            if (currentSelectedAsset == asset)
            {
                DeselectCurrentAsset(); // Deselect before removing
            }
            assets.Remove(asset);
            Destroy(asset);
            Debug.Log($"Asset removed: {asset.name}");
        }
        else
        {
            Debug.LogWarning("Asset not found in the list. Cannot remove.");
        }
    }

    // Select an asset in the scene
    public void SelectAsset(GameObject asset)
    {
        if (assets.Contains(asset))
        {
            // Deselect the previous asset
            DeselectCurrentAsset();

            // Set the new asset as selected
            currentSelectedAsset = asset;
            var objectHighlight = currentSelectedAsset.GetComponent<ObjectHighlight>();
            if (objectHighlight != null)
            {
                objectHighlight.HighlightingOn();
            }
            InspectorManager.Instance.UpdateInspector();
            Debug.Log($"Asset selected: {currentSelectedAsset.name}");
        }
        else
        {
            Debug.LogWarning("Asset not found in the list. Cannot select.");
        }
    }

    // Deselect the current asset
    private void DeselectCurrentAsset()
    {
        if (currentSelectedAsset != null)
        {
            var objectHighlight = currentSelectedAsset.GetComponent<ObjectHighlight>();
            if (objectHighlight != null)
            {
                objectHighlight.HighlightingOff();
            }
            currentSelectedAsset = null;
        }
    }

    // Get the currently selected asset
    public GameObject GetCurrentSelectedAsset()
    {
        return currentSelectedAsset;
    }

    public void DisableSelection()
    {
        foreach (var asset in assets)
        {
            Transform grabInteractor = asset.transform.Find("DistanceHandGrabRelative");
            if (grabInteractor != null)
            {
                grabInteractor.gameObject.SetActive(false);
            }
        }
    }

    public void EnableSelection()
    {
        foreach (var asset in assets)
        {
            Transform grabInteractor = asset.transform.Find("DistanceHandGrabRelative");
            if (grabInteractor != null)
            {
                grabInteractor.gameObject.SetActive(true);
            }
        }
    }
}
