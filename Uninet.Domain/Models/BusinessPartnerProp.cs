using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public enum ActionType
    {
        Button = 1,
        DateTime = 2,
        String = 3
    }

    public class BusinessPartnerProp
    {
        public string BusinesspartnerName { get; set; }
        public string VatId { get; set; }
        public int DocAmount { get; set; }
        public string Status { get; set; }
        public DateTime? LastInvitationDate { get; set; }

        public ActionItem Actions { get; set; }
    }

    public class ActionItem
    {
        public string Label { get; set; }
        public ActionType Type { get; set; }

        public ActionItem(string label, ActionType type)
        {
            Label = label;
            Type = type;
        }
    }

}
