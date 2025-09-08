using UnityEngine;
using NativeWebSocket;
using System;
using System.Threading.Tasks;

public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; private set; }

    private WebSocket websocket;

    public event Action<string> OnMessageReceived;

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

    private string getUrl()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
         // 설정 불러오기
         var config = Resources.Load<ServerConfig>("ServerConfig");
         if (config == null)
         {
             Debug.LogError("ServerConfig.asset이 Resources 폴더에 없습니다!");
             return "";
         }
         string url = config.GetServerIP();
         return url;
#elif UNITY_ANDROID || UNITY_IOS
 return "wss://defensegamewebsocketserver.onrender.com/ws";
#endif
    }
    public async Task TryConnect()
    {
        string url = getUrl();
        Debug.Log($"웹소켓 연결 시도: {url}");
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket connected.");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"WebSocket error: {e}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed.");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            OnMessageReceived?.Invoke(message);
        };

        try
        {
            await websocket.Connect();
            Debug.Log("웹소켓 연결 완료!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"연결 실패: {ex.Message}");
        }
    }
    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public async void Send(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            if (websocket.State == WebSocketState.Open)
            {
                await websocket.SendText(message);
            }
            else
            {
                Debug.LogWarning("WebSocket not connected.");
            }    
        }
        else
        {
            Debug.LogError("websocket 인스턴스가 null입니다.");
        }
    }
}