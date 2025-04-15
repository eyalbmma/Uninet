using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Uninet.APP.Interfaces;
using UninetWebApi2.Helpers;
using Uninet.Domain.Entities;
using Uninet.Domain.Models;
using Microsoft.Extensions.Options;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly ConcurrentDictionary<string, TempTokenInfo> TempTokens = new();
        private readonly IRefreshTokenService _tokenService;
        private readonly IExternalSystemService _systemService;
        private readonly ExternalTokenConfig _tokenConfig;
        private readonly IConfiguration _configuration;

        public AuthController(IRefreshTokenService tokenService, IExternalSystemService systemService, IOptions<ExternalTokenConfig> tokenConfig, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _systemService = systemService;
            _tokenConfig = tokenConfig.Value;
            _configuration = configuration;
        }


        [HttpPost("RegisterInit")]
        public IActionResult RegisterInit([FromBody] RegisterInitRequest request)
        {
            var system = _systemService.GetSystemById(request.ExternalSystemGuid);
            if (system == null || !system.IsActive)
                return Unauthorized("System not registered or inactive.");

            string tempToken = Guid.NewGuid().ToString();
            TempTokens[tempToken] = new TempTokenInfo
            {
                ExternalSystemGuid = request.ExternalSystemGuid,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            return Ok(new
            {
                message = "Success",
                systemGuid = request.ExternalSystemGuid,
                tempToken = tempToken // this is crucial to include
            });
        }


        [HttpPost("LoginWithSecret")]
        public IActionResult LoginWithSecret([FromBody] LoginWithSecretRequest request)
        {
            if (!TempTokens.TryGetValue(request.TempToken, out var tempInfo))
                return Unauthorized("TempToken invalid");

            if (tempInfo.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("TempToken expired");

            if (tempInfo.ExternalSystemGuid != request.ExternalSystemGuid)
                return Unauthorized("Mismatched system");

            var system = _systemService.GetSystemById(request.ExternalSystemGuid);
            if (system == null || !system.IsActive)
                return Unauthorized("System not found");

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (Math.Abs(now - request.Timestamp) > system.ClockDriftToleranceSeconds)//if the external system call this api after more than 60 seconds it wont work 
                return Unauthorized("Timestamp out of range");

            string expected = HMACHelper.ComputeSHA256(system.ApiKey + request.Timestamp + system.SharedSecret);
            if (expected != request.DynamicSecret.ToLower())
                return Unauthorized("Invalid secret");

            TempTokens.TryRemove(request.TempToken, out _);

            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenConfig.AccessTokenExpiryMinutes);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpiryDays);

            string accessToken = GenerateJwtToken(system, accessExpiresAt);
            string refreshToken = Guid.NewGuid().ToString();

            _tokenService.SaveOrUpdate(refreshToken, system.ExternalSystemGuid, refreshExpiresAt);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresInSeconds = _tokenConfig.AccessTokenExpiryMinutes * 60,
                ExpiresAtUtc = accessExpiresAt,
                ExpiresAtIsrael = TimeZoneInfo.ConvertTimeFromUtc(accessExpiresAt, TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time")).ToString("yyyy-MM-dd HH:mm:ss")
            });
        }


        [HttpPost("RefreshToken")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!_tokenService.Exists(request.RefreshToken, out var systemGuid))
                return Unauthorized("Invalid refresh token");

            var system = _systemService.GetSystemById(systemGuid);
            if (system == null || !system.IsActive)
                return Unauthorized("System inactive");

            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenConfig.AccessTokenExpiryMinutes);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpiryDays);

            string newAccessToken = GenerateJwtToken(system, accessExpiresAt);
            string newRefreshToken = Guid.NewGuid().ToString();


            _tokenService.SaveOrUpdate(newRefreshToken, system.ExternalSystemGuid, refreshExpiresAt);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresInSeconds = _tokenConfig.AccessTokenExpiryMinutes * 60,
                ExpiresAtUtc = accessExpiresAt,
                ExpiresAtIsrael = TimeZoneInfo.ConvertTimeFromUtc(
                    accessExpiresAt,
                    TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"))
                    .ToString("yyyy-MM-dd HH:mm:ss")
            });
        }










        private string GenerateJwtToken(ExternalSystem system, DateTime expiresAt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Get base64-encoded secret from config
            var base64Secret = _configuration["jwtTokenConfig:secret"];

            // Decode from base64
            var keyBytes = Convert.FromBase64String(base64Secret);
            var key = new SymmetricSecurityKey(keyBytes);

            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, system.ExternalSystemGuid.ToString()),
                    new Claim("systemGuid", system.ExternalSystemGuid.ToString()),
                    new Claim("systemName", system.ApiKey),
                    new Claim("scope", system.AllowedScopes ?? "read")
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



    }

    public class RegisterInitRequest
    {
        public Guid ExternalSystemGuid { get; set; }
    }

    public class LoginWithSecretRequest
    {
        public Guid ExternalSystemGuid { get; set; }
        public string TempToken { get; set; }
        public long Timestamp { get; set; }
        public string DynamicSecret { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class TempTokenInfo
    {
        public Guid ExternalSystemGuid { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
