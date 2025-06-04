using Banking.Services.User.Core.Application.Models;
using MediatR;

namespace Banking.Services.User.Core.Application.UseCases.RegisterUser;

public record RegisterUserCommand : IRequest<UserResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}
