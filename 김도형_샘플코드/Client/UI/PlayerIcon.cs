using System;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket.Models;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    [SerializeField] private Image readyIcon;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private Image baseIcon;
    [SerializeField] private GameObject baseGameObject;
    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private Image HostIcon;
    [SerializeField] private Button KickButton;
    public string playerId;
    public string job_type;
    private bool ui_lock = false;

    private void Awake()
    {
        KickButton.onClick.AddListener(OnclickKickButton);
    }

    public void SetInfo(RoomInfo roomInfo)
    {
        playerId = roomInfo.playerId;
        nickNameText.text = roomInfo.nickName;
        job_type = roomInfo.jobType ?? "tank";
        SetJobIcon(job_type);
        SetReady(roomInfo.isReady);
        UpdateHostIcon();
    }

    public void UpdateHostIcon()
    {
        HostIcon.gameObject.SetActive(playerId == RoomSession.HostId);

        if (UserSession.UserId == RoomSession.HostId &&  UserSession.UserId != playerId)
        {
            //내가 호스트이고 다른 플레이어들의 아이콘이면 킥버튼을 켜줌
            SetKickButtonActive(true);
        }
        else
        {
            SetKickButtonActive(false);
        }
    }

    public void SetKickButtonActive(bool active)
    {
        KickButton.gameObject.SetActive(active);
    }

    public void OnclickKickButton()
    {
        if (ui_lock) return;
        ui_lock = true;
        //내가 호스트가 아니면
        //자기자신을 강퇴하려고 하면
        if (UserSession.UserId != RoomSession.HostId || UserSession.UserId == playerId)
        {
            ui_lock = false;
            return;
        }
        StartCoroutine(TryKickButtonCoroutine());
    }
    IEnumerator TryKickButtonCoroutine()
    {
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId},
            { "roomcode", RoomSession.RoomCode},
            { "targetUserID", playerId}
        };

        yield return ApiManager.Instance.Post(
            "room/kick",
            data,
            onSuccess: (res) =>
            {
                //웹소켓에서도 타겟유저id 제거
                var message = new
                {
                    type = "kick_user",
                    playerId = UserSession.UserId,
                    roomCode = RoomSession.RoomCode,
                    targetUserId = playerId,
                };
                string json = JsonConvert.SerializeObject(message);
                WebSocketClient.Instance.Send(json);
            },
            onError: (err) =>
            {
                Debug.Log($"강퇴를 하지 못했음.{playerId}");
            }
        );
        ui_lock = false;
    }
    
    public void SetReady(bool ready)
    {
        readyGameObject.SetActive(ready);
        baseGameObject.SetActive(!ready);
    }
    public void SetJobIcon(string job_tpye)
    {
        this.job_type = job_tpye;
        string capitalJob = FirstCharToUpper(job_tpye);
        string spritePath = $"Character/{capitalJob}/PROFILE_{capitalJob}";

        Sprite overrideSprite = Resources.Load<Sprite>(spritePath);
        if (overrideSprite != null)
        {
            baseIcon.enabled = true;
            baseIcon.sprite = overrideSprite;
            readyIcon.enabled = true;
            readyIcon.sprite = overrideSprite;
        }
        else
        {
            baseIcon.enabled = false;
            readyIcon.enabled = false;
        }
    }
    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }
}
