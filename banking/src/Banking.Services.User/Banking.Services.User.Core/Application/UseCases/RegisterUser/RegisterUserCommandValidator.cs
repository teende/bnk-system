using FluentValidation;
using Banking.Services.User.Core.Domain.Repositories;
using Banking.Services.User.Core.Interfaces;
using MediatR;

namespace Banking.Services.User.Core.Application.UseCases.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    

    public RegisterUserCommandValidator(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .Must(email => email.Contains("@")).WithMessage("Email must contain '@'")
            .Must(email => email.Split('@')[1].Contains(".")).WithMessage("Email must contain a domain")
            .MaximumLength(50).WithMessage("Email must not exceed 50 characters")
            .MustAsync(async (email, cancellationToken) => 
            {
                var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
                return user == null;
            }).WithMessage("Email is already registered");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(50).WithMessage("Password must not exceed 50 characters")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmation password cannot be empty")
            .Equal(x => x.Password).WithMessage("Passwords do not match");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters");
    }

    public async Task<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new Domain.Entities.User(request.Email, request.FirstName, request.LastName);
        user.SetPasswordHash(hashedPassword);

        await _userRepository.AddAsync(user, cancellationToken);

        return Unit.Value;
    }
}
  