using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_BussinessLayer.Helpers
{
    public static class UtilityHelpers
    {
        public static string NormalizeEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
                return string.Empty;
            return email.Trim().ToLowerInvariant();
        }
        public static string NormalizePhone(string phone)
        {
            if(string.IsNullOrWhiteSpace(phone))
                return string.Empty;
            return phone.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        }

        /// <summary>
        /// Generate a cryptoGraphically Secure 6 digit Otp
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GenerateOtp()
        {
            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.Fill(bytes);
            uint value = BitConverter.ToUInt32(bytes);
            int code = (int)(value % 1_000_000);
            return code.ToString();
        }
    }
}
