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

        public Guid ExternalSystemGuid { get; set; }

        public string ApiKey { get; set; }
        public string SharedSecret { get; set; }
        public string AllowedIP { get; set; }
        public string AllowedScopes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ClockDriftToleranceSeconds { get; set; }
        public string ExternalSystemName { get; set; }
    }

}
