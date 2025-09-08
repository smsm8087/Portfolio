using System;
using System.Collections.Generic;
using CharacterSelect;
using DataModels;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;


public class StartGameHandler : INetworkMessageHandler
{
    private readonly Action callback;
    public string Type => "started_game";

    public StartGameHandler(Action callback)
    {
        this.callback = callback;
    }
    public void Handle(NetMsg msg)
    {
        callback?.Invoke();

        SceneLoader.Instance.LoadScene("IngameScene", () =>
        {
            //IngameScene이 로드 되었으므로
            var message = new
            {
                type = "scene_loaded",
                playerId = UserSession.UserId,
                roomCode = RoomSession.RoomCode,
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
        });
    }
}
public class CreateRoomHandler : INetworkMessageHandler
{
    public string Type => "room_created";
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            Debug.Log($"방 생성 성공! 코드: {RoomSession.RoomCode}");
            CharacterSelectSceneManager.Instance.Initialize();
        }); 
    }
}
public class JoinRoomHandler : INetworkMessageHandler
{
    public string Type => "room_joined";
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            Debug.Log($"입장 성공! 코드: {RoomSession.RoomCode}");
            CharacterSelectSceneManager.Instance.Initialize();
        }); 
    }
}
public class RoomInfoHandler : INetworkMessageHandler
{
    public string Type => "room_info";
    private readonly Action callback; 
    public RoomInfoHandler(Action callback)
    {
        this.callback = callback;
    }

    public void Handle(NetMsg msg)
    {
        RoomSession.Set(msg.roomCode, msg.hostId);
        List<RoomInfo> RoomInfos = msg.RoomInfos;
        foreach (var roomInfo in RoomInfos)
        {
            if (RoomSession.RoomInfos.Find(x=> x.playerId == roomInfo.playerId) != null) continue;
            RoomSession.AddUser(roomInfo);
        }

        if (RoomInfos.Count != RoomSession.RoomInfos.Count)
        {
            //추가를 했는데도 맞지않으면 룸인포에서 삭제해야함.
            for (int i = 0; i < RoomSession.RoomInfos.Count; i++)
            {
                RoomInfo roomInfo = RoomSession.RoomInfos[i];
                if (RoomInfos.Find(x => x.playerId == roomInfo.playerId) != null) continue;
                //방을 나간 유저 룸인포에서 삭제
                RoomSession.RemoveUser(roomInfo.playerId);
            }
        }
        callback?.Invoke();
    }
}
public class OutRoomHandler : INetworkMessageHandler
{
    public string Type => "out_room";
    private Action callback;

    public OutRoomHandler(Action callback)
    {
        this.callback =  callback;
    }
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("LobbyScene", () =>
        {
            Debug.Log($"방 나가기 성공!");
            RoomSession.Init();
            callback?.Invoke();
        }); 
    }
}
public class ChatRoomHandler : INetworkMessageHandler
{
    public string Type => "chat_room";

    private GameObject ChatUI;
    private Transform ChatParent;

    public ChatRoomHandler(GameObject ChatUI, Transform ChatParent)
    {
        this.ChatUI = ChatUI;
        this.ChatParent = ChatParent;
    }
    
    public void Handle(NetMsg msg)
    {
        AddChatMessage(msg.nickName, msg.message);
    }
    public void AddChatMessage(string nickname, string message)
    {
        GameObject newChat = GameObject.Instantiate(ChatUI, ChatParent);

        TextMeshProUGUI nicknameText = newChat.transform.Find("NickName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = newChat.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        nicknameText.text = nickname;
        messageText.text = message;
    }
}
public class SelectedCharacterHandler : INetworkMessageHandler
{
    public string Type => "selected_character";
    
    public void Handle(NetMsg msg)
    {
        CharacterSelectSceneManager.Instance.UpdatePlayerIcon(msg.playerId, msg.jobType);
        //캐릭터를 선택하면 준비가 된 것임
        CharacterSelectSceneManager.Instance.SetReady(msg.playerId, true);
    }
}
public class DeSelectedCharacterHandler : INetworkMessageHandler
{
    public string Type => "deselected_character";
    
    public void Handle(NetMsg msg)
    {
        CharacterSelectSceneManager.Instance.SetReady(msg.playerId, false);
    }
}

