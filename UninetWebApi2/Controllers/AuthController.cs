// AuthController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Uninet.APP.Interfaces;
using UninetWebApi2.Helpers;
using Uninet.Domain.Entities;
using Uninet.Domain.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRefreshTokenService _tokenService;
        private readonly IExternalSystemService _systemService;
        private readonly ExternalTokenConfig _tokenConfig;

        public AuthController(IRefreshTokenService tokenService, IExternalSystemService systemService, IOptions<ExternalTokenConfig> tokenConfig)
        {
            _tokenService = tokenService;
            _systemService = systemService;
            _tokenConfig = tokenConfig.Value;
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest("Refresh token is required.");

            // בודק אם ה-refresh token קיים במסד
            if (!_tokenService.Exists(request.RefreshToken, out Guid systemGuid))
                return Unauthorized("Invalid refresh token.");

            // שליפת פרטי מערכת חיצונית
            var system = _systemService.GetSystemByGuid(systemGuid);
            if (system == null || !system.IsActive)
                return Unauthorized("External system not found or inactive.");

            // הפקת Access Token חדש
            var accessExpiresAt = DateTime.UtcNow.AddMinutes(_tokenConfig.AccessTokenExpiryMinutes);
            var refreshExpiresAt = DateTime.UtcNow.AddDays(_tokenConfig.RefreshTokenExpiryDays);

            string newAccessToken = GenerateJwtToken(system, accessExpiresAt);
            string newRefreshToken = Guid.NewGuid().ToString();

            // עדכון ה-refresh token במסד
            _tokenService.SaveOrUpdate(newRefreshToken, systemGuid, refreshExpiresAt);
            _tokenService.Delete(request.RefreshToken); // מוחק את הישן

            return Ok(new
            {
                access_token = newAccessToken,
                token_type = "Bearer",
                expires_in = _tokenConfig.AccessTokenExpiryMinutes * 60,
                refresh_token = newRefreshToken,
                scope = system.AllowedScopes
            });
        }



        [HttpPost("token")]
        public IActionResult Token([FromForm] TokenRequest request)
        {
            if (request.Grant_Type?.ToLower() != "client_credentials")
                return BadRequest("Unsupported grant_type");

            var system = _systemService.GetSystemByApiKey(request.Client_Id);
            if (system == null || !system.IsActive)
                return Unauthorized("Invalid client_id");

            if (!string.Equals(system.SharedSecret, request.Client_Secret, StringComparison.Ordinal))
                return Unauthorized("Invalid client_secret");

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
            var signingKey = GetPrivateKey();

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
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256),
                //EncryptingCredentials = new EncryptingCredentials
                //(
                //    GetClientPublicKey(),
                //    SecurityAlgorithms.RsaOAEP,             // הצפנת מפתח סשן
                //    SecurityAlgorithms.Aes256CbcHmacSha512   // הצפנת התוכן
                //)
                //remarked by eyal temporary until i will have the public key that the client sent me for the encyption of the signing key 
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private RsaSecurityKey GetClientPublicKey()
        {
            using (var reader = new StreamReader("C:\\Users\\eyalber1.CLALIT\\Documents\\jwt-keys\\client_public_key.pem"))
            {
                var pem = reader.ReadToEnd();
                var rsa = RSA.Create();
                rsa.ImportFromPem(pem.ToCharArray());
                return new RsaSecurityKey(rsa);
            }
        }

        private RsaSecurityKey GetPrivateKey()
        {
            using (var reader = new StreamReader("C:\\Users\\eyalber1.CLALIT\\Documents\\jwt-keys\\private_key.pem"))
            {
                var pem = reader.ReadToEnd();
                var rsa = RSA.Create();
                rsa.ImportFromPem(pem.ToCharArray());
                return new RsaSecurityKey(rsa);
            }
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
