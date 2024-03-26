using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("MainSubCopmaniesMasters")]
    public  class MainSubCopmaniesMasters
    {
        [Key]
        public int MainCompanyId { get; set; }

        [Key]
        public int SubCopmanyId { get; set; }



        public DateTime LastTimeDataShowed { get; set; }
    }
}
