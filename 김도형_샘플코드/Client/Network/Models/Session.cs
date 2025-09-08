using System.Collections.Generic;
using System.Linq;

public static class RoomSession
{
    public static string RoomCode { get; private set; }
    public static string HostId { get; private set; }
    public static List<RoomInfo> RoomInfos { get; private set; } = new List<RoomInfo>();

    public static void Init()
    {
        RoomCode = string.Empty;
        HostId = string.Empty;
        RoomInfos.Clear();
    }
    public static void Set(string code, string  hostId)
    {
        RoomCode = code;
        HostId = hostId;
    }

    public static void AddUser(RoomInfo roomInfo)
    {
        RoomInfos.Add(roomInfo);
        RoomInfos = RoomInfos.OrderByDescending(x => x.playerId == RoomSession.HostId).ToList();

    }
    public static void RemoveUser(string playerId)
    {
        RoomInfo roomInfo = RoomInfos.Find(x => x.playerId == playerId);
        if (roomInfo != null)
        {
            RoomInfos.Remove(roomInfo);
        }

        if (RoomInfos.Count <= 0)
        {
            RoomCode = string.Empty;
            HostId = string.Empty;
        }
        RoomInfos = RoomInfos.OrderByDescending(x => x.playerId == RoomSession.HostId).ToList();
    }
}
public class UserSession
{
    public static string UserId { get; private set; }
    public static string Nickname { get; private set; }

    public static void Set(string userId, string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }
}