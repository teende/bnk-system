using System.ComponentModel.DataAnnotations;

namespace Banking.Services.Admin.Application.Dtos
{
    public class UpdateUserDto
    {
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(30)]
        public string Role { get; set; }
    }
} 
