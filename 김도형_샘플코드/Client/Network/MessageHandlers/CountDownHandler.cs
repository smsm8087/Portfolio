using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class CountDownHandler : INetworkMessageHandler
{
    public string Type => "countdown";
    public void Handle(NetMsg msg)
    {
        //단순 카운트다운용 메시지 핸들러
        CenterText centerText = GameObject.Find("CenterText").GetComponent<CenterText>();
        centerText.UpdateText(msg.countDown, msg.message);
        //카운트 다운후에는 지금 스타트메세지만 출력되서 임시로 해놓음
        if (!String.IsNullOrEmpty(msg.message))
        {
            NetworkManager.Instance.ResetHp();
        }
    }
}