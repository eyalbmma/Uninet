using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class IncomeItem
    {
        public string catalogNum { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }
        public string currency { get; set; }
        public float? currencyRate { get; set; }
        public int? vatType { get; set; }
        public float? vat { get; set; }
        public string itemId { get; set; }
        public float? vatRate { get; set; }
        public float? amountTotal { get; set; }
    }
}
