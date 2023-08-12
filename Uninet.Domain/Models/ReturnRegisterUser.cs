using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class ReturnRegisterUser
    {
        public int Userid { get; set; }
        public int UserStatusIndication { get; set; }

        public bool? verified { get; set; }
    }
}
