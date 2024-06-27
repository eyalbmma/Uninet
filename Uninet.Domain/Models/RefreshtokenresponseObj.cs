using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class RefreshtokenresponseObj
    {
        public string RefreshToken { get; set; }
        public DateTimeOffset RefreshTokenExpireTime { get; set; }
    }
}