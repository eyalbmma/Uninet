using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class BusinessPartnerLists
    {
        public List<ClientObj> ClientEmailList { get; set; }
        public List<SupplierObj> SupplierList { get; set; }
       public  List<EmailSentResult> EmailSentResults { get; set; }

        public BusinessPartnerLists()
        {
            ClientEmailList = new List<ClientObj>();
            SupplierList = new List<SupplierObj>();
            EmailSentResults = new List<EmailSentResult>();
        }
    }


    public class ClientObj
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }



    public class SupplierObj
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
