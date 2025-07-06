using Banking.Services.Account.Api.Dtos;

namespace Banking.Services.Account.Core.Application
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<TransferResultDto> TransferMoneyAsync(TransferMoneyDto dto)
        {
            var sender = await _accountRepository.GetByIdAsync(dto.FromAccountId);
            var receiver = await _accountRepository.GetByIdAsync(dto.ToAccountId);

            if (sender == null || receiver == null)
                return new TransferResultDto { Success = false, ErrorMessage = "Invalid account IDs" };

            if (!sender.Balances.ContainsKey(dto.Currency) || sender.Balances[dto.Currency] < dto.Amount)
                return new TransferResultDto { Success = false, ErrorMessage = "Insufficient balance" };

            sender.UpdateBalance(dto.Currency, -dto.Amount, $"Transfer to {receiver.AccountNumber}");
            receiver.UpdateBalance(dto.Currency, dto.Amount, $"Transfer from {sender.AccountNumber}");

            await _accountRepository.UpdateAsync(sender);
            await _accountRepository.UpdateAsync(receiver);

            var transactionId = sender.Transactions.LastOrDefault()?.Id;

            return new TransferResultDto
            {
                Success = true,
                SenderNewBalance = sender.Balances[dto.Currency],
                ReceiverNewBalance = receiver.Balances[dto.Currency],
                TransactionId = transactionId
            };
        }
    }
}