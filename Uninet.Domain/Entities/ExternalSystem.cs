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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ExternalSystemID { get; set; }

        [MaxLength(50)]
        public string ExternalSystemName { get; set; }

        [MaxLength(100)]
        public string ApiKey { get; set; }

        [MaxLength(256)]
        public string SharedSecret { get; set; }

        [MaxLength(100)]
        public string AllowedIP { get; set; }

        [MaxLength(200)]
        public string AllowedScopes { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ClockDriftToleranceSeconds { get; set; }
    }
}
