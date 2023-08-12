using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    

    [Table("AdminUsers")]
    public class AdminUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminUserid { get; set; }
        [Key]
        public string Email { get; set; }
        [Key]
        public int PhoneNumber { get; set; }
        public string? passwordEncrypted { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? ValidUser { get; set; }

        public DateTime? DateOtpSent { get; set; }
        
        public int? OtpSentCounter { get; set; }
        public bool? Otpsent { get; set; }

        

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireTime { get; set; }

        


        public string? GoogleId { get; set; }


        public string?  ResetPasswordToken { get; set; }

        public DateTime?  ExpiredpasswordTokenDate { get; set; }

    }
}
