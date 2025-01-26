using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class ExpenseType
    {
        public string ExpenseTypeId { get; set; }
        public string ExpenseTypeDesc { get; set; }

        public bool IsDefault { get; set; }
    }
}
