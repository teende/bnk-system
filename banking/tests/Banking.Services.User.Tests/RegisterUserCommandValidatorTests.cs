using Xunit;
using Banking.Services.User.Core.Application.UseCases.RegisterUser;

namespace Banking.Services.User.Tests
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator _validator = new();

        [Fact]
        public void ValidCommand_PassesValidation()
        {
            var cmd = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "Ivan",
                LastName = "Ivanov"
            };
            var result = _validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        [InlineData("test@.com")]
        [InlineData("test@domain")]
        public void InvalidEmail_FailsValidation(string email)
        {
            var cmd = new RegisterUserCommand
            {
                Email = email,
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "Ivan",
                LastName = "Ivanov"
            };
            var result = _validator.Validate(cmd);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Theory]
        [InlineData("short")]
        [InlineData("nouppercase1!")]
        [InlineData("NOLOWERCASE1!")]
        [InlineData("NoNumber!")]
        [InlineData("NoSpecial1")]
        public void InvalidPassword_FailsValidation(string password)
        {
            var cmd = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = password,
                ConfirmPassword = password,
                FirstName = "Ivan",
                LastName = "Ivanov"
            };
            var result = _validator.Validate(cmd);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }

        [Fact]
        public void PasswordsDoNotMatch_FailsValidation()
        {
            var cmd = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password2!",
                FirstName = "Ivan",
                LastName = "Ivanov"
            };
            var result = _validator.Validate(cmd);
            Assert.Contains(result.Errors, e => e.PropertyName == "ConfirmPassword");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("A")]
        [InlineData("Имя123")] // недопустимые символы
        public void InvalidFirstName_FailsValidation(string firstName)
        {
            var cmd = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = firstName,
                LastName = "Ivanov"
            };
            var result = _validator.Validate(cmd);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("B")]
        [InlineData("Фамилия123")] // недопустимые символы
        public void InvalidLastName_FailsValidation(string lastName)
        {
            var cmd = new RegisterUserCommand
            {
                Email = "test@example.com",
                Password = "Password1!",
                ConfirmPassword = "Password1!",
                FirstName = "Ivan",
                LastName = lastName
            };
            var result = _validator.Validate(cmd);
            Assert.Contains(result.Errors, e => e.PropertyName == "LastName");
        }
    }
} 