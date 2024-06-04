using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("FirstTimeConsoleIndication")]
    public  class FirstTimeConsoleIndication
    {
        [Key]
        public int Userid { get; set; }
        [Key]
        public int  Mainorganization { get; set; }
        [Key]
        public int ?Subcompanyid { get; set; }
        public bool FirsttimeOnConsoleForEntity { get; set; }
    }
}
