using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class ResetPasswordRequestcs
    {
        public string? ResetPasswordToken { get; set; }
        public string? passwordEncrypted { get; set; }
        public int Lang { get; set; }

    }
}
