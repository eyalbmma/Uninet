using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class ExpensesDigitalDocumentProp
    {
        public string Supplier_name_Sender { get; set; }

        public int Supplier_ID { get; set; }
        public string DocNumber { get; set; }
        public string Doctype { get; set; }

        public DateTime DocDate { get; set; }

        public double AmountAV { get; set; }


        public List<ExpenseType> ExpenseTypeList { get; set; }

        public Int32 internalCompanyId { get; set; }

        public string Jsondocumentid { get; set; }

        public string TaxId { get; set; }

        public double AmountBeforeVat { get; set; }

        public double Vat { get; set; }


    }
}
