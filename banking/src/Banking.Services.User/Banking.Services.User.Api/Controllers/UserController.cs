using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using FluentValidation;
using Banking.Services.User.Core.Application.UseCases.RegisterUser;
using System;
using System.Threading.Tasks;
using Banking.Services.User.Api.Dtos;
using Banking.Services.User.Core.Application.Models;

namespace Banking.Services.User.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        public UserController(ILogger<UserController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register([FromBody] CreateUserDto createUserDto)
        {
            var command = new RegisterUserCommand
            {
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                ConfirmPassword = createUserDto.ConfirmPassword,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName
            };

            try
            {
                var result = await _mediator.Send(command);
                _logger.LogInformation("Пользователь зарегистрирован: {Email}", command.Email);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (ApplicationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя");
                return StatusCode(500, "Произошла ошибка при регистрации");
            }
        }
    }
}
