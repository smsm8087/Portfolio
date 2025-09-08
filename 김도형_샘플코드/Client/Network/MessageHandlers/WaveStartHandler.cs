using System;
using System.Collections;
using UnityEngine;
using UI;

public class WaveStartHandler : INetworkMessageHandler
{
    public string Type => "wave_start";
    public void Handle(NetMsg msg)
    {
        // 웨이브 시작 토스트 메시지 표시
        string waveText = $"Wave {msg.wave}";
        CenterText centerText = GameObject.Find("CenterText").GetComponent<CenterText>();
        if (centerText != null)
        {
            centerText.UpdateText(-1, waveText);
        }
    }
}