using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using JetBrains.Annotations;
using Shared.Utils;

namespace Model.Data
{
    public class User
    {
        public string UserId { get; set; }

        [Required, StringLength(FieldLenghts.User.Name), MinLength(4)]
        public string Name { get; set; }

        [Required, StringLength(FieldLenghts.User.Mail), MinLength(6)]
        public string Email { get; set; }

        public string PasswordHashed { get; set; }

        [Required] public DateTime Birth { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastLogin { get; set; }

        [Required, StringLength(FieldLenghts.User.Country), MinLength(2)]
        public string Country { get; set; }

        public decimal Balance { get; set; }

        public float UnconfirmedDeposits { get; set; }
        public float UnconfirmedWithdraws { get; set; }

        public List<Claim> Claims { get; set; }
        public bool IsAdmin { get; set; }

        public List<UserLog> Logs { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Bet> Bets { get; set; }
    }
}