using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class DigitalDocumentDInputRequest
    {

        //[JsonPropertyName("expense_doctype")]
        //public string expense_doctype { get; set; }


        //[JsonPropertyName("expense_docnum")]
        //public string? expense_docnum { get; set; }


        //[JsonPropertyName("expense_sum")]
        //public float? expense_sum { get; set; }



        //[JsonPropertyName("currency_id")]
        //public int? currency_id { get; set; }


        //[JsonPropertyName("currency_code")]
        //public string? currency_code { get; set; }





        //[JsonPropertyName("rate")]
        //public float? rate { get; set; }


        //[JsonPropertyName("client_id")]
        //public int? client_id { get; set; }


        //[JsonPropertyName("project_id")]
        //public int? project_id { get; set; }//dont know from where 


        //[JsonPropertyName("expense_date")]
        //public int? expense_date { get; set; }//dont know from where 


        //[JsonPropertyName("invoice_date")]
        //public DateTime invoice_date { get; set; }//get from dateissued 


        //[JsonPropertyName("vat_date")]
        //public DateTime vat_date { get; set; }//get from dateissued 

        //[JsonPropertyName("is_draft")]//dont know from where 
        //public bool? is_draft { get; set; }

        //[JsonPropertyName("is_recurring")]//dont know from where 
        //public bool? is_recurring { get; set; }




        //[JsonPropertyName("recurring_period")]//dont know from where 
        //public string? recurring_period { get; set; }


        //[JsonPropertyName("recurring_number")]//dont know from where 
        //public int? recurring_number { get; set; }


        //[JsonPropertyName("recurring_end_date")]//dont know from where 
        //public DateTime? recurring_end_date { get; set; }



        //[JsonPropertyName("expense_paid")]//dont know from where 
        //public bool? expense_paid { get; set; }


        //[JsonPropertyName("expense_paid_date")]//dont know from where 
        //public DateTime? expense_paid_date { get; set; }


        //[JsonPropertyName("cc")]//dont know from where  and what type
        //public DateTime? cc { get; set; }


        //[JsonPropertyName("cheques")]//dont know from where  and what type
        //public DateTime? cheques { get; set; }



        //[JsonPropertyName("comment")]//dont know from where 
        //public string? comment { get; set; }


        [JsonPropertyName("JsonDocumentid")]
        public string JsonDocumentid { get; set; }



        [JsonPropertyName("sendingDigitalDocumentBusinessID")]
        public int sendingDigitalDocumentBusinessID { get; set; }

        [JsonPropertyName("ClientVat_id")]
        public string  ClientVat_id { get; set; }

        [JsonPropertyName("BusinessVatId")]
        public string  BusinessVatId { get; set; }



        [JsonPropertyName("Lang")]
        public int Lang { get; set; }





    }
}
