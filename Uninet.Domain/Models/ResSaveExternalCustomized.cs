using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class ResSaveExternalCustomized
    {
        public bool Success { get; set; }
        public string textResponse { get; set; }
        public bool SystemRegisteredInuninet { get; set; }
        public bool? ValidExternalsystemCredenatials { get; set; }
        public string FullName { get; set; }

        public List<string>  RelatedSubCompanyListIds { get; set; }

        public bool? ClickedButtonToInviteBusinessPartners { get; set; }
    }
}
