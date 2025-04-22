using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpPost("token")]
        public IActionResult Token([FromForm] TokenRequest request)
        {
            if (request.Grant_Type?.ToLower() != "client_credentials")
                return BadRequest("Unsupported grant_type");

            var system = _systemService.GetSystemByApiKey(request.Client_Id);
            if (system == null || !system.IsActive)
                return Unauthorized("Invalid client_id");

            if (string.IsNullOrEmpty(request.Client_Secret))
                return Unauthorized("Missing client_secret");

            if (!string.Equals(system.SharedSecret, request.Client_Secret, StringComparison.Ordinal))
                return Unauthorized("Invalid client_secret");

            var requestedScopes = (request.Scope ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var allowedScopes = (system.AllowedScopes ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (requestedScopes.Except(allowedScopes).Any())
                return Unauthorized("Requested scope is not allowed.");


            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenConfig.AccessTokenExpiryMinutes);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpiryDays);

            string accessToken = GenerateJwtToken(system, accessExpiresAt);
            string refreshToken = Guid.NewGuid().ToString();

            _tokenService.SaveOrUpdate(refreshToken, system.ExternalSystemGuid, refreshExpiresAt);

            return Ok(new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = _tokenConfig.AccessTokenExpiryMinutes * 60,
                refresh_token = refreshToken,
                scope = request.Scope ?? system.AllowedScopes
            });
        }
        private string GenerateJwtToken(ExternalSystem system, DateTime expiresAt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var base64Secret = _configuration["jwtTokenConfig:secret"];
            var keyBytes = Convert.FromBase64String(base64Secret);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            var encryptionBase64Key = _configuration["jwtTokenConfig:encryptionKey"];
            var encryptionKeyBytes = Convert.FromBase64String(encryptionBase64Key);
            var encryptingKey = new SymmetricSecurityKey(encryptionKeyBytes);

            var now = DateTime.UtcNow;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, system.ExternalSystemGuid.ToString()),
                new Claim(ClaimTypes.NameIdentifier, system.ExternalSystemGuid.ToString()),
                new Claim("systemType", "externalSystem"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("systemName", system.ApiKey),
                new Claim("scope", system.AllowedScopes ?? "read"),
                new Claim("aud", "uninet-api"),
                new Claim("iss", "auth.uninet")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                EncryptingCredentials = new EncryptingCredentials(encryptingKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

       
    }

    public class TokenRequest
    {
        public string Grant_Type { get; set; }
        public string Client_Id { get; set; }
        public string Client_Secret { get; set; }
        public string Scope { get; set; }
    }
}
