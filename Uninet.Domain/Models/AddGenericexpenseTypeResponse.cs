using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class AddGenericexpenseTypeResponse
    {
        public List<ExpenseType> ExpenseTypeList { get; set; }
        public string textResponse { get; set; }
    }
}
