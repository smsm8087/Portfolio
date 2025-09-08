using NUnit.Framework.Internal;
using UnityEngine;

public enum ServerMode
{
    Local,
    TestLocal,
    MultiPlay
}
public enum ServerAPIMode
{
    Local,
    MultiPlay
}

[CreateAssetMenu(menuName = "Network/Server Config")]
public class ServerConfig : ScriptableObject
{
    public ServerMode serverMode = ServerMode.Local;
    public ServerAPIMode serverAPIMode = ServerAPIMode.Local;
    public string TestLocalIp = "59.12.167.192";
    public string renderServerIp = "wss://defensegamewebsocketserver.onrender.com/ws";        

    public int port = 5215;

    public string GetServerIP()
    {
        switch (serverMode)
        {
            case ServerMode.Local:
                return $"ws://127.0.0.1:{port}/ws";
            case ServerMode.TestLocal:
                return $"ws://{TestLocalIp}:{port}/ws";
            case ServerMode.MultiPlay:
                return renderServerIp;
        }
        return $"ws://{TestLocalIp}:{port}/ws";
    }
    public string GetApiServerIP()
    {
        switch (serverAPIMode)
        {
            case ServerAPIMode.Local:
                return $"http://127.0.0.1:5000/api";       
            case ServerAPIMode.MultiPlay:
                return $"http://{TestLocalIp}:5000/api";
        }
        return $"http://{TestLocalIp}:5000/api";
    }
}