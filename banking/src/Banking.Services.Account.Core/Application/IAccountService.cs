using Banking.Services.Account.Api.Dtos;

namespace Banking.Services.Account.Core.Application
{
    public interface IAccountService
    {
        TransferResultDto TransferMoney(TransferMoneyDto dto);
        Task<TransferResultDto> TransferMoneyAsync(TransferMoneyDto dto);    
        Task<AccountDto> GetAccountByIdAsync(Guid accountId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(Guid accountId);
        
    }
} 