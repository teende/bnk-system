using Microsoft.AspNetCore.Mvc;
using Banking.Services.Admin.Application;
using Banking.Services.Admin.Application.Dtos;

namespace Banking.Services.Admin.Api.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    public class UserManagementController : ControllerBase
    {

        private readonly IUserManagementService _userService;
        public UserManagementController(IUserManagementService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserByIdAsync), new { userId = user.Id }, user);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserAsync(Guid userId, [FromBody] UpdateUserDto dto)
        {
            await _userService.UpdateUserAsync(userId, dto);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            await _userService.DeleteUserAsync(userId);
            return NoContent();
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthResultDto>> AuthenticateAsync([FromBody] LoginDto dto)
        {
            var result = await _userService.AuthenticateAsync(dto);
            if (!result.Success)
                return Unauthorized(result);
            return Ok(result);
        }

        [HttpGet("authorize/{userId}")]
        public async Task<ActionResult<bool>> AuthorizeAsync(Guid userId, [FromQuery] string requiredRole)
        {
            var authorized = await _userService.AuthorizeAsync(userId, requiredRole);
            return Ok(authorized);
        }

        [HttpPost("account-request")]
        public async Task<ActionResult<RequestDto>> CreateAccountRequestAsync([FromBody] AccountRequestDto dto)
        {
            var request = await _userService.CreateAccountRequestAsync(dto);
            return Ok(request);
        }

        [HttpGet("account-requests")]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetAccountRequestsAsync()
        {
            var requests = await _userService.GetAccountRequestsAsync();
            return Ok(requests);
        }

        [HttpPost("account-requests/{requestId}/approve")]
        public async Task<IActionResult> ApproveAccountRequestAsync(Guid requestId)
        {
            await _userService.ApproveAccountRequestAsync(requestId);
            return NoContent();
        }

        [HttpPost("account-requests/{requestId}/reject")]
        public async Task<IActionResult> RejectAccountRequestAsync(Guid requestId)
        {
            await _userService.RejectAccountRequestAsync(requestId);
            return NoContent();
        }
    }
}