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
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.DATA.Services;
using Uninet.Domain.Interfaces;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Models;

namespace Uninet.APP.Services
{
    public  class jwtAppService: IjwtAppService
    {
        private readonly JwtTokenConfig _jwtTokenConfig;
        private readonly ILogger<JwtDataAccessService> _logger;
        // private readonly IloginRepository _loginRepository;
        private readonly IRepository<UninetContext> _repository;
        private readonly IJwtDataAccessService _JwtDataAccessService;
        private readonly byte[] _secret;
        public IConfiguration Configuration { get; }
        public jwtAppService(ILogger<JwtDataAccessService> logger, IRepository<UninetContext> repository, IConfiguration configuration, IJwtDataAccessService JwtDataAccessService)//, IloginRepository loginRepository
        {
            _logger = logger;
            _repository = repository;
            Configuration = configuration;
            _secret = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("jwtTokenConfig:secret"));
            _JwtDataAccessService = JwtDataAccessService;
        }


        public async Task<int> GetUserIdByRefreshToken(string refreshToken)
        {
            return await _JwtDataAccessService.GetUserIdByRefreshToken(refreshToken);
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

        public async Task<RefreshtokenresponseObj> GenerateRefreshToken(int? Userid)
        {
            return await _JwtDataAccessService.GenerateRefreshToken( Userid);
        }


        public async Task<AccesstokenReturnObj> GenerateAccessToken(IEnumerable<Claim> claims)
        {
            return await _JwtDataAccessService.GenerateAccessToken(claims);
        }
    



        public async Task<string> GenerateAccessTokenFromClaims(IEnumerable<Claim> claims)
        {
         return   await _JwtDataAccessService.GenerateAccessTokenFromClaims(claims);
        }



        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
        return await _JwtDataAccessService.GetPrincipalFromExpiredToken(token);
        }


    }
}
