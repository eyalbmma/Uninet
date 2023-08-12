using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class ResentOtpRequest
    {
        public string Email { get; set; }
        public string EncryptedUserId { get; set; }
        public int TemplateId { get; set; }
        public int Lang { get; set; }
        
    }
}
