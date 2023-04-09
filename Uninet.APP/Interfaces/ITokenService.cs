using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.APP.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(string userId);
        Task<string> GenerateRefreshToken();

        Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
       

        Task<bool> ValidateRefreshToken(string refreshToken);
    }
}
