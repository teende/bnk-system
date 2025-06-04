using System;

namespace Banking.Services.User.Core.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        private User() { }

        public User(string email, string firstName, string lastName)
        {
            Id = Guid.NewGuid();
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 