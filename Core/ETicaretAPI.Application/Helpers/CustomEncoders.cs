using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Helpers
{
    public static class CustomEncoders
    {
        public static string UrlEncode(this string value)
        {
            byte[] resetTokenBytes = Encoding.UTF8.GetBytes(value);

            string resetToken = WebEncoders.Base64UrlEncode(resetTokenBytes);

            return resetToken;
        }

        public static string UrlDecode(this string value)
        {
            byte[] resetTokenBytes = WebEncoders.Base64UrlDecode(value);

            var resetToken = Encoding.UTF8.GetString(resetTokenBytes);

            return resetToken;
        }
    }
}
