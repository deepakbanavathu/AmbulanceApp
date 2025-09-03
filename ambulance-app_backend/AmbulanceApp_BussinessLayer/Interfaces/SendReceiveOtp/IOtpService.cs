using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_BussinessLayer.Interfaces.SendReceiveOtp
{
    public interface IOtpService
    {
        Task SendOtpAsync(string toEmail, string otp);
    }
}
