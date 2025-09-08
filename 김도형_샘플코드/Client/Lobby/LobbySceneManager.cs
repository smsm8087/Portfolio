using NativeWebSocket.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_InputField roomCodeInput;

    [SerializeField] private Button CustomPlayButton;
    [SerializeField] private Button QuickMatchButton;
    [SerializeField] private GameObject SelectMode;
    [SerializeField] private GameObject CustomPlay;

    [SerializeField] private GameObject CreateRoomPopup;
    [SerializeField] private Button CreateRoomPopupCreateButton;
    [SerializeField] private Button CreateRoomPopupCloseButton;

    [SerializeField] private TopMenuManager topMenuManager;
    private bool ui_lock = false;

    private void Start()
    {
        SoundManager.Instance.PlayBGM("lobby");
        SelectMode.SetActive(true);
        CustomPlay.SetActive(false);
        CustomPlayButton.onClick.AddListener(() =>
        {
            SelectMode.SetActive(false);
            CustomPlay.SetActive(true);
            topMenuManager.SetBackButtonListener(() =>
            {
                SelectMode.SetActive(true);
                CustomPlay.SetActive(false);
                CreateRoomPopup.SetActive(false);
                topMenuManager.SetBackButtonListener(null);
            });
        });
        
        createRoomButton.onClick.AddListener(() => {
            CreateRoomPopup.SetActive(true);
        });

        CreateRoomPopupCreateButton.onClick.AddListener(() =>
        {
            CreateRoomPopup.SetActive(false);
            StartCoroutine(CreateRoom());
        });
        CreateRoomPopupCloseButton.onClick.AddListener(() =>
        {
            CreateRoomPopup.SetActive(false);
        });

        joinRoomButton.onClick.AddListener(() => StartCoroutine(JoinRoom()));
        WebSocketClient.Instance.OnMessageReceived += Handle;
    }
    void OnDisable()
    {
        if (WebSocketClient.Instance != null)
            WebSocketClient.Instance.OnMessageReceived -= Handle;
    }
    void Handle(string msg)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(msg);
        switch (netMsg.type)
        {
            case "confirm":
            {
                var handler = new ConfirmPopupHandler();
                handler.Handle(netMsg);
            }
            break;
            case "notice":
            {
                var handler = new NoticePopupHandler();
                handler.Handle(netMsg);
            }
            break;
            case "room_created":
            {
                var handler = new CreateRoomHandler();
                handler.Handle(netMsg);
            }
            break;
            case "room_joined":
            {
                var handler = new JoinRoomHandler();
                handler.Handle(netMsg);
            }
            break;
        }
    }
    IEnumerator CreateRoom()
    {
        if(ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId }
        };

        yield return ApiManager.Instance.Post(
            "room/create",
            data,
            onSuccess: (res) =>
            {
                var parsed = JsonUtility.FromJson<ApiResponse.CreateRoomResponse>(res);
                RoomSession.Set(parsed.roomCode, parsed.hostId);
                var msg = new NetMsg
                {
                    type = "create_room",
                    playerId = UserSession.UserId,
                    roomCode = parsed.roomCode,
                    nickName = UserSession.Nickname,
                };
                string json = JsonConvert.SerializeObject(msg);
                WebSocketClient.Instance.Send(json);
            },
            onError: (err) =>
            {
                Debug.Log($"방 생성 실패: {err}");
            }
        );
        ui_lock = false;
    }
    IEnumerator JoinRoom()
    {
        if(ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId },
            { "roomCode", roomCodeInput.text.Trim().ToUpper() }
        };

        yield return ApiManager.Instance.Post(
            "room/join",
            data,
            onSuccess: (res) =>
            {
                var parsed = JsonUtility.FromJson<ApiResponse.JoinRoomResponse>(res);
                RoomSession.Set(parsed.roomCode, parsed.hostId);
                var msg = new NetMsg
                {
                    type = "join_room",
                    playerId = UserSession.UserId,
                    roomCode = parsed.roomCode,
                    nickName = UserSession.Nickname,
                };
                string json = JsonConvert.SerializeObject(msg);
                WebSocketClient.Instance.Send(json);
            },
            onError: (err) =>
            {
                Debug.Log($"입장 실패: {err}");
            }
        );
        ui_lock = false;
    }
}
