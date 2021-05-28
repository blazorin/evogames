using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudBlazor;
using Shared.Utils;

namespace Client.Components.Helper.Validation
{
    public static class ClientValidationHelper
    {
        public static string CheckAge(DateTime? arg)
        {
            return arg != null && (DateTime.Compare(arg.Value.AddYears(18), DateTime.Now) == 1 ||
                                   arg.Value.Year < DateTime.Now.Year - 100)
                ? "You need to be major of age to play"
                : null;
        }

        public static IEnumerable<string> CheckUsername(string arg, bool usernameExists = false)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                yield return "Username is required";
                yield break;
            }

            if (BlackList.Names.Any(name => name == arg))
                yield return $"Username {arg} is not allowed";

            if (usernameExists)
                yield return "This username already exists";

            if (arg.Length < 4)
                yield return "Username too short";


            if (arg.Length > 15)
                yield return "Username too large";

            if (!arg.All(char.IsLetterOrDigit))
                yield return "Username does not accept symbols";
        }

        public static IEnumerable<string> CheckEmail(string arg, bool emailExists = false)
        {
            //EmailAddressAttribute attribute = new();

            if (string.IsNullOrWhiteSpace(arg))
            {
                yield return "Email is required";
                yield break;
            }

            if (!CheckValidEmail.Validate(arg))
                yield return "Invalid email";

            if (emailExists)
                yield return "This email already exists";
        }

        public static IEnumerable<string> PasswordStrength(string pw, MudTextField<string> passwordRepeatField)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required";
                yield break;
            }

            if (pw.Length < 8)
                yield return "Password must be at least of length 8";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Password must contain at least one capital letter";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Password must contain at least one lowercase letter";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Password must contain at least one digit";

            if (!string.IsNullOrEmpty(passwordRepeatField.Value))
                passwordRepeatField.Validate();
        }
        
        public static string PasswordMatch(string arg, MudTextField<string> passwordField)
        {
            return !string.IsNullOrEmpty(passwordField.Value) && passwordField.Value != arg ? "Passwords don't match" : null;
        }
    }
}