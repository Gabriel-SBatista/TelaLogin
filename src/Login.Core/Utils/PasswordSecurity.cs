using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Login.Core.Utils
{
    public static class PasswordSecurity
    {
        public static string VerifySecurity(string password)
        {
            int securityLevel = 0;

            if (password.Count() > 6)
            {
                if (Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]"))
                {
                    securityLevel++;
                }
                if (Regex.IsMatch(password, @"[A-Z]"))
                {
                    securityLevel++;
                }
                if (Regex.IsMatch(password, @"[a-z]"))
                {
                    securityLevel++;
                }
            }

            return StringSecurityLevel(securityLevel);
        }

        public static string StringSecurityLevel(int securityLevel)
        {
            switch (securityLevel)
            {
                case 1:
                    return "Fraca";
                case 2:
                    return "Média";
                case 3:
                    return "Forte";
                default:
                    return "Muito fraca";
            }
        }
    }
}
