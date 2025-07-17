using Banking.Services.Account.Api.Dtos;

namespace Banking.Services.Account.Core.Application
{
    public interface IAccountService
    {
        TransferResultDto TransferMoney(TransferMoneyDto dto);
<<<<<<< HEAD
        Task<TransferResultDto> TransferMoneyAsync(TransferMoneyDto dto);    
        Task<AccountDto> GetAccountByIdAsync(Guid accountId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(Guid accountId);
        
=======
>>>>>>> feature/BNK-4-cards-and-accounts
    }
} 