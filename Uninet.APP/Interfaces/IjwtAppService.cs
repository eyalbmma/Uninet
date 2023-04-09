using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.APP.Interfaces
{
    public  interface IjwtAppService
    {

        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
        Task<string> GetRefreshTokenByUserId(int Userid);
        Task<string> GenerateAccessTokenFromClaims(IEnumerable<Claim> claims);
        Task<string> GenerateRefreshToken(int Userid);
        //Task<string> GenerateAccessToken(string userId);
        Task<string> GenerateAccessToken(IEnumerable<Claim> claims);
        Task<int> SaveRefreshToken(int Userid, string refreshToken);
    }
}
