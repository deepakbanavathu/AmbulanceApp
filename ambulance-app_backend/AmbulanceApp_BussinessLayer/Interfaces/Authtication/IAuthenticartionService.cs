using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmbulanceApp.Models.Authentication;
using AmbulanceApp.Models.DTO.Request;
using Microsoft.AspNetCore.Http;

namespace AmbulanceApp_BussinessLayer.Interfaces.Authtication
{
    public interface IAuthenticationServices
    {
        Task<AuthResponse> SendPhoneOtpAsync(PhoneLoginRequest req, HttpContext httpContext);
        Task<AuthResponse> VerifyPhoneOtpAsync(VerifyOtpRequest req, HttpContext httpContext);

        Task<AuthResponse> LoginWithEmailAsync(EmailLoginRequest req, HttpContext context);

        Task<AuthResponse> SendEmailOtpAsync(EmailOtpRequest req, HttpContext context);
        Task<AuthResponse> VerifyEmailOtpAsync(VerifyOtpRequest req, HttpContext httpContext);
        Task<AuthResponse> RefreshTokenAsync(AmbulanceApp.Models.DTO.Request.RefreshRequest req, HttpContext context);
    }
}
