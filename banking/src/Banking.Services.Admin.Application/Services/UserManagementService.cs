using Banking.Services.Admin.Application.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Services.Admin.Application.Services
{

    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRequestRepository _accountRequestRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<UserManagementService> _logger;
        private const string StatusPending = "Pending";
        private const string StatusApproved = "Approved";
        private const string StatusRejected = "Rejected";

        public UserManagementService(
            IUserRepository userRepository,
            IAccountRequestRepository accountRequestRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _accountRequestRepository = accountRequestRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new ValidationException("A user with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.AddAsync(user);
            _logger.LogInformation("New user created: {Email}", user.Email);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role
            });
        }

        public async Task UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");

            user.UserName = dto.UserName ?? user.UserName;
            user.Email = dto.Email ?? user.Email;
            user.Role = dto.Role ?? user.Role;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User updated: {UserId}", userId);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _userRepository.DeleteAsync(userId);
            _logger.LogInformation("User deleted: {UserId}", userId);
        }

        public async Task<AuthResultDto> AuthenticateAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUserNameAsync(dto.UserName);
            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, dto.Password))
            {
                return new AuthResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            var token = _jwtTokenService.GenerateToken(user);
            return new AuthResultDto
            {
                Success = true,
                Token = token,
                UserName = user.UserName,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<bool> AuthorizeAsync(Guid userId, string requiredRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null && user.Role == requiredRole;
        }

        public async Task<RequestDto> CreateAccountRequestAsync(AccountRequestDto dto)
        {
            var request = new AccountRequest
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Email = dto.Email,
                RequestedRole = dto.RequestedRole,
                Comment = dto.Comment,
                Status = StatusPending,
                CreatedAt = DateTime.UtcNow
            };

            await _accountRequestRepository.AddAsync(request);
            _logger.LogInformation("Account request created: {Email}", dto.Email);

            return new RequestDto
            {
                Id = request.Id,
                UserName = request.UserName,
                Email = request.Email,
                RequestedRole = request.RequestedRole,
                Status = request.Status,
                Comment = request.Comment,
                CreatedAt = request.CreatedAt
            };
        }

        public async Task<IEnumerable<RequestDto>> GetAccountRequestsAsync()
        {
            var requests = await _accountRequestRepository.GetAllAsync();
            return requests.Select(r => new RequestDto
            {
                Id = r.Id,
                UserName = r.UserName,
                Email = r.Email,
                RequestedRole = r.RequestedRole,
                Status = r.Status,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                ProcessedAt = r.ProcessedAt,
                ProcessedBy = r.ProcessedBy,
                RejectReason = r.RejectReason
            });
        }

        public async Task ApproveAccountRequestAsync(Guid requestId)
        {
            var request = await _accountRequestRepository.GetByIdAsync(requestId);
            if (request == null) throw new Exception("Account request not found.");

            request.Status = StatusApproved;
            request.ProcessedAt = DateTime.UtcNow;
            request.ProcessedBy = "system";

            await _accountRequestRepository.UpdateAsync(request);
            _logger.LogInformation("Account request approved: {RequestId}", requestId);
        }

        public async Task RejectAccountRequestAsync(Guid requestId)
        {
            var request = await _accountRequestRepository.GetByIdAsync(requestId);
            if (request == null) throw new Exception("Account request not found.");

            request.Status = StatusRejected;
            request.ProcessedAt = DateTime.UtcNow;
            request.ProcessedBy = "system";

            await _accountRequestRepository.UpdateAsync(request);
            _logger.LogInformation("Account request rejected: {RequestId}", requestId);
        }
    }
}