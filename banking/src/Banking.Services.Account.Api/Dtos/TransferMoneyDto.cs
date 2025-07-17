namespace Banking.Services.Account.Api.Dtos
{
    public class TransferMoneyDto
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public bool Success {get;set;}
        public string ErrorMessage { get; set; }
    }
} 
