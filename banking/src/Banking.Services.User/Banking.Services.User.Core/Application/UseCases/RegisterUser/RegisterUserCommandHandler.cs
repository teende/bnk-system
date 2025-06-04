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
            var passwordHash = await _passwordHasher.HashPasswordAsync(request.Password);
            var user = new Domain.Entities.User(request.Email, request.FirstName, request.LastName);
            user.SetPasswordHash(passwordHash);

            await _userRepository.AddAsync(user, cancellationToken);

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