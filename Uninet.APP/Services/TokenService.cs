using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.APP.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Uninet.APP.Interfaces;
    using Uninet.DATA.Services.MultipleContext;
    using Uninet.DATA.Services;
    using Uninet.Domain.Interfaces;

    public class TokenService: ITokenService
    {
       
       
        private readonly int _accessTokenExpirationMinutes = 1;
        private readonly int _refreshTokenExpirationMinutes = 10;
        public IConfiguration Configuration { get; }
        
        public TokenService(ILogger<TokenService> logger, IConfiguration configuration)//, IloginRepository loginRepository
        {
            Configuration = configuration;
        }
        public async Task<string> GenerateAccessToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(Configuration.GetValue<string>("jwtTokenConfig:secret"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userId", userId) }),
                Issuer = Configuration.GetValue<string>("jwtTokenConfig:issuer"),
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();




            // save the refresh token in your database or wherever you're storing it
            return refreshToken;
        }



        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var key = Convert.FromBase64String(Configuration.GetValue<string>("jwtTokenConfig:secret"));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
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


        //public async Task<ClaimsPrincipal> GetPrincipalFromExpiredAccessToken(string token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Convert.FromBase64String(_secretKey);
        //    try
        //    {
        //        tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = true,
        //            ValidIssuer = _issuer,
        //            ValidateAudience = false,
        //            ValidateLifetime = false // we're validating the expired token, so this needs to be false
        //        }, out SecurityToken validatedToken);
        //        var jwtToken = (JwtSecurityToken)validatedToken;
        //        return new ClaimsPrincipal(jwtToken.Claims);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            // validate that the refresh token exists and is still valid (i.e. hasn't expired)
            // you can implement this however you like, depending on where you're storing the refresh tokens
            return true;
        }
    }

}
