using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static NativeWebSocket.Models.ApiResponse;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    private string BaseUrl = "http://localhost:5000/api";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        string testUrl = GetUrl();
        if (!string.IsNullOrEmpty(testUrl))
        {
            BaseUrl = testUrl;
        }
    }

    string GetUrl()
    {
        var config = Resources.Load<ServerConfig>("ServerConfig");
        if (config == null)
        {
            Debug.LogError("ServerConfig.asset이 Resources 폴더에 없습니다!");
            return "";
        }
        string url = config.GetApiServerIP();
        return url;
    }
    public IEnumerator Post(string endpoint, Dictionary<string, string> formData, Action<string> onSuccess, Action<string> onError)
    {
        WWWForm form = new WWWForm();
        foreach (var kv in formData)
        {
            form.AddField(kv.Key, kv.Value);
        }

        string url = $"{BaseUrl}/{endpoint}";
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        www.useHttpContinue = false;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(www.error);
            var err = JsonUtility.FromJson<ErrorRes>(www.downloadHandler.text);
            PopupManager.Instance.ShowNoticePopup(err?.message ?? $"HTTP {www.responseCode}");
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }
}