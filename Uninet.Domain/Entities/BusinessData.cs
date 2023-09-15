using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{

    [Table("BusinessData")]
    public  class BusinessData
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int BusinessId { get; set; }

        [Key]
        public string JsonDocumentid { get; set; }

        

        public string BusinessVatId { get; set; }
        public int? ClientVat_id { get; set; }


        public string? client_name { get; set; }

        public int? DataSourceEnum { get; set; }

        public string? ClientEmail { get; set; }

        public bool? EmailSent { get; set; }

        public DateTime? DateEmailSent { get; set; }

        public bool? DocumentApprovedtoUninet { get; set; }



        public string? supplier_name_Sender { get; set; }

        public DateTime? docDate { get; set; }

        public double? amountAV { get; set; }

        public string? currency_code { get; set; }


    }
}
