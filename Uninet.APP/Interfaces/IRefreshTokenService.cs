using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.APP.Interfaces
{
    public interface IRefreshTokenService
    {
        void SaveOrUpdate(string token, int systemId, DateTime expiresAt);
        bool Exists(string token, out int systemId);
        void Delete(string token);
    }
}
