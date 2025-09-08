using System;
using DataModels;
using UnityEngine;
using UI;
using System.Collections;

public class NoticePopupHandler : INetworkMessageHandler
{
    public string Type => "notice";
    public void Handle(NetMsg msg)
    {
        PopupManager.Instance.ShowNoticePopup(msg.message);
    }
}