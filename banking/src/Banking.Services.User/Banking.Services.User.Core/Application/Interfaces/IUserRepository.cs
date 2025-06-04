using Banking.Services.User.Core.Domain.Entities;

namespace Banking.Services.User.Core.Application.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}