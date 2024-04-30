using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{


    [Table("LUT_ExternalSystem")]
    public class ExternalSystem
    {

        [Key]
        public int ExternalSystemID { get; set; }


        public string ExternalSystemName { get; set; }

        
            
    }
}
