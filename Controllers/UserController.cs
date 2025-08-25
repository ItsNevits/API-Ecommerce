using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersionNeutral]
    [ApiController]
    [Authorize(Roles = RolesName.Admin)]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            var usersDto = users.Adapt<IEnumerable<UserDto>>();
            return Ok(usersDto);
        }

        [HttpGet("{id:guid}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Invalid user ID");
            var user = _userRepository.GetUser(id);
            if (user == null) return NotFound("User not found");
            var userDto = user.Adapt<UserDto>();
            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost(Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(createUserDto.Username) || string.IsNullOrWhiteSpace(createUserDto.Password)) return BadRequest("Username and Password are required");
            if (_userRepository.UserExists(createUserDto.Username)) return BadRequest("Username already exists");

            var user = await _userRepository.CreateUser(createUserDto);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating user");
            }
            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        [AllowAnonymous]
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userRepository.Login(userLoginDto);
            if (user == null) return Unauthorized("Invalid username or password");
            return Ok(user);
        }
    }
}
