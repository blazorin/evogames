using Shared.Enums;

namespace Shared.Dto
{
    public class TransactionDto
    {
        public string TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
        public CoinType CoinType { get; set; }
        public decimal Amount { get; set; }
    }
}