using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("SubCompanysID")]
    public  class SubCompanysID
    {
        


        [Key]
        public int SubCompanyid { get; set; }
    }
}
