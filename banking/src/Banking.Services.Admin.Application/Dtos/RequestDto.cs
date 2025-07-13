using System;

namespace Banking.Services.Admin.Application.Dtos
{
    public class RequestDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RequestedRole { get; set; }
        public string Status { get; set; } 
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ProcessedBy { get; set; }
        public string RejectReason { get; set; }
    }
}