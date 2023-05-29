using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{


    [Table("CompanyPulledDataLog")]
    public class CompanyPulledDataLog
    {
        [Key]
        public int CompanyVatid { get; set; }

        public DateTime LastPullDataDate { get; set; }
    }
}
