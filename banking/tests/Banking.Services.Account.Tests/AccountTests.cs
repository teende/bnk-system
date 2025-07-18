using Xunit;
using System;
using System.Collections.Generic;
using Banking.Services.Account.Core.Domain.Entities;

namespace Banking.Services.Account.Tests
{
    public class AccountTests
    {
        [Fact]
        public void CreateAccount_SetsProperties()
        {
            var userId = Guid.NewGuid();
            var acc = new Account(userId, AccountType.Checking, 0, new[] { "USD", "RUB" });
            Assert.Equal(userId, acc.UserId);
            Assert.Equal(AccountType.Checking, acc.Type);
            Assert.Contains("USD", acc.Balances.Keys);
            Assert.Contains("RUB", acc.Balances.Keys);
            Assert.Equal(AccountStatus.Active, acc.Status);
        }

        [Fact]
        public void AddCurrency_AddsNewCurrency()
        {
            var acc = new Account(Guid.NewGuid(), AccountType.Checking);
            acc.AddCurrency("EUR");
            Assert.Contains("EUR", acc.Balances.Keys);
        }

        [Fact]
        public void RemoveCurrency_RemovesCurrencyWithZeroBalance()
        {
            var acc = new Account(Guid.NewGuid(), AccountType.Checking);
            acc.AddCurrency("EUR");
            acc.RemoveCurrency("EUR");
            Assert.DoesNotContain("EUR", acc.Balances.Keys);
        }

        [Fact]
        public void RemoveCurrency_ThrowsIfBalanceNotZero()
        {
            var acc = new Account(Guid.NewGuid(), AccountType.Checking);
            acc.AddCurrency("EUR");
            acc.UpdateBalance("EUR", 100);
            Assert.Throws<AccountBusinessException>(() => acc.RemoveCurrency("EUR"));
        }

        [Fact]
        public void UpdateBalance_ThrowsIfInsufficientFunds()
        {
            var acc = new Account(Guid.NewGuid(), AccountType.Checking);
            Assert.Throws<AccountBusinessException>(() => acc.UpdateBalance("USD", -100));
        }

        [Fact]
        public void Block_Unblock_Close_ChangeStatus()
        {
            var acc = new Account(Guid.NewGuid(), AccountType.Checking);
            acc.Block();
            Assert.Equal(AccountStatus.Blocked, acc.Status);
            acc.Unblock();
            Assert.Equal(AccountStatus.Active, acc.Status);
            acc.Close();
            Assert.Equal(AccountStatus.Closed, acc.Status);
        }
    }
} 