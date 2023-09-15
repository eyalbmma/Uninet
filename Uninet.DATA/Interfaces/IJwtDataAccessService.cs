using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.DATA.Interfaces
{
    public interface IJwtDataAccessService
    {




        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
        Task<int> GetUserIdByRefreshToken(string refreshToken);
        Task<string> GenerateAccessTokenFromClaims(IEnumerable<Claim> claims);
        Task<RefreshtokenresponseObj> GenerateRefreshToken(int? Userid);
        //Task<string> GenerateAccessToken(string userId);
        Task<string> GenerateAccessToken(IEnumerable<Claim> claims);
        Task<int> SaveRefreshToken(int Userid, string refreshToken);
    }
}
