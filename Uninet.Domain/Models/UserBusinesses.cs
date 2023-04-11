using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class UserBusinesses
    {
       public  List<BusinessRequest> BusinessRequests { get; set; }
       public int Userid { get; set; }
    }
}
