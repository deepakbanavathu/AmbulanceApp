using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_BussinessLayer.Serivces
{
    public  class EmailOtpService
    {
        private readonly IConfiguration _config;
        public EmailOtpService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string toEmail,string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["Mailtrap:FromName"], _config["Mailtrap:FromEmamil"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Your AmbulanceApp OTP";
            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP is {otp}. It will expirte in 5 minutes."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Mailtrap:Host"], int.Parse(_config["Mailtrap:Port"]), false);
            await client.AuthenticateAsync(_config["Mailtrap:UserName"], _config["Mailtrap:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
