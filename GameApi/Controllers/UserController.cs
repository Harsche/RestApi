using GameApi.Models;
using GameApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<UserDTO> GetUsers()
        {
            return
            [
                new UserDTO{Id = 1,Username = "Ocoloso"},
                new UserDTO{Id = 2,Username = "Ocoloso"},
            ];
        }

        [HttpGet("id")]
        public IEnumerable<UserDTO> GetUser(int id)
        {
            return
            [
                new UserDTO{Id = 1,Username = "Ocoloso"},
                new UserDTO{Id = 2,Username = "Ocoloso"},
            ];
        }
    }
}
