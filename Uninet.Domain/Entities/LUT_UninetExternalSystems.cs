using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("LUT_UninetExternalSystems")]
    public class LUT_UninetExternalSystems
    {
       
        public string SystemName { get; set; }
       
        [Key]
        public int SyestemId { get; set; }

        public string Logo { get; set; }

        public string Video { get; set; }
    }
}
