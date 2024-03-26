using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Entities
{
    [Table("LUTIcountSourceWebhookCompanyMapping")]
    public class LUTIcountSourceWebhookCompanyMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WebHookSourceid { get; set; }

        public int Internalcompanyid { get; set; }

        public int? SubCompanyId { get; set; }

        public int WebhookID { get; set; }





    }
}
