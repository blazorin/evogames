using System;
using Shared.Enums;

namespace Shared.Dto
{
    public class BetDto
    {
        public string BetId { get; set; }

        public Games Game { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public BetStatus Status { get; set; }
    }
}