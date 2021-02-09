using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Model.Data
{
    public class Bet
    {
        public string BetId { get; set; }

        [Required] public Games Game { get; set; }

        [Required] public decimal Amount { get; set; }

        [Required] public DateTime Date { get; set; }

        [Required] public BetStatus Status { get; set; }
        
        public string RejectedReason { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}