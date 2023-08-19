using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("Jobbatchlog")]
    public class Jobbatchlog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        public int? TaskId { get; set; }

        public string? TaskDesc { get; set; }
        public DateTime? date { get; set; }
        public string? text { get; set; }
    }
}
