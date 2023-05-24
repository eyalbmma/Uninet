using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Uninet.Domain.Entities
{
    
    [Table("ExternalSystemDynamicFields")]
    public class ExternalSystemDynamicFields
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int ExternalSystemId { get; set; }

        public string? FieldLabelName { get; set; }

        public string? FieldLabelValue { get; set; } 




    }
}
