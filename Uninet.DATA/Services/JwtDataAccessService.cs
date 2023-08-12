using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;

namespace Uninet.DATA.Services
{
    public class JwtTokenConfig
    {
        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("audience")]
        public string Audience { get; set; }

        [JsonPropertyName("accessTokenExpiration")]
        public int AccessTokenExpiration { get; set; }

        [JsonPropertyName("refreshTokenExpiration")]
        public int RefreshTokenExpiration { get; set; }
    }
    public class RefreshToken
    {
        [JsonPropertyName("Userid")]
        public string Userid { get; set; }    // can be used for usage tracking
        // can optionally include other metadata, such as user agent, ip address, device name, and so on

        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
    public class JwtAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
    public class JwtDataAccessService : IJwtDataAccessService
    {   
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly ILogger<JwtDataAccessService> _logger;
        // private readonly IloginRepository _loginRepository;
        private readonly IRepository<UninetContext> _repository;

        private readonly byte[] _secret;
        public IConfiguration Configuration { get; }
        public JwtDataAccessService(ILogger<JwtDataAccessService> logger, IRepository<UninetContext> repository, IConfiguration configuration)//, IloginRepository loginRepository
        {
            _logger = logger;
            _repository = repository;
            Configuration = configuration;
            _secret = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("jwtTokenConfig:secret"));
            //_loginRepository = loginRepository;
        }

        public async Task<int> GetUserIdByRefreshToken(string refreshToken)
        {
            try
            {
                var Input = new
                {

                    RefreshToken = refreshToken
                };
                var result = _repository.ExecuteGetSP<GetUserIdByRefreshTokenResponse>(ConstUninetStoredprocedure.SP_GetUserIdByRefreshToken, Input);
                var res = result.ToList();
                return res[0].UserId;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private async Task<string> GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Configuration.GetValue<string>("jwtTokenConfig:issuer"),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = Configuration.GetValue<string>("jwtTokenConfig:audience"),
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }
        public async Task<string> GetRefreshTokenByUserId(int Userid)
        {
            try
            {
                var Input = new
                {

                    Userid = Userid
                };
                var result = _repository.ExecuteGetSP<RefreshResponse>(ConstUninetStoredprocedure.SP_GetRefreshToken, Input);
                var res = result.ToList();
                return res[0].RefreshTkn;

            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<int> SaveRefreshToken(int Userid, string refreshToken)
        {
            try
            {
                var RefreshInput = new
                {
                    RefreshTkn = refreshToken,
                    Userid = Userid
                };
                var result = _repository.ExecuteGetSP<SavedRefreshTokenResponse>(ConstUninetStoredprocedure.SP_UpdateInsertRefreshToken, RefreshInput);
                var res = result.ToList();
                return res[0].Success;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<string> GenerateRefreshToken(int Userid)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                //SP_SaveRefreshToken

                var refreshtoken= Convert.ToBase64String(randomNumber);

                var Input = new
                {
                    RefreshToken= refreshtoken,
                    userId = Userid
                };
                var result = _repository.ExecuteGetSP<SaveRefreshTokenResponse>(ConstUninetStoredprocedure.SP_SaveRefreshToken, Input);
                var res = result.ToList();
                if (res[0].res)
                {
                    return refreshtoken;
                }
                else
                {
                    return "";
                }
                    
                
                
            }
        }
        public async Task<string> GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtTokenConfig:secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: Configuration["jwtTokenConfig:issuer"],
                audience: Configuration["Tokens:audience"],
                claims: claims, //the user's claims, for example new Claim[] { new Claim(ClaimTypes.Name, "The username"), //... 
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(Convert.ToDouble(Configuration["jwtTokenConfig:accessTokenExpiration"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
        }


        //public async Task<string> GenerateAccessToken(string userId)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Convert.FromBase64String(Configuration.GetValue<string>("jwtTokenConfig:secret"));
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[] { new Claim("userId", userId) }),
        //        Issuer = Configuration.GetValue<string>("jwtTokenConfig:issuer"),
        //        Expires = DateTime.UtcNow.AddMinutes(Configuration.GetValue<double>("jwtTokenConfig:accessTokenExpiration")),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}



        public async Task<string> GenerateAccessTokenFromClaims(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtTokenConfig:secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                //issuer: Configuration["jwtTokenConfig:issuer"],
                audience: Configuration["Tokens:audience"],
                claims: claims, //the user's claims, for example new Claim[] { new Claim(ClaimTypes.Name, "The username"), //... 
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddSeconds(Convert.ToDouble(Configuration["jwtTokenConfig:accessTokenExpiration"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt); //the method is called WriteToken but returns a string
        }



        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwtTokenConfig:secret"])),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("REPLACE_WITH_BASE64_SECRET_FOR_LOCAL_ONLY")),
                ValidateLifetime = false, //eyal remark here we are saying that we don't care about the token's expiration date because we sent an access token that was already expired to get a new token based on a live refresh token

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // var tokentest = new System.IdentityModel.Tokens.JwtSecurityToken(jwt);
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }


    }
}
