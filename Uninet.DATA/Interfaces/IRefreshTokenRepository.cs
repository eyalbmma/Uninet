using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.DATA.Interfaces
{
    public interface IRefreshTokenRepository
    {

        void SaveOrUpdate(string token, Guid systemGuid, DateTime expiresAt);
        bool Exists(string token, out Guid systemGuid);
        void Delete(string token);
    }
}
