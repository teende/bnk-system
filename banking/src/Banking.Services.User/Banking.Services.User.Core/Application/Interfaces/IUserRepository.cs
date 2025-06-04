using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Banking.Services.User.Core.Domain.Entities;

namespace Banking.Services.User.Core.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Banking.Services.User.Core.Domain.Entities.User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Banking.Services.User.Core.Domain.Entities.User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<Banking.Services.User.Core.Domain.Entities.User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Banking.Services.User.Core.Domain.Entities.User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(Banking.Services.User.Core.Domain.Entities.User user, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
} 