using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class DigitalDocumentToApproveObj
    {
        public List<DigitalDocumentToApprove> listDigitalDocumentToApprove { get; set; }
        
       // public int TotallistDigitalDocumentToApprove { get; set; }
        public string fullname { get; set; }

        public List<CompanyNameRelatedToUser> ListOfSubCompaniesandNames { get; set; }

        public bool ShowFirstTimeMessage { get; set; }
        public bool ShowExceedsMessage { get; set; }
    }
}
