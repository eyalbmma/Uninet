using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    public class AddUserCredentialsSystemResult
    {
        public int AdminUserid { get; set; }
        public string  Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string OrganizationName { get; set; }

        public int? NewSubCompanyId { get; set; }

    }
}
