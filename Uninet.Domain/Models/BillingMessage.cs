using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class BillingMessage
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ButtonLabel { get; set; }

        // Constructor to easily create a message with all properties
        public BillingMessage(string title, string content, string buttonLabel)
        {
            Title = title;
            Content = content;
            ButtonLabel = buttonLabel;
        }

        // Static methods to easily get predefined messages
        public static BillingMessage GetSuccessMessage()
        {
            return new BillingMessage("Billing Succeed", "", "Ok");
        }

        public static BillingMessage GetFailureMessage()
        {
            return new BillingMessage(
                "Billing Failed",
                "There was an error with the billing process, please try again later or contact us.",
                "Ok");
        }
    }

}
