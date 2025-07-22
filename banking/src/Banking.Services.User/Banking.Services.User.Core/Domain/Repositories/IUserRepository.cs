using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Banking.Services.User.Core.Domain.Entities;

namespace Banking.Services.User.Core.Domain.Repositories;

public interface IUserRepository
{
    Task<Domain.Entities.User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Domain.Entities.User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(Domain.Entities.User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
} 