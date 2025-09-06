using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AmbulanceApp_BussinessLayer.Serivces;
using AmbulanceApp.Models;
using AmbulanceApp_BussinessLayer.Interfaces.Tokengeneration;
using AmbulanceApp_BussinessLayer.Interfaces.Authtication;
using Microsoft.AspNetCore.Authentication;
using AmbulanceApp.Models.DTO.Request;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace Ambulance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationServices _authService;

        public AuthController(IAuthenticationServices authService)
        {
            _authService = authService;
        }

        [HttpPost("send-otp/phone")]
        public async Task<IActionResult> SendPhoneOtp([FromBody] PhoneLoginRequest req) =>
            (await _authService.SendPhoneOtpAsync(req, HttpContext)) is var resp && resp.Success
            ?Ok(resp) : BadRequest(resp);

        [HttpPost("verify/phone")]
        public async Task<IActionResult> VerifyPhoneOtp([FromBody] VerifyOtpRequest req) =>
            (await _authService.VerifyPhoneOtpAsync(req, HttpContext)) is var resp && resp.Success
            ? Ok(resp) : BadRequest(resp);

        [HttpPost("login/email")]
        public async Task<IActionResult> LoginWithEmail([FromBody] EmailLoginRequest req) =>
            (await _authService.LoginWithEmailAsync(req, HttpContext)) is var resp  && resp.Success
            ?Ok(resp) : BadRequest(resp);

        [HttpPost("send-otp/email")]
        public async Task<IActionResult> SendEmailotp([FromBody] EmailOtpRequest req) =>
            (await _authService.SendEmailOtpAsync(req, HttpContext)) is var resp && resp.Success 
            ?Ok(resp) : BadRequest(resp);

        [HttpPost("verify/email")]
        public async Task<IActionResult> verifyEmailOtp([FromBody] VerifyOtpRequest req) =>
            (await _authService.VerifyEmailOtpAsync(req, HttpContext)) is var resp && resp.Success
            ?Ok(resp) : BadRequest(resp);

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] AmbulanceApp.Models.DTO.Request.RefreshRequest req) =>
            (await _authService.RefreshTokenAsync(req, HttpContext)) is var resp && resp.Success
            ?Ok(resp) : BadRequest(resp);        
    }
}
