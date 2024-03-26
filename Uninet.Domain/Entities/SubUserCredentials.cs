using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("SubUserCredentials")]
    public class SubUserCredentials
    {

        [Key]
        public int Userid { get; set; }
        
        [Key]
        public int CompanyId { get; set; }
        
        [Key]
        public int SubCompanyId { get; set; }


        [Key]
        public int SubUserId { get; set; }

        


    }
}
