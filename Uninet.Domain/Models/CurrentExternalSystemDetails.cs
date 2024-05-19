using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class CurrentExternalSystemDetails
    {
        
            public int ExternalSystemId { get; set; }
            public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
        

    }
}
