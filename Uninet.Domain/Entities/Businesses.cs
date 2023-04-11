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
        
        public string BusinessName { get; set; }
        public string? BusinessEmail { get; set; }
        public int? DelearType { get; set; }
        public int? BusinessType { get; set; }
        

    }
}
