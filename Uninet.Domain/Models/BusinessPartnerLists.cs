using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class BusinessPartnerLists
    {
        public List<string> ClientEmailList { get; set; }
        public List<string> SupplierList { get; set; }

        public BusinessPartnerLists()
        {
            ClientEmailList = new List<string>();
            SupplierList = new List<string>();
        }
    }
}
