using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Banking.Services.Account.Core.Domain.Entities;

namespace Banking.Services.Account.Core.Application
{
    public interface IAccountRepository
    {
        Task<Account> GetByIdAsync(Guid id);
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        Task<Account> AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account> GetByAccountNumberAsync(string accountNumber);
    }
} 