using UnityEngine;

public class CheatManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //치트입니다~
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            //server_restart
            var msg = new NetMsg
            {
                type = "restart",
                playerId = NetworkManager.Instance.MyUserId,
                isCheat = true,
            };
            NetworkManager.Instance.SendMsg(msg);
        }
    }
}
