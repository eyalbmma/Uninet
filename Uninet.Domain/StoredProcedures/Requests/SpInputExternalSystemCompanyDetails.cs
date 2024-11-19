using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.Domain.StoredProcedures.Requests
{
    public class SpInputExternalSystemCompanyDetails
    {
        public List<CustomizedDataLIst> ListInputLabelDetails { get; set; }
        
        public int ExternalSystemId { get; set; }
        public int Companyid { get; set; }
        public int? SubCompanyId { get; set; }

        public int Lang { get; set; } 
    }
}
