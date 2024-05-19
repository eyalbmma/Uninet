using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("UserCreditCardHolder")]
    public class UserCreditCardHolderTable
    {
        [Key]
        public int UserId { get; set; }
        [Key]
        public int BusinessId { get; set; }

        [Key]
        public int SubCompanyId { get; set; }


        [Key]
        public int ExternalSystemId { get; set; }



        public string CreditcardLogo { get; set; }
        public string CardName { get; set; }
        public string lastdigits { get; set; }
        public string expirationdate { get; set; }
    }
}
