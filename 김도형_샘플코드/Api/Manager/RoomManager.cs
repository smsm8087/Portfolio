using System;
using System.Collections.Generic;
using System.Linq;

namespace DefenseGameWebServer.Manager
{
    public class RoomSession
    {
        public string RoomCode;
        public string HostUserId;
        public List<string> Participants = new();
        public DateTime CreatedAt = DateTime.UtcNow;
        public bool isStarted = false;
    }

    public class RoomManager
    {
        private readonly Dictionary<string, RoomSession> _rooms = new();
        private readonly Random _rand = new();

        public RoomSession CreateRoom(string hostUserId)
        {
            string code;
            do
            {
                code = GenerateRoomCode();
            } while (_rooms.ContainsKey(code));

            var room = new RoomSession
            {
                RoomCode = code,
                HostUserId = hostUserId
            };
            room.Participants.Add(hostUserId);
            _rooms[code] = room;

            Console.WriteLine($"[Room Created] {code} by user {hostUserId}");
            return room;
        }

        public bool TryJoinRoom(string code, string userId, out RoomSession room)
        {
            if (_rooms.TryGetValue(code, out room))
            {
                room.Participants.Add(userId);
                Console.WriteLine($"[Room Join] user {userId} joined {code}");
                return true;
            }
            return false;
        }
        public bool TryOutRoom(string code, string userId, out RoomSession room)
        {
            if (_rooms.TryGetValue(code, out room))
            {
                if (!room.Participants.Contains(userId)) return false;
                room.Participants.Remove(userId);
                Console.WriteLine($"[Room Out] user {userId} out {code}");

                if (room.Participants.Count <= 0)
                {
                    _rooms.Remove(code);
                    room = null;
                    Console.WriteLine($"[Room Remove] user {userId} out {code}");
                } else
                {
                    //호스트 이동
                    if (room.HostUserId == userId)
                    {
                        room.HostUserId = room.Participants[0];
                        Console.WriteLine($"[Room HostMove] user {userId} out {code}");
                    }
                }
                    return true;
            }
            return false;
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(c => c[_rand.Next(c.Length)]).ToArray());
        }
        public List<string> GetParticipants(string roomCode)
        {
            if (!_rooms.ContainsKey(roomCode))
                throw new ArgumentException("Room does not exist.", nameof(roomCode));
            return _rooms[roomCode].Participants;
        }
        public RoomSession GetRoom(string roomCode)
        {
            if (!_rooms.TryGetValue(roomCode, out var room))
                throw new ArgumentException("Room does not exist.", nameof(roomCode));
            return room;
        }
    }
}