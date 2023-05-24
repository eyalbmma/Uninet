using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("LutCompanies")]
    public class LutCompanies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyInnerId { get; set; }

        
        public string? CompanyName { get; set; }

        public string? CompanyEmail { get; set; }

      
    }
}
