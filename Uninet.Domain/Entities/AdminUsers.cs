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
        public int AdminUserid { get; set; }
        [Key]
        public string Email { get; set; }
        [Key]
        public int PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool ValidUser { get; set; }

        public bool DateEmailVerificationSent { get; set; }
        public DateTime EmailVerificationsent { get; set; }

        public string GuidVerification { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }

        





    }
}
