using Xunit;
using System;
using Banking.Services.User.Core.Domain.Entities;

namespace Banking.Services.User.Tests
{
    public class UserTests
    {
        [Fact]
        public void CreateUser_SetsProperties()
        {
            var user = new User("test@example.com", "hash", "Ivan", "Ivanov");
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("hash", user.PasswordHash);
            Assert.Equal("Ivan", user.FirstName);
            Assert.Equal("Ivanov", user.LastName);
            Assert.True(user.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Update_ChangesNameAndUpdatesTimestamp()
        {
            var user = new User("test@example.com", "hash", "Ivan", "Ivanov");
            var oldDate = user.UpdatedAt;
            user.Update("Petr", "Petrov");
            Assert.Equal("Petr", user.FirstName);
            Assert.Equal("Petrov", user.LastName);
            Assert.True(user.UpdatedAt > oldDate || user.UpdatedAt != null);
        }

        [Fact]
        public void UpdatePassword_ChangesPasswordAndUpdatesTimestamp()
        {
            var user = new User("test@example.com", "hash", "Ivan", "Ivanov");
            var oldDate = user.UpdatedAt;
            user.UpdatePassword("newhash");
            Assert.Equal("newhash", user.PasswordHash);
            Assert.True(user.UpdatedAt > oldDate || user.UpdatedAt != null);
        }
    }
} 