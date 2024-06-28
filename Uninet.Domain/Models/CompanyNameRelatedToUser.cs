using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Uninet.Domain.Models
{
    public class CompanyNameRelatedToUser
    {
        public int SubCopmanyId { get; set; }
        public string CompanyName { get; set; }

        public bool IsDefault { get; set; }
        public int TotalDocs { get; set; }

        public int MainCompanyId { get; set; }


        public bool ShowFirstTimeMessage { get; set; }
        public bool ShowExceedsMessage { get; set; }

    }
}
