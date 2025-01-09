using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public string baseUrl = "http://localhost:12345";

    public IEnumerator SendPostRequest(string endpoint, Action<string> onSuccess = null)
    {
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(baseUrl + endpoint, ""))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator SendJsonPostRequest<T>(string endpoint, T data, Action<string> onSuccess = null)
    {
        string jsonData = JsonUtility.ToJson(data);
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    public IEnumerator DownloadFile(string endpoint, string fileName,string localPath, Action onSuccess = null, int maxRetries = 5, float retryDelay = 1.0f)
    {
        int retryCount = 0;
        bool downloadCompleted = false;
        string url = baseUrl + endpoint + UnityWebRequest.EscapeURL(fileName);
        while (!downloadCompleted && retryCount < maxRetries)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                Debug.Log(request.responseCode);
                if (request.responseCode == 400)
                {
                    Debug.Log("File not found. Retrying...");
                    retryCount++;
                    yield return new WaitForSeconds(retryDelay);
                }
                else
                {
                    System.IO.File.WriteAllBytes(localPath, request.downloadHandler.data);
                    Debug.Log($"File downloaded to: {localPath}");
                    onSuccess?.Invoke();
                    downloadCompleted = true;
                }
            }
        }

        if (!downloadCompleted)
        {
            Debug.LogError("Failed to download file after multiple attempts.");
        }
    }
}
