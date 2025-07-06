namespace Banking.Services.Account.Api.Dtos
{
    public class TransferResultDto
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public decimal? SenderNewBalance {get; set;}
        public decimal? ReceiverNewBalance {get; set;}
        public Guid? TransactionId {get; set;}
        
    }
} 