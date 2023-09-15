using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class VerifyEamilLinkREsponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }

        public bool Success { get; set; }
       
    }
}
