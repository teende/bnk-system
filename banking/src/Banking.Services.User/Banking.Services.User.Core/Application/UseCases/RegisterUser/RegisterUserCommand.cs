using Banking.Services.User.Core.Application.Models;
using MediatR;

namespace Banking.Services.User.Core.Application.UseCases.RegisterUser;

public class RegisterUserCommand : IRequest<UserResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
