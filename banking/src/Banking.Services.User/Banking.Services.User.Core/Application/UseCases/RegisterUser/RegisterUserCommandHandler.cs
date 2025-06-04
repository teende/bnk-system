using System.Threading;
using System.Threading.Tasks;
using Banking.Services.User.Core.Application.Interfaces;
using Banking.Services.User.Core.Application.Models;
using Banking.Services.User.Core.Domain.Entities;
using MediatR;

namespace Banking.Services.User.Core.Application.UseCases.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt
            };
        }
    }
} 