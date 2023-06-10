using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
   

    [Table("Businesses")]
    public class Businesses
    {
        [Key] 
        public int AdminUserid { get; set; }
        [Key]
        public int BusinessId { get; set; }
        
      
        
        public int? BusinessType { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        public string?  OrganizationRole { get; set; }
        public string? OrganizationName { get; set; }

        public int? OrganizationType { get; set; }

        public int? ExternalSystemId { get; set; }

       
        public string? Apikey { get; set; }



        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Endpoint { get; set; }


    }
}
