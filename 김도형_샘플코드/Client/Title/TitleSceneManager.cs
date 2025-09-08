using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket.Models;
using UI;

public class TitleSceneManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nicknameInput; // 회원가입 전용
    public Button loginButton;
    public Button signupButton;
    private bool ui_lock = false;
    
    [Header("Press Any Key → Login UI")]
    public GameObject startPrompt;
    public GameObject loginUIGroup;
    private bool hasStarted = false;

    private async void Start()
    {
        //sound
        SoundManager.Instance.PlayBGM("title");
        loginButton.onClick.AddListener(() => StartCoroutine(Login()));
        signupButton.onClick.AddListener(() => StartCoroutine(Signup()));
        startPrompt.SetActive(true);
        loginUIGroup.SetActive(false);
        await WebSocketClient.Instance.TryConnect();
    }
    
    private void Update()
    {
        if (!hasStarted && Input.anyKeyDown)
        {
            SoundManager.Instance.PlaySFX("ui_click");
            hasStarted = true;
            startPrompt.SetActive(false);
            loginUIGroup.SetActive(true);
        }
    }
    
    IEnumerator Login()
    {
        if (ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "username", usernameInput.text },
            { "password", passwordInput.text }
        };

        yield return ApiManager.Instance.Post(
            "auth/login",
            data,
            onSuccess: (res) =>
            {
                var loginRes = JsonUtility.FromJson<ApiResponse.LoginResponse>(res);
                UserSession.Set(loginRes.userId, loginRes.nickname);
                SceneLoader.Instance.LoadScene("LobbyScene");
            },
            onError: (err) =>
            {
            }
        );
        ui_lock = false;
    }

    IEnumerator Signup()
    {
        if (ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "username", usernameInput.text },
            { "password", passwordInput.text },
            { "nickname", nicknameInput.text }
        };

        yield return ApiManager.Instance.Post(
            "auth/signup",
            data,
            onSuccess: (res) =>
            {
                var loginRes = JsonUtility.FromJson<ApiResponse.LoginResponse>(res);
                UserSession.Set(loginRes.userId, loginRes.nickname);
                SceneLoader.Instance.LoadScene("LobbyScene");
            },
            onError: (err) =>
            {
            }
        );
        ui_lock = false;
    }
}