using Login.Core.Requests;
using Login.Core.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Login.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var response = await _userService.GetUsersAsync(cancellationToken);

            if (!response.Success)
            {
                return BadRequest(response.Messages);
            }

            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegisterRequest userRequest, CancellationToken cancellationToken)
        {
            var response = await _userService.RegisterUserAsync(userRequest, cancellationToken);

            if (!response.Success)
            {
                return BadRequest(response.Messages);
            }

            return Ok(response.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody] UserLoginRequest userRequest, CancellationToken cancellationToken)
        {
            var response = await _userService.LoginUserAsync(userRequest, cancellationToken);

            if (!response.Success)
            {
                return BadRequest(response.Messages);
            }

            return Ok(response.Data);
        }

        [HttpPut("confirm-email/{userId:int}")]
        public async Task<IActionResult> Put(int userId, CancellationToken cancellationToken)
        {
            var response = await _userService.ConfirmEmailAsync(userId, cancellationToken);

            if (!response.Success)
            {
                return BadRequest(response.Messages);
            }

            return Ok(response.Data);
        }
    }
}
