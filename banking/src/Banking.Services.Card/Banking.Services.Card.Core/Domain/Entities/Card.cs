using System;
using System.ComponentModel.DataAnnotations;

namespace Banking.Services.Card.Core.Domain.Entities
{
    public class Card
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        [StringLength(16)]
        public string CardNumber { get; private set; }

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; private set; }

        [Required]
        public DateTime ExpiryDate { get; private set; }

        [Required]
        [StringLength(3)]
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

        public virtual Account Account { get; private set; }

        private Card() { } 

        public Card(string cardHolderName, CardType type, Guid accountId, decimal creditLimit = 0)
        {
            if (string.IsNullOrEmpty(cardHolderName))
                throw new ArgumentException("Card holder name cannot be empty", nameof(cardHolderName));

            if (type == CardType.Credit && creditLimit <= 0)
                throw new ArgumentException("Credit limit must be greater than 0 for credit cards", nameof(creditLimit));

            Id = Guid.NewGuid();
            CardNumber = GenerateCardNumber();
            CardHolderName = cardHolderName;
            CVV = GenerateCVV();
            ExpiryDate = DateTime.UtcNow.AddYears(5);
            Balance = 0;
            CreditLimit = type == CardType.Credit ? creditLimit : 0;
            Type = type;
            Status = CardStatus.Active;
            AccountId = accountId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateBalance(decimal amount)
        {
            ValidateCardOperation();

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

            if (Type == CardType.Credit && Balance + amount < -CreditLimit)
                throw new InvalidOperationException("Credit limit would be exceeded");

            Balance += amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Withdraw(decimal amount)
        {
            ValidateCardOperation();

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0", nameof(amount));

            if (!HasSufficientFunds(amount))
                throw new InvalidOperationException("Insufficient funds");

            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Block()
        {
            if (Status == CardStatus.Blocked)
                throw new InvalidOperationException("Card is already blocked");

            Status = CardStatus.Blocked;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Unblock()
        {
            if (Status == CardStatus.Active)
                throw new InvalidOperationException("Card is already active");

            if (IsExpired())
                throw new InvalidOperationException("Cannot unblock expired card");

            Status = CardStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiryDate;

        public bool HasSufficientFunds(decimal amount) => 
            Type == CardType.Debit ? Balance >= amount : Balance + CreditLimit >= amount;

        private void ValidateCardOperation()
        {
            if (Status != CardStatus.Active)
                throw new InvalidOperationException($"Card is {Status.ToString().ToLower()}");

            if (IsExpired())
                throw new InvalidOperationException("Card has expired");
        }

        private string GenerateCardNumber()
        {
            string prefix;
            switch (Type)
            {
                case CardType.Debit:
                    prefix = "400000"; // Visa
                    break;
                case CardType.Credit:
                    prefix = "510000"; // MasterCard
                    break;
                default:
                    throw new ArgumentException("Unknown card type");
            }

            var random = new Random();
            var digits = new int[9];
            for (int i = 0; i < 9; i++)
            {
                digits[i] = random.Next(0, 10);
            }

            string partialNumber = prefix + string.Join("", digits);
            int sum = 0;
            for (int i = 0; i < partialNumber.Length; i++)
            {
                int digit = int.Parse(partialNumber[i].ToString());
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9)
                    {
                        digit -= 9;
                    }
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