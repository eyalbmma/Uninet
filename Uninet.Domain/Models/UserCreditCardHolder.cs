using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    
    public class CreditCardHolderUser
    {
        public string CreditcardLogo { get; set; }
        public string CardName { get; set; }
        public string Last4Digits { get; set; }
        public string ExpirationDate { get; set; }
    }

    public class UserCreditCardHolderList
    {
        public List<CreditCardHolderUser> CreditCardHolders { get; set; } = new List<CreditCardHolderUser>();
    }


}
