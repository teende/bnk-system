using FluentValidation;
using Banking.Services.User.Core.Interfaces;

namespace Banking.Services.User.Core.Application.UseCases.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;
    
    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не может быть пустым")
            .EmailAddress().WithMessage("Неверный формат email")
            .Must(email => email.Contains("@")).WithMessage("Email должен содержать символ @")
            .Must(email => email.Split('@')[1].Contains(".")).WithMessage("Email должен содержать домен")
            .MaximumLength(50).WithMessage("Email не может быть длиннее 50 символов")
            .MustAsync(async (email, cancellationToken) => 
            {
                var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
                return user == null;
            }).WithMessage("Email уже зарегистрирован");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не может быть пустым")
            .MinimumLength(8).WithMessage("Пароль должен быть не менее 8 символов")
            .MaximumLength(50).WithMessage("Пароль не может быть длиннее 50 символов")
            .Must(password => password.Any(char.IsUpper)).WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Must(password => password.Any(char.IsLower)).WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Must(password => password.Any(char.IsDigit)).WithMessage("Пароль должен содержать хотя бы одну цифру");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Подтверждение пароля не может быть пустым")
            .Equal(x => x.Password).WithMessage("Пароли не совпадают");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя не может быть пустым")
            .Must(name => name.All(c => char.IsLetter(c) || c == ' ')).WithMessage("Имя может содержать только буквы и пробелы")
            .MinimumLength(3).WithMessage("Имя должно быть не менее 3 символов")
            .MaximumLength(50).WithMessage("Имя не может быть длиннее 50 символов");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия не может быть пустой")
            .Must(name => name.All(c => char.IsLetter(c) || c == ' ')).WithMessage("Фамилия может содержать только буквы и пробелы")
            .MinimumLength(3).WithMessage("Фамилия должна быть не менее 3 символов")
            .MaximumLength(50).WithMessage("Фамилия не может быть длиннее 50 символов");
    }
}
  