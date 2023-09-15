using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class AddexpenseTypeResponse
    {
       
        public bool status { get; set; }
        public string reason { get; set; }
        public string error_description { get; set; }
        public string[] error_details { get; set; }
    }
}
