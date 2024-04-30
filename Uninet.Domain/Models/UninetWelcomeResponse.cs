using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class UninetWelcomeResponse
    {
        public string EntityName { get; set; }
        public int VatId { get; set; }
        public string FinancialSoftware { get; set; }
    }
}
