using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("RefreshTokens")] // 👈 this is what tells EF to use the correct table
    public class ExternalServiceRefreshToken
    {
        [Key]
        public string Token { get; set; }

        public int? ExternalSystemId { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
