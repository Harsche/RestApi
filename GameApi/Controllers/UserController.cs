using System.Security.Claims;
using GameApi.Data;
using GameApi.Models;
using GameApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;

        public UserController(ILogger<UserController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserDTO>> GetUsers()
        {
            return Ok(_db.Users.ToList());
        }

        [HttpGet]
        [Route("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserDTO>> GetMyUser()
        {
            ArgumentNullException.ThrowIfNull(HttpContext.User.Identity?.Name);
            var name = HttpContext.User.Identity.Name;
            return Ok(_db.Users.First(u => u.Username == name));
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> GetUser(int id)
        {
            if (id <= 0) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            return user == null ? NotFound() : Ok(user.ToDTO());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> CreateUser([FromBody] UserDTO userDTO)
        {
            if (userDTO == null) { return BadRequest(); }

            if (userDTO.Id != 0) { return StatusCode(StatusCodes.Status500InternalServerError); }

            if (_db.Users.Any(u => u.Username.ToLower() == userDTO.Username.ToLower()))
            {
                ModelState.AddModelError("", "Username already exists.");
                return BadRequest(ModelState);
            }

            var user = userDTO.ToUser();
            user.CreateDate = DateTime.UtcNow;
            _db.Users.Add(user);
            _db.SaveChanges();

            return CreatedAtRoute("GetUser", new { id = user.Id }, user.ToDTO());
        }

        [HttpDelete("{id:int}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteUser(int id)
        {
            if (id <= 0) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null) { return NotFound(); }

            _db.Users.Remove(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateUser(int id, [FromBody] UserDTO userDTO)
        {
            if (id <= 0 || userDTO.Id != id) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null) { return NotFound(); }

            user.Username = userDTO.Username;
            user.Level = userDTO.Level;

            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialUser(int id, JsonPatchDocument<UserDTO> patchDTO)
        {
            if (id <= 0 || patchDTO == null) { return BadRequest(); }

            var user = _db.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);

            if (user == null) { return NotFound(); }

            var userDTO = user.ToDTO();
            patchDTO.ApplyTo(userDTO, ModelState);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var updatedUser = userDTO.ToUser();
            updatedUser.CreateDate = user.CreateDate;
            _db.Users.Update(updatedUser);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
