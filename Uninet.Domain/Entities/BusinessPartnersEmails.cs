using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    public class BusinessPartnersEmails
    {
        public int VatId { get; set; }
        public string EntityType { get; set; }
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public int SubCompanyId { get; set; }
        public bool? EmailSent { get; set; }  
        public DateTime? LastDateSent { get; set; }  

        
    }
}
