using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.Domain.StoredProcedures.Responses
{

    public class AddBusinessToUserResult
    {
        public bool Result { get; set; }
        public List<BusinessRequest> BusinessRequests { get; set; }
       
    }

}
