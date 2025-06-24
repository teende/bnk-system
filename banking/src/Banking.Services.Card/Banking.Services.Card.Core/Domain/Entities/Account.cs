using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Banking.Services.Card.Core.Domain.Entities
{
    public class AccountBusinessException : Exception
    {
        public AccountBusinessException(string message) : base(message) { }
    }

    public class Account
    {
        private const int AccountNumberLength = 20;
        private static readonly HashSet<string> SupportedCurrencies = new HashSet<string> { "RUB", "USD", "EUR" };
        private static readonly decimal DefaultCreditLimit = -10000m;

        private readonly ILogger<Account> _logger;

        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid UserId { get; private set; }

        [Required]
        [StringLength(AccountNumberLength)]
        public string AccountNumber { get; private set; }

        public Dictionary<string, decimal> Balances { get; private set; }
        public Dictionary<string, decimal> CurrencyLimits { get; private set; }
        public List<AccountTransaction> Transactions { get; private set; }

        [Required]
        public AccountType Type { get; private set; }

        [Required]
        public AccountStatus Status { get; private set; }

        public decimal InterestRate { get; private set; }
        public DateTime? LastInterestCalculationDate { get; private set; }

        [Required]
        public DateTime CreatedAt { get; private set; }

        [Required]
        public DateTime UpdatedAt { get; private set; }

        public virtual ICollection<Card> Cards { get; private set; }

        private Account()
        {
            Balances = new Dictionary<string, decimal>();
            CurrencyLimits = new Dictionary<string, decimal>();
            Transactions = new List<AccountTransaction>();
            Cards = new List<Card>();
        }

        public Account(
            Guid userId,
            AccountType type,
            decimal interestRate = 0,
            IEnumerable<string> initialCurrencies = null,
            Func<Guid, bool> userExists = null,
            Func<string, bool> isAccountNumberUnique = null,
            ILogger<Account> logger = null)
        {
            _logger = logger;
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id is required.");
            if (userExists != null && !userExists(userId))
                throw new AccountBusinessException("User does not exist.");
            if (type == AccountType.Savings && interestRate <= 0)
                throw new AccountBusinessException("Interest rate must be greater than 0 for savings accounts.");
            if (type == AccountType.Credit && interestRate <= 0)
                throw new AccountBusinessException("Interest rate must be greater than 0 for credit accounts.");
            if (interestRate < 0)
                throw new AccountBusinessException("Interest rate cannot be negative.");

            UserId = userId;
            Id = Guid.NewGuid();
            Type = type;
            Status = AccountStatus.Active;
            InterestRate = interestRate;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Cards = new List<Card>();
            Balances = new Dictionary<string, decimal>();
            CurrencyLimits = new Dictionary<string, decimal>();
            Transactions = new List<AccountTransaction>();

            if (initialCurrencies != null)
            {
                foreach (var currency in initialCurrencies)
                {
                    AddCurrency(currency);
                }
            }
            else
            {
                AddCurrency("USD");
            }

            string accNum;
            do
            {
                accNum = GenerateAccountNumber();
            }
            while (isAccountNumberUnique != null && !isAccountNumberUnique(accNum));
            AccountNumber = accNum;
            _logger?.LogInformation($"Account created: {AccountNumber} for user {UserId}");
        }

        public void AddCurrency(string currency, decimal? limit = null)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required.");
            currency = currency.ToUpper();
            if (!SupportedCurrencies.Contains(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not supported.");
            if (Balances.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' already exists for this account.");
            Balances[currency] = 0m;
            CurrencyLimits[currency] = limit ?? (Type == AccountType.Credit ? DefaultCreditLimit : 0m);
            UpdatedAt = DateTime.UtcNow;
            _logger?.LogInformation($"Currency {currency} added to account {AccountNumber}");
        }

        public void RemoveCurrency(string currency)
        {
            currency = currency.ToUpper();
            if (!Balances.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not available for this account.");
            if (Balances[currency] != 0)
                throw new AccountBusinessException($"Cannot remove currency '{currency}' with non-zero balance.");
            Balances.Remove(currency);
            CurrencyLimits.Remove(currency);
            UpdatedAt = DateTime.UtcNow;
            _logger?.LogInformation($"Currency {currency} removed from account {AccountNumber}");
        }

        public void UpdateBalance(string currency, decimal amount, string description = null)
        {
            currency = currency.ToUpper();
            if (!Balances.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not available for this account.");
            if (Status != AccountStatus.Active)
                throw new AccountBusinessException("Operations are allowed only for active accounts.");
            var limit = CurrencyLimits.ContainsKey(currency) ? CurrencyLimits[currency] : (Type == AccountType.Credit ? DefaultCreditLimit : 0m);
            if (Type != AccountType.Credit && Balances[currency] + amount < 0)
                throw new AccountBusinessException("Insufficient funds.");
            if (Type == AccountType.Credit && Balances[currency] + amount < limit)
                throw new AccountBusinessException("Credit limit exceeded.");
            Balances[currency] += amount;
            UpdatedAt = DateTime.UtcNow;
            var transaction = new AccountTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = this.Id,
                Currency = currency,
                Amount = amount,
                Description = description,
                Date = DateTime.UtcNow
            };
            Transactions.Add(transaction);
            _logger?.LogInformation($"Balance updated for account {AccountNumber}, currency {currency}, amount {amount}, new balance {Balances[currency]}");
        }

        public decimal GetBalance(string currency)
        {
            currency = currency.ToUpper();
            if (!Balances.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not available for this account.");
            return Balances[currency];
        }

        public Dictionary<string, decimal> GetAllBalances()
        {
            return new Dictionary<string, decimal>(Balances);
        }

        public void SetCurrencyLimit(string currency, decimal limit)
        {
            currency = currency.ToUpper();
            if (!Balances.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not available for this account.");
            CurrencyLimits[currency] = limit;
            UpdatedAt = DateTime.UtcNow;
            _logger?.LogInformation($"Limit set for currency {currency} in account {AccountNumber}: {limit}");
        }

        public decimal GetCurrencyLimit(string currency)
        {
            currency = currency.ToUpper();
            if (!CurrencyLimits.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not available for this account.");
            return CurrencyLimits[currency];
        }

        public void CalculateInterest(string currency)
        {
            currency = currency.ToUpper();
            if (!Balances.ContainsKey(currency))
                throw new AccountBusinessException($"Currency '{currency}' is not available for this account.");
            if (Type == AccountType.Savings)
            {
                if (Balances[currency] < 0)
                    throw new AccountBusinessException("Savings account cannot have a negative balance.");
                if (LastInterestCalculationDate == null || DateTime.UtcNow >= LastInterestCalculationDate.Value.AddDays(30))
                {
                    var interest = Balances[currency] * InterestRate / 100;
                    Balances[currency] += interest;
                    LastInterestCalculationDate = DateTime.UtcNow;
                    UpdatedAt = DateTime.UtcNow;
                    _logger?.LogInformation($"Interest calculated for account {AccountNumber}, currency {currency}, interest {interest}");
                }
            }
            else if (Type == AccountType.Credit)
            {
                if (Balances[currency] < 0)
                    throw new AccountBusinessException("Credit account cannot have a negative balance.");
                if (LastInterestCalculationDate == null || DateTime.UtcNow >= LastInterestCalculationDate.Value.AddDays(30))
                {
                    var interest = Balances[currency] * InterestRate / 100;
                    Balances[currency] += interest;
                    LastInterestCalculationDate = DateTime.UtcNow;
                    UpdatedAt = DateTime.UtcNow;
                    _logger?.LogInformation($"Interest calculated for account {AccountNumber}, currency {currency}, interest {interest}");
                }
            }
        }

        public void Block()
        {
            if (Status == AccountStatus.Blocked)
                throw new AccountBusinessException("Account is already blocked.");
            if (Status == AccountStatus.Closed)
                throw new AccountBusinessException("Cannot block a closed account.");
            Status = AccountStatus.Blocked;
            UpdatedAt = DateTime.UtcNow;
            _logger?.LogInformation($"Account {AccountNumber} blocked");
        }

        public void Unblock()
        {
            if (Status != AccountStatus.Blocked)
                throw new AccountBusinessException("Account is not blocked.");
            Status = AccountStatus.Active;
            UpdatedAt = DateTime.UtcNow;
            _logger?.LogInformation($"Account {AccountNumber} unblocked");
        }

        public void Close()
        {
            if (Status == AccountStatus.Closed)
                throw new AccountBusinessException("Account is already closed.");
            foreach (var bal in Balances.Values)
                if (bal != 0)
                    throw new AccountBusinessException("Cannot close account with non-zero balance.");
            Status = AccountStatus.Closed;
            UpdatedAt = DateTime.UtcNow;
            _logger?.LogInformation($"Account {AccountNumber} closed");
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            string number;
            do
            {
                number = "";
                for (int i = 0; i < AccountNumberLength; i++)
                    number += random.Next(0, 10).ToString();
            }
            while (!Regex.IsMatch(number, @"^\d{20}$"));
            return number;
        }
    }

    public class AccountTransaction
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }

    public enum AccountType
    {
        Checking,
        Savings,
        Credit
    }

    public enum AccountStatus
    {
        Active,
        Blocked,
        Closed
    }
}