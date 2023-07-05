using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("LUT_ExtrenalFieldsType")]
    public  class LUT_ExtrenalFieldsType
    {
        [Key]
        public int FieldType { get; set; }
        public string FieldTypeText { get; set; }
    }
}
