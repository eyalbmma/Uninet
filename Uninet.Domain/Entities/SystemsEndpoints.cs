using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{

    [Table("SystemsEndpoints")]
    public class SystemsEndpoints
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        public int ExternalSystemId { get; set; }
        [Key]
        public string  Endpoint { get; set; }

        public string? EndpointCategory { get; set; }


        [Key]
        public string MethodeType { get; set; }
        

    }
}
