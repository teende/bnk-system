using Banking.Services.Admin.Application.Dtos;

namespace Banking.Services.Admin.Application.Services
{
    public interface IUserManagementService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task UpdateUserAsync(Guid userId, UpdateUserDto dto);
        Task DeleteUserAsync(Guid userId);

        Task<AuthResultDto> AuthenticateAsync(LoginDto dto);
        Task<bool> AuthorizeAsync(Guid userId, string requiredRole);

        Task<RequestDto> CreateAccountRequestAsync(AccountRequestDto dto);
        Task<IEnumerable<RequestDto>> GetAccountRequestsAsync();
        Task ApproveAccountRequestAsync(Guid requestId);
        Task RejectAccountRequestAsync(Guid requestId);
    }
} 