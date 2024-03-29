using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using JetBrains.Annotations;
using Shared.Enums;
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

        public DateTime? Birth { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastLogin { get; set; }


        public string Country { get; set; }

        public Language Language { get; set; }

        public decimal Balance { get; set; }

        public float UnconfirmedDeposits { get; set; }
        public float UnconfirmedWithdraws { get; set; }

        public List<UserPerms> Perms { get; set; }
        //public List<string> EnabledTokens { get; set; }
        //public List<string> DisabledTokens { get; set; }

        public bool IsOwner { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }


        public List<UserLog> Logs { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Bet> Bets { get; set; }
    }
}