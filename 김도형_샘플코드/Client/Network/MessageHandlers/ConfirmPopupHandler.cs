using System;
using System.Collections;
using DataModels;
using UnityEngine;

public class ConfirmPopupHandler : MonoBehaviour, INetworkMessageHandler
{
    public string Type => "confirm";

    public void Handle(NetMsg msg)
    {
        PopupManager.Instance.ShowConfirmPopup(msg.message, msg.requestId);
    }
}