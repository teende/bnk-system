using System;
using System.Collections.Generic;

namespace Banking.Services.Account.Api.Dtos
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
        public Dictionary<string, decimal> Balances { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
} 