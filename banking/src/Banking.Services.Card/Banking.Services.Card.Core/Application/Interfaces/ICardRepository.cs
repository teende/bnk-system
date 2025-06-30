using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Banking.Services.Card.Core.Domain.Entities;

namespace Banking.Services.Card.Core.Application.Interfaces
{
    public interface ICardRepository
    {
        Task<Card> GetByIdAsync(Guid id);
        Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId);
        Task<int> GetUserCardCountAsync(Guid userId);
        Task<Card> AddAsync(Card card);
        Task UpdateAsync(Card card);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNumberAsync(string cardNumber);
    }