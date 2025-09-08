using DefenseGameWebServer.Manager;
using Microsoft.AspNetCore.Mvc;

namespace DefenseGameApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private static readonly RoomManager _roomManager = new(); // 싱글톤처럼 사용

        [HttpPost("create")]
        public IActionResult Create([FromForm] string userId)
        {
            var room = _roomManager.CreateRoom(userId);
            return Ok(new { roomCode = room.RoomCode, hostId = room.HostUserId });
        }

        [HttpPost("join")]
        public IActionResult Join([FromForm] string userId, [FromForm] string roomCode)
        {
            if (_roomManager.TryJoinRoom(roomCode, userId, out var room))
            {
                return Ok(new { roomCode = room.RoomCode, hostId = room.HostUserId });
            }
            return NotFound(new { message = "방이 존재하지 않습니다." });
        }
        [HttpPost("out")]
        public IActionResult Out([FromForm] string userId, [FromForm] string roomCode)
        {
            if (_roomManager.TryOutRoom(roomCode, userId, out var room))
            {
                if(room == null)
                {
                    return Ok();
                }
                else
                {
                    return Ok(new { hostId = room.HostUserId});
                }
            }
            return NotFound(new { message = "방에서 나가지 못했습니다" });
        }
        [HttpPost("status")]
        public IActionResult GetRoomPaticipantCount([FromForm] string roomCode)
        {
            //게임시작 버튼 누른거임
            RoomSession room = _roomManager.GetRoom(roomCode);
            if(room == null)
            {
                return NotFound(new { message = "방이 존재하지 않습니다." });
            }

            List<string> participants = _roomManager.GetParticipants(roomCode);
            if (participants.Count <= 0)
            {
                return NotFound(new { message = "방이 존재하지 않습니다." });
            }

            // 게임 시작 상태로 변경
            room.isStarted = true; 
            return Ok();
        }
        [HttpPost("kick")]
        public IActionResult KickUserFromRoom([FromForm] string userId, [FromForm] string roomCode, [FromForm] string targetUserID )
        {
            RoomSession room = _roomManager.GetRoom(roomCode);
            if (room == null)
            {
                return NotFound(new { message = "방이 존재하지 않습니다." });
            }
            if(userId != room.HostUserId)
            {
                return BadRequest(new { message = "호스트만 유저를 추방할 수 있습니다." });
            }
            if( userId == targetUserID)
            {
                return BadRequest(new { message = "자기 자신을 추방할 수 없습니다." });
            }
            _roomManager.TryOutRoom(roomCode, targetUserID, out room);
            return Ok();
        }
    }
}