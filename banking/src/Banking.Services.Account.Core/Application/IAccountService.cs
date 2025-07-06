using Banking.Services.Account.Api.Dtos;

namespace Banking.Services.Account.Core.Application
{
    public interface IAccountService
    {
        TransferResultDto TransferMoney(TransferMoneyDto dto);
    }
} 