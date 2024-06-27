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

        // Static methods to easily get predefined messages based on language
        public static BillingMessage GetSuccessMessage(int lang)
        {
            if (lang == 1) // English
            {
                return new BillingMessage("Billing Succeed", "", "Ok");
            }
            else if (lang == 2) // Hebrew
            {
                return new BillingMessage("החיוב הצליח", "", "אישור");
            }
            else // Default to English if language is not recognized
            {
                return new BillingMessage("Billing Succeed", "", "Ok");
            }
        }

        public static BillingMessage GetFailureMessage(int lang)
        {
            if (lang == 1) // English
            {
                return new BillingMessage(
                    "Billing Failed",
                    "There was an error with the billing process, please try again later or contact us.",
                    "Ok");
            }
            else if (lang == 2) // Hebrew
            {
                return new BillingMessage(
                    "החיוב נכשל",
                    "אירעה שגיאה בתהליך החיוב, אנא נסה שוב מאוחר יותר או פנה אלינו.",
                    "אישור");
            }
            else // Default to English if language is not recognized
            {
                return new BillingMessage(
                    "Billing Failed",
                    "There was an error with the billing process, please try again later or contact us.",
                    "Ok");
            }
        }
    }


}
