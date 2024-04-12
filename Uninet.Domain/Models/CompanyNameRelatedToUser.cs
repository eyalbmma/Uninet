using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class CompanyNameRelatedToUser
    {
        public int SubCopmanyId { get; set; }
        public string CompanyName { get; set; }

        public bool IsDefault { get; set; }
    }
}
