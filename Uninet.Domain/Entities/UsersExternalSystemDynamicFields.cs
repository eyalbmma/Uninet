using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
   


    [Table("UsersExternalSystemDynamicFields")]
    public class UsersExternalSystemDynamicFields
    {
        [Key]
        public int Companyid { get; set; }

        [Key]
        public int SubCompayId { get; set; }
        

        [Key]
        public int Userid { get; set; }

        [Key]
        public int ExternalSystemId { get; set; }

        [Key]
        public string FieldLabelName { get; set; }

        [Key]
        public string FieldLabelValue { get; set; }
        

    }

}
