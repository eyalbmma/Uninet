using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("RefreshTokens")]
    public class ExternalServiceRefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }

        public Guid ExternalSystemGuid { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? CreatedAt { get; set; }
    }


}
