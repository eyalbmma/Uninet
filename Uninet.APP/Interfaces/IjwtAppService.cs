using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public  interface IjwtAppService
    {
        
        Task<int> GetUserIdByRefreshToken(string  RefreshToken);
        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
        Task<string> GetRefreshTokenByUserId(int Userid);
        Task<string> GenerateAccessTokenFromClaims(IEnumerable<Claim> claims);
        Task<RefreshtokenresponseObj> GenerateRefreshToken(int? Userid);
        //Task<string> GenerateAccessToken(string userId);
        Task<string> GenerateAccessToken(IEnumerable<Claim> claims);
        Task<int> SaveRefreshToken(int Userid, string refreshToken);
    }
}
