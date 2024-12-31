using GameApi.Data;
using GameApi.Models;
using GameApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UserDTO>> GetUsers()
        {
            var users = _db.Users.Select(u => u.ToDTO());
            return Ok(users);
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
        public IActionResult UpdatePartialUser(int id, JsonPatchDocument<User> patchDTO)
        {
            if (id <= 0 || patchDTO == null) { return BadRequest(); }

            var user = _db.Users.FirstOrDefault(u => u.Id == id);

            if (user == null) { return NotFound(); }

            patchDTO.ApplyTo(user, ModelState);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            _db.SaveChanges();
            return NoContent();
        }
    }
}
