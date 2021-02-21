using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;
using Shared.Utils;

namespace Model.Data
{
    public class Transaction
    {
        public string TransactionId { get; set; }

        [Required] public TransactionType TransactionType { get; set; }

        [Required] public CoinType CoinType { get; set; }

        [Required] public decimal Amount { get; set; }
        
        [Required] public DateTime Date { get; set; }

        [StringLength(FieldLenghts.Transaction.Note)]
        public string AdminNote { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}