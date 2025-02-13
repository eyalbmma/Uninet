using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
  
    public class createExpenseApiResponse
    {
        public string id { get; set; } // Add this to map the "id" field in the response
        public ApiData api { get; set; }
        public bool status { get; set; }
        public string reason { get; set; }
        public int expense_id { get; set; }
        public string textResponse { get; set; }
    }

    public class ApiData
    {
        public int version { get; set; }
        public int tz { get; set; }
        public double ts { get; set; }
        public string lang { get; set; }
        public int rid { get; set; }
        public string module { get; set; }
        public string method { get; set; }
        public List<ApiMessage> messages { get; set; }
    }

    public class ApiMessage
    {
        public double ts { get; set; }
        public string type { get; set; }
        public string data { get; set; }
    }

}
