using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Enums;

namespace Uninet.Domain.Models
{
    public class Vehicle
    {
        public string VehicleId { get; set; }
        public string VehicleName { get; set; }
    }
   
    public class JoinEntityRequest
    {
        // פרטי מערכת הכספים
        public string FisId { get; set; }
        public string FisName { get; set; }

        // פרטי הישות
        public string EntityIdInternal { get; set; }
        public string EntityName { get; set; }
        public string EntityTaxNumber { get; set; }
        public string EntityVatNumber { get; set; }
        public string EntityCountry { get; set; }
        public EntityType EntityType { get; set; }
        public AuthenticationLevel EntityAuthenticationLevel { get; set; }

        // אימות ותנאים
        public bool EntityTermsAgree { get; set; }
        

        // פרטי קשר
        public string? EntityEmail { get; set; }
        public string? EntityPhone { get; set; }

        // רכבים
        public List<string>? EntityCars { get; set; }

        // מזהה מערכת חיצונית
        public Guid ExternalSystemGuid { get; set; }
    }

    public class CustomizedDataList
    {
        public string FieldLabelName { get; set; }
        public string FieldLabelValue { get; set; }
        public int? FiledType { get; set; }
        public string? FieldTypeDesc { get; set; }
    }

}
