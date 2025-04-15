using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class Vehicle
    {
        public string VehicleId { get; set; }
        public string VehicleName { get; set; }
    }

    public class JoinEntityRequest
    {
        public int? FinanceSystemId { get; set; }
        public string? FinanceSystemName { get; set; }
        public int? InternalEntityId { get; set; }
        public List<Vehicle>? Vehicles { get; set; }

        public Guid ExternalSystemGuid { get; set; }

        // מידע שמגיע ממערכת הכספים (user/pass/cid או apiKey וכו')
        public List<CustomizedDataList> Variables { get; set; }

        public int? SubCompanyId { get; set; }

        // פרטי הישות
        public string EntityName { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string EntityType { get; set; }
        public string TaxId { get; set; }
        public string VatId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        // פרטי הקריאה
        public string RequestId { get; set; }
        public DateTime? PreviousRequestDate { get; set; }
        public DateTime? CurrentRequestDate { get; set; }

        // אימות ואישור
        public int? VerificationLevel { get; set; }
        public bool AcceptTerms { get; set; }

        // טוקן - למערכות שדורשות (כמו גרין אינבויס)
       /// <summary>
       /// public string Token { get; set; }
       /// </summary>
        public DateTime? TokenExpiration { get; set; }
    }

    public class CustomizedDataList
    {
        public string FieldLabelName { get; set; }
        public string FieldLabelValue { get; set; }
        public int? FiledType { get; set; }
        public string? FieldTypeDesc { get; set; }
    }

}
