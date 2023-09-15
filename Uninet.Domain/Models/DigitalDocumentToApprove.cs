using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class DigitalDocumentToApprove
    {
        public string JsonDocumentid { get; set; }

        public int ClientVat_id { get; set; }

        public int SendingDigitalDocumentBusinessID { get; set; }

        public string BusinessVatId { get; set; }

        public string DocInfoUrl { get; set; }


        public string? supplier_name_Sender { get; set; }

        public DateTime? docDate { get; set; }

        public double? amountAV { get; set; }

        public string? currency_code { get; set; }

        //public string Supplier_name_Sender { get; set; }

        //public int Supplier_ID { get; set; }
        //public int DocNumber { get; set; }
        //public string Doctype { get; set; }

        //public DateTime DocDate { get; set; }

        //public float AmountAV { get; set; }


        //public List<ExpenseType> ExpenseTypeList { get; set; }


    }
}
