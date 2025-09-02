using AmbulanceApp.Models.Authentication;
using AmbulanceApp.Models.DTO.Request;
using AmbulanceApp_BussinessLayer.Interfaces.Authtication;
using AmbulanceApp_BussinessLayer.Interfaces.RedishCache;
using AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AmbulanceApp_BussinessLayer.Serivces
{
    public class AuthSerivce : IAuthenticationServices
    {
        private readonly IUserRespository _user;
        private readonly IJwtToken _jwtService;
        private readonly IRefreshToken _refreshTokenService;
        private readonly IRedisService _redisService;

        public AuthSerivce(IUserRespository users, IJwtToken jwtservice,
                           IRefreshToken refreshToken, IRedisService redis)
        {
            _users = users;
            _jwtService = jwtservice;
            _refreshTokenService = refreshToken;
            _redisService = redis;
        }

        public async Task<AuthResponse> SendPhoneOtpAsync(PhoneLoginRequest req, HttpContext context)
        {
            var otp = new Random(0).Next(100000, 999999).ToString();
            await _redisService.SetAsync($"otp:phone:{req.Phone}", otp,TimeSpan.FromMinutes(5));

            //Integrate SMS response

            return new AuthResponse { Success = true , Message="OTO Sent to phone"};
        }

        public async Task<AuthResponse> VerifyPhoneOtpAsync(VerifyOtpRequest req, HttpContext context)
        {
            var storedOtp = await _redisService.GetAsync($"otp:phone:{req.Contact}");
            if (storedOtp == null || storedOtp != req.Otp)
                return new AuthResponse { Success = false, Message = "Invalid Otp" };

            await _redisService.RemoveAsync($"otp:phone:{req.Contact}");

            var user = await _users.GetOrCreateByPhoneAsync(req.Contact);

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(),"User");

            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id.ToString());
            await _refreshTokenService.saveRefreshTokenAsync(refresh);

            return new AuthResponse { Success = true , Message="Login Successful", AccessToken = accessToken, RefreshToken= refresh.Token,ExpiresIn = 900 };
        }

        public async Task<AuthResponse> LoginWithEmailAsync(EmailLoginRequest req, HttpContext context)
        {
            var user = await _users.GetbyEmailAsync(req.Email);
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not registerd" };

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "invalid credentials" };

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), "User");

            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id.ToString());
            await _refreshTokenService.saveRefreshTokenAsync(refresh);

            return new AuthResponse { Success = true, Message = "Login Successful", AccessToken = accessToken, RefreshToken = refresh.Token, ExpiresIn = 900 };
        }

        public async Task<AuthResponse> SendEmailOtpAsync(EmailOtpRequest req, HttpContext context)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            await _redisService.SetAsync($"otp:email:{req.Email}",otp,TimeSpan.FromMinutes(5));

            //implement the integrated email service

            return new AuthResponse { Success = true, Message = "Otp sent to email" };
        }

        public async Task<AuthResponse> VerifyEmailOtpAsync(VerifyOtpRequest req, HttpContext httpContext)
        {
            var startOtp = await _redisService.GetAsync($"otp:email:{req.Contact}");
            if (startOtp == null || startOtp != req.Otp)
                return new AuthResponse { Success = false, Message = "Invalid Otp" };

            await _redisService.RemoveAsync($"otp:email:{req.Contact}");

            var user = await _users.GetOrCreateByEmailAsync(req.Contact);

            var accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(),"User");
            var refresh = _refreshTokenService.GenerateRefreshToken(user.Id.ToString());

            await _refreshTokenService.saveRefreshTokenAsync(refresh);

            return new AuthResponse { Success = true,Message="Login Sucessful",AccessToken=accessToken, RefreshToken=refresh.Token,ExpiresIn=900};
        }

        public async Task<AuthResponse> RefreshTokenAsync(AmbulanceApp.Models.DTO.Request.RefreshRequest req, HttpContext context)
        {
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
