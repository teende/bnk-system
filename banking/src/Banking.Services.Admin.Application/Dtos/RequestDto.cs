using System.ComponentModel.DataAnnotations;

namespace Banking.Services.Admin.Application.Dtos
{
    public class LoginDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } // JWT или другой токен
        public string UserName { get; set; }
        public string Role { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class AccountRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        public string RequestedRole { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
    }

    public class RequestDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RequestedRole { get; set; }
        public string Status { get; set; } // Например: Pending, Approved, Rejected
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ProcessedBy { get; set; }
        public string RejectReason { get; set; }
    }
}