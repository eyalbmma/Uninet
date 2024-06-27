using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    public  class SaveRefreshTokenResponse
    {
        public bool Result { get; set; }
        public DateTimeOffset RefreshTokenExpireTime { get; set; }
    }
}
