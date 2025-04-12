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
        private const string JwtSecretKey = "SuperLongPrivateKeyYouControl123!";

        public AuthController(IRefreshTokenService tokenService, IExternalSystemService systemService, IOptions<ExternalTokenConfig> tokenConfig)
        {
            _tokenService = tokenService;
            _systemService = systemService;
            _tokenConfig = tokenConfig.Value;
        }

        [HttpPost("RegisterInit")]
        public IActionResult RegisterInit([FromBody] RegisterInitRequest request)
        {
            var system = _systemService.GetSystemById(request.ExternalSystemId);
            if (system == null || !system.IsActive)
                return Unauthorized("System not registered or inactive.");

            string tempToken = Guid.NewGuid().ToString();
            TempTokens[tempToken] = new TempTokenInfo
            {
                ExternalSystemId = request.ExternalSystemId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            return Ok(new { tempToken });
        }

        [HttpPost("LoginWithSecret")]
        public IActionResult LoginWithSecret([FromBody] LoginWithSecretRequest request)
        {
            if (!TempTokens.TryGetValue(request.TempToken, out var tempInfo))
                return Unauthorized("TempToken invalid");

            if (tempInfo.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("TempToken expired");

            if (tempInfo.ExternalSystemId != request.ExternalSystemId)
                return Unauthorized("Mismatched system");

            var system = _systemService.GetSystemById(request.ExternalSystemId);
            if (system == null || !system.IsActive)
                return Unauthorized("System not found");

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (Math.Abs(now - request.Timestamp) > system.ClockDriftToleranceSeconds)
                return Unauthorized("Timestamp out of range");

            string expected = HMACHelper.ComputeSHA256(system.ApiKey + request.Timestamp + system.SharedSecret);
            if (expected != request.DynamicSecret.ToLower())
                return Unauthorized("Invalid secret");

            TempTokens.TryRemove(request.TempToken, out _);

            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenConfig.AccessTokenExpiryMinutes);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpiryDays);

            string accessToken = GenerateJwtToken(system, accessExpiresAt);
            string refreshToken = Guid.NewGuid().ToString();

            _tokenService.SaveOrUpdate(refreshToken, system.ExternalSystemID, refreshExpiresAt);

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
            if (!_tokenService.Exists(request.RefreshToken, out var systemId))
                return Unauthorized("Invalid refresh token");

            var system = _systemService.GetSystemById(systemId);
            if (system == null || !system.IsActive)
                return Unauthorized("System inactive");

            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenConfig.AccessTokenExpiryMinutes);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpiryDays);

            string newAccessToken = GenerateJwtToken(system, accessExpiresAt);
            string newRefreshToken = Guid.NewGuid().ToString();

            _tokenService.SaveOrUpdate(newRefreshToken, systemId, refreshExpiresAt);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresInSeconds = _tokenConfig.AccessTokenExpiryMinutes * 60,
                ExpiresAtUtc = accessExpiresAt,
                ExpiresAtIsrael = TimeZoneInfo.ConvertTimeFromUtc(accessExpiresAt, TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time")).ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        private string GenerateJwtToken(ExternalSystem system, DateTime expiresAt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSecretKey);

            var claims = new[]
            {
                new Claim("systemId", system.ExternalSystemID.ToString()),
                new Claim("systemName", system.ApiKey),
                new Claim("scope", system.AllowedScopes ?? "read")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class RegisterInitRequest
    {
        public int ExternalSystemId { get; set; }
    }

    public class LoginWithSecretRequest
    {
        public int ExternalSystemId { get; set; }
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
        public int ExternalSystemId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
