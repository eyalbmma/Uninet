using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class SupplierItem
    {
        public string supplier_id { get; set; }
        public int vat_id { get; set; } 
        public string supplier_name { get; set; }
        public string company_name { get; set; }

    }



    public class SupplierItemMorning
    {
        public string supplier_id { get; set; }
        public int vat_id { get; set; }
        public string supplier_name { get; set; }
        public string company_name { get; set; }

    }
}
