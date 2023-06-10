using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class BusinessRequestFoeExpenses
    {
        public string BusinessId { get; set; }
        public string AdminUserid { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
