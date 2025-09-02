using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp.Models.DTO.Request
{
    public record VerifyOtpRequest(string Contact, string Otp);
}
