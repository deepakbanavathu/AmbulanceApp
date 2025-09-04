using AmbulanceApp.Models.Authentication;
using AmbulanceApp.Models.DTO.Request;
using AmbulanceApp_BussinessLayer.Interfaces.Authtication;
using AmbulanceApp_BussinessLayer.Interfaces.RedishCache;
using AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration;
using Microsoft.AspNetCore.Http;
using AmbulanceApp_DBContext.DBContract;
using AmbulanceApp_BussinessLayer.Interfaces.SendReceiveOtp;
using AmbulanceApp_BussinessLayer.Helpers;
using AmbulanceApp_DBContext.Entities;

namespace AmbulanceApp_BussinessLayer.Serivces
{
    public class AuthSerivce : IAuthenticationServices
    {
        private readonly IUserRespository _user;
        private readonly IJwtToken _jwtService;
        private readonly IRefreshToken _refreshTokenService;
        private readonly IRedisService _redisService;
        private readonly IOtpService _emailOtpService;

        public AuthSerivce(IUserRespository users, IJwtToken jwtservice,
                           IRefreshToken refreshToken, IRedisService redis, IOtpService emailOtpService)
        {
            _user = users;
            _jwtService = jwtservice;
            _refreshTokenService = refreshToken;
            _redisService = redis;
            _emailOtpService = emailOtpService;
        }

        public async Task<AuthResponse> SendPhoneOtpAsync(PhoneLoginRequest req, HttpContext context)
        {
            if(string.IsNullOrWhiteSpace(req.Phone))
                return new AuthResponse { Success = false, Message = "Phone number is required" };

            var phone = UtilityHelpers.NormalizePhone(req.Phone);
            var otp = UtilityHelpers.GenerateOtp();
            await _redisService.SetAsync($"otp:phone:{req.Phone}", otp,TimeSpan.FromMinutes(5));

            //Integrate SMS response

            return new AuthResponse { Success = true , Message="OTO Sent to phone"};
        }

        public async Task<AuthResponse> VerifyPhoneOtpAsync(VerifyOtpRequest req, HttpContext context)
        {
            if(string.IsNullOrWhiteSpace(req.Contact) || string.IsNullOrWhiteSpace(req.Otp))
                return new AuthResponse { Success = false, Message = "Contact and Otp are required" };

            var phone = UtilityHelpers.NormalizePhone(req.Contact);
            var key = $"otp:phone:{phone}";
            var storedOtp = await _redisService.GetAsync(key);
            if (storedOtp is null || storedOtp != req.Otp)
                return new AuthResponse { Success = false, Message = "Invalid Otp" };

            await _redisService.RemoveAsync(key);

            var user = await _user.GetOrCreateByPhoneAsync(phone);

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(),"User");

            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id.ToString());
            await _refreshTokenService.saveRefreshTokenAsync(refresh);

            return new AuthResponse { Success = true , Message="Login Successful", AccessToken = accessToken, RefreshToken= refresh.Token,ExpiresIn = 900 };
        }

        public async Task<AuthResponse> LoginWithEmailAsync(EmailLoginRequest req, HttpContext context)
        {
            if(string.IsNullOrWhiteSpace(req.Email))
                return new AuthResponse { Success = false, Message = "Email is required" };
            var email = UtilityHelpers.NormalizeEmail(req.Email);
            var user = await _user.GetByEmailAsync(email);
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not registerd" };

            //if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            //    return new AuthResponse { Success = false, Message = "invalid credentials" };

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), "User");

            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id.ToString());
            await _refreshTokenService.saveRefreshTokenAsync(refresh);

            return new AuthResponse { Success = true, Message = "Login Successful", AccessToken = accessToken, RefreshToken = refresh.Token, ExpiresIn = 900 };
        }

        public async Task<AuthResponse> SendEmailOtpAsync(EmailOtpRequest req, HttpContext context)
        {
            if(string.IsNullOrWhiteSpace(req.Email))
                return new AuthResponse { Success = false, Message = "Email is required" };

            var email = UtilityHelpers.NormalizeEmail(req.Email);
            var otp = UtilityHelpers.GenerateOtp();
           
            await _redisService.SetAsync($"otp:email:{req.Email}",otp,TimeSpan.FromMinutes(5));

            //implement the integrated email service
            await _emailOtpService.SendOtpAsync(req.Email, otp);
            return new AuthResponse { Success = true, Message = "Otp sent to email" };
        }

        public async Task<AuthResponse> VerifyEmailOtpAsync(VerifyOtpRequest req, HttpContext httpContext)
        {
            if(string.IsNullOrWhiteSpace(req.Contact) || string.IsNullOrWhiteSpace(req.Otp))
                return new AuthResponse { Success = false, Message = "Contact and Otp are required" };
            var email = UtilityHelpers.NormalizeEmail(req.Contact);
            var key = $"otp:email:{email}";
            var startOtp = await _redisService.GetAsync(key);
            if (startOtp == null || startOtp != req.Otp)
                return new AuthResponse { Success = false, Message = "Invalid Otp" };

            await _redisService.RemoveAsync(key);

            var user = await _user.GetOrCreateByEmailAsync(req.Contact);
            await _user.SaveAsync();

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(),"User");
            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id.ToString());

            await _refreshTokenService.saveRefreshTokenAsync(refresh);

            return new AuthResponse { Success = true,Message="Login Sucessful",AccessToken=accessToken, RefreshToken=refresh.Token,ExpiresIn=900};
        }

        public async Task<AuthResponse> RefreshTokenAsync(AmbulanceApp.Models.DTO.Request.RefreshRequest req, HttpContext context)
        {
            if(string.IsNullOrWhiteSpace(req.RefreshToken))
                return new AuthResponse { Success = false, Message = "Refresh token is required" };

            var hashed = _refreshTokenService.HashToken(req.RefreshToken);
            var storedValue = await _redisService.GetAsync($"refresh:{hashed}");

            if (string.IsNullOrEmpty(storedValue))
                return new AuthResponse { Success = false, Message = "Invalid or expired refresh token" };

            var userId = System.Text.Json.JsonDocument.Parse(storedValue).RootElement.GetProperty("UserId").GetString();
            
            var valid = await _refreshTokenService.ValidateRefreshToken(userId, req.RefreshToken);

            if (!valid)
                return new AuthResponse { Success = false, Message = "Invalid or expired refresh token" };

            await _refreshTokenService.RevokeRefreshToken(userId, req.RefreshToken);

            var newrefresh = _refreshTokenService.GenerateRefreshToken(userId);
            await _refreshTokenService.saveRefreshTokenAsync(newrefresh);

            var newAccessToken = _jwtService.GenerateAccessToken(userId, "User");

            return new AuthResponse { Success = true, Message = "Token refreshed", AccessToken=newAccessToken, RefreshToken= newrefresh.Token, ExpiresIn = 900 };
        }
    }
}
