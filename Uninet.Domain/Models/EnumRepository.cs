using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{

    public enum FilterType
    {
        Supplier = 1,
        Client = 2,
        Both = 3,
        ShortversionSuplier=4
    }
    public  class EnumRepository
    {
        public FilterType FilterType { get; } = FilterType.Both; // Default value

        public EnumRepository()
        {
           
           
        }
    }
}
