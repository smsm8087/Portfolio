using System;
using System.Collections.Generic;

namespace NativeWebSocket.Models
{
    public class ApiResponse
    {
        [Serializable]
        public class ErrorRes
        {
            public string message;
        }

        [Serializable]
        public class LoginResponse
        {
            public string userId;
            public string nickname;
        }
        [Serializable]
        public class CreateRoomResponse
        {
            public string roomCode;
            public string hostId;
        }

        [Serializable]
        public class JoinRoomResponse
        {
            public string roomCode;
            public string hostId;
        }
        [Serializable]
        public class RoomStatusResponse
        {
            public List<string> playerIds;
        }
        [Serializable]
        public class RoomOutResponse
        {
            public string hostId;
        }
    }
}