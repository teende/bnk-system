using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Banking.Services.Card.Core.Domain.Entities
{
    public class Card
    {
        private const int CardNumberLength = 16;
        private const int PinLength = 4;
        private const int CvvLength = 3;
        private const int CardExpiryYears = 5;
        private readonly ILogger<Card> _logger;

        [Key]
        public Guid Id { get; private set; }
        [Required, StringLength(CardNumberLength)]
        public string CardNumber { get; private set; }
        [Required, StringLength(100)]
        public string CardHolderName { get; private set; }
        [Required]
        public DateTime ExpiryDate { get; private set; }
        [Required, StringLength(CvvLength)]
        public string CVV { get; private set; }
        [Required]
        public decimal Balance { get; private set; }
        [Required]
        public decimal CreditLimit { get; private set; }
        [Required]
        public CardType Type { get; private set; }
        [Required]
        public CardStatus Status { get; private set; }
        [Required]
        public DateTime CreatedAt { get; private set; }
        [Required]
        public DateTime UpdatedAt { get; private set; }
        [Required]
        public Guid AccountId { get; private set; }
        public string Currency { get; private set; }
        public string PinCode { get; private set; }
        public List<CardTransaction> Transactions { get; private set; }

        private Card() { }

        public Card(
            string cardHolderName,
            CardType type,
            Guid accountId,
            string currency,
            decimal creditLimit = 0,
            Func<string, bool> isCardNumberUnique = null,
            ILogger<Card> logger = null)
        {
            _logger = logger;
            ValidateCardHolderName(cardHolderName);
            ValidateCurrency(currency);
            ValidateCreditLimit(type, creditLimit);

            Id = Guid.NewGuid();
            CardNumber = GenerateUniqueCardNumber(type, isCardNumberUnique);
            CardHolderName = cardHolderName;
            CVV = GenerateCVV();
            ExpiryDate = DateTime.UtcNow.AddYears(CardExpiryYears);
            Balance = 0;
            CreditLimit = type == CardType.Credit ? creditLimit : 0;
            Type = type;
            Status = CardStatus.Active;
            AccountId = accountId;
            Currency = currency.ToUpper();
            PinCode = GeneratePinCode();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Transactions = new List<CardTransaction>();
            LogInfo($"Card created: {CardNumber} for account {AccountId}");
        }

        public void UpdateBalance(decimal amount, string description = null)
        {
            ValidateCardOperation();
            ValidateAmount(amount);
            if (Type == CardType.Credit && Balance + amount < -CreditLimit)
                throw new InvalidOperationException("Credit limit would be exceeded");
            if (Type == CardType.Debit && Balance + amount < 0)
                throw new InvalidOperationException("Insufficient funds");
            Balance += amount;
            UpdatedAt = DateTime.UtcNow;
            AddTransaction(amount, description);
            LogInfo($"Card {CardNumber} balance updated by {amount}, new balance {Balance}");
        }

        public void Withdraw(decimal amount, string description = null) => UpdateBalance(-amount, description);

        public void Block()
        {
            if (Status == CardStatus.Blocked)
                throw new InvalidOperationException("Card is already blocked");
            Status = CardStatus.Blocked;
            UpdatedAt = DateTime.UtcNow;
            LogInfo($"Card {CardNumber} blocked");
        }

        public void Unblock()
        {
            if (Status == CardStatus.Active)
                throw new InvalidOperationException("Card is already active");
            if (IsExpired())
                throw new InvalidOperationException("Cannot unblock expired card");
            Status = CardStatus.Active;
            UpdatedAt = DateTime.UtcNow;
            LogInfo($"Card {CardNumber} unblocked");
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiryDate;

        public bool HasSufficientFunds(decimal amount) =>
            Type == CardType.Debit ? Balance >= amount : Balance + CreditLimit >= amount;

        public void Reissue(Func<string, bool> isCardNumberUnique = null)
        {
            if (Status != CardStatus.Active)
                throw new InvalidOperationException("Only active cards can be reissued.");
            CardNumber = GenerateUniqueCardNumber(Type, isCardNumberUnique);
            CVV = GenerateCVV();
            ExpiryDate = DateTime.UtcNow.AddYears(CardExpiryYears);
            UpdatedAt = DateTime.UtcNow;
            LogInfo($"Card {CardNumber} reissued");
        }

        public void ChangePin(string newPin)
        {
            ValidatePin(newPin);
            PinCode = newPin;
            UpdatedAt = DateTime.UtcNow;
            LogInfo($"Card {CardNumber} PIN changed");
        }

        // --- Private helpers ---
        private void ValidateCardHolderName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Card holder name cannot be empty", nameof(name));
        }

        private void ValidateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));
        }

        private void ValidateCreditLimit(CardType type, decimal creditLimit)
        {
            if (type == CardType.Credit && creditLimit <= 0)
                throw new ArgumentException("Credit limit must be greater than 0 for credit cards", nameof(creditLimit));
        }

        private void ValidateAmount(decimal amount)
        {
            if (amount == 0)
                throw new ArgumentException("Amount must not be zero", nameof(amount));
        }

        private void ValidatePin(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin) || pin.Length != PinLength)
                throw new ArgumentException("PIN code must be 4 digits", nameof(pin));
            if (!int.TryParse(pin, out _))
                throw new ArgumentException("PIN code must be numeric", nameof(pin));
        }

        private void ValidateCardOperation()
        {
            if (Status != CardStatus.Active)
                throw new InvalidOperationException($"Card is {Status.ToString().ToLower()}");
            if (IsExpired())
                throw new InvalidOperationException("Card has expired");
        }

        private string GenerateUniqueCardNumber(CardType type, Func<string, bool> isCardNumberUnique)
        {
            string cardNum;
            do
            {
                cardNum = GenerateCardNumber(type);
            }
            while (isCardNumberUnique != null && !isCardNumberUnique(cardNum));
            return cardNum;
        }

        private string GenerateCardNumber(CardType type)
        {
            string prefix = type == CardType.Debit ? "400000" : "510000";
            var random = new Random();
            var digits = new int[9];
            for (int i = 0; i < 9; i++)
                digits[i] = random.Next(0, 10);
            string partialNumber = prefix + string.Join("", digits);
            int sum = 0;
            for (int i = 0; i < partialNumber.Length; i++)
            {
                int digit = int.Parse(partialNumber[i].ToString());
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }
                sum += digit;
            }
            int checkDigit = (10 - (sum % 10)) % 10;
            return partialNumber + checkDigit;
        }

        private string GenerateCVV()
        {
            var random = new Random();
            return random.Next(100, 1000).ToString();
        }

        private string GeneratePinCode()
        {
            var random = new Random();
            return random.Next(1000, 10000).ToString();
        }

        private void AddTransaction(decimal amount, string description)
        {
            Transactions.Add(new CardTransaction
            {
                Id = Guid.NewGuid(),
                CardId = this.Id,
                Amount = amount,
                Description = description,
                Date = DateTime.UtcNow
            });
        }

        private void LogInfo(string message)
        {
            _logger?.LogInformation(message);
        }
    }

    public class CardTransaction
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }

    public enum CardStatus
    {
        Active,
        Blocked,
        Expired
    }

    public enum CardType
    {
        Debit,
        Credit
    }
} 