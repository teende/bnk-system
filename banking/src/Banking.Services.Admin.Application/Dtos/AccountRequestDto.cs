using System.ComponentModel.DataAnnotations;

namespace Banking.Services.Admin.Application.Dtos
{
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
}