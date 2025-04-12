using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class JoinEntityRequest
    {
        // פרטי מערכת הכספים
        public int FinanceSystemId { get; set; }
        public string FinanceSystemName { get; set; }

        // פרטי הקריאה
        public string RequestId { get; set; }
        public DateTime? PreviousRequestDate { get; set; }
        public DateTime CurrentRequestDate { get; set; }

        // פרטי הישות
        public int InternalEntityId { get; set; }
        public string EntityName { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public string EntityType { get; set; }
        public string TaxId { get; set; }
        public string VatId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        // פרטי רכבים (רשימה)
        public List<VehicleInfo> Vehicles { get; set; }

        // רמת אימות (enum או מספר)
        public int VerificationLevel { get; set; }

        // אישור תנאי שימוש
        public bool AcceptTerms { get; set; }
    }

    public class VehicleInfo
    {
        public string VehicleId { get; set; }
        public string VehicleName { get; set; }
        // ניתן להוסיף שדות נוספים
    }
}
