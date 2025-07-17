using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Banking.Services.Admin.Application;
using Banking.Services.Admin.Application.Dtos;

namespace Banking.Services.Admin.Api.Controllers
{
    /// <summary>
    /// Controller for user management in the administrative panel
    /// </summary>
    [ApiController]
    [Route("api/admin/users")]
    [Produces("application/json")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userService;
        
        public UserManagementController(IUserManagementService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="dto">User creation data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetUserByIdAsync), new { userId = user.Id }, user);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while creating user.");
            }
        }

        /// <summary>
        /// Gets user by ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User data</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest("Invalid user ID.");

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while retrieving user.");
            }
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while retrieving users.");
            }
        }

        /// <summary>
        /// Updates user data
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="dto">Update data</param>
        /// <returns>Operation result</returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUserAsync(Guid userId, [FromBody] UpdateUserDto dto)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest("Invalid user ID.");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userService.UpdateUserAsync(userId, dto);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while updating user.");
            }
        }

        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Operation result</returns>
        [HttpDelete("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest("Invalid user ID.");

                await _userService.DeleteUserAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while deleting user.");
            }
        }

        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <param name="dto">Login data</param>
        /// <returns>Authentication result</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(AuthResultDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AuthResultDto>> AuthenticateAsync([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.AuthenticateAsync(dto);
                if (!result.Success)
                    return Unauthorized(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred during authentication.");
            }
        }

        /// <summary>
        /// Checks user authorization for specific role
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="requiredRole">Required role</param>
        /// <returns>Authorization result</returns>
        [HttpGet("authorize/{userId}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> AuthorizeAsync(Guid userId, [FromQuery] string requiredRole)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest("Invalid user ID.");

                if (string.IsNullOrWhiteSpace(requiredRole))
                    return BadRequest("Required role is missing.");

                var authorized = await _userService.AuthorizeAsync(userId, requiredRole);
                return Ok(authorized);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred during authorization.");
            }
        }

        /// <summary>
        /// Creates account creation request
        /// </summary>
        /// <param name="dto">Request data</param>
        /// <returns>Created request</returns>
        [HttpPost("account-request")]
        [ProducesResponseType(typeof(RequestDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<RequestDto>> CreateAccountRequestAsync([FromBody] AccountRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var request = await _userService.CreateAccountRequestAsync(dto);
                return Ok(request);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while creating account request.");
            }
        }

        /// <summary>
        /// Gets all account creation requests
        /// </summary>
        /// <returns>List of requests</returns>
        [HttpGet("account-requests")]
        [ProducesResponseType(typeof(IEnumerable<RequestDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetAccountRequestsAsync()
        {
            try
            {
                var requests = await _userService.GetAccountRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while retrieving account requests.");
            }
        }

        /// <summary>
        /// Approves account creation request
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <returns>Operation result</returns>
        [HttpPost("account-requests/{requestId}/approve")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ApproveAccountRequestAsync(Guid requestId)
        {
            try
            {
                if (requestId == Guid.Empty)
                    return BadRequest("Invalid request ID.");

                await _userService.ApproveAccountRequestAsync(requestId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while approving account request.");
            }
        }

        /// <summary>
        /// Rejects account creation request
        /// </summary>
        /// <param name="requestId">Request ID</param>
        /// <returns>Operation result</returns>
        [HttpPost("account-requests/{requestId}/reject")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RejectAccountRequestAsync(Guid requestId)
        {
            try
            {
                if (requestId == Guid.Empty)
                    return BadRequest("Invalid request ID.");

                await _userService.RejectAccountRequestAsync(requestId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while rejecting account request.");
            }
        }
    }
}