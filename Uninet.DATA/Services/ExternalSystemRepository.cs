using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;

namespace Uninet.DATA.Services
{
    public class ExternalSystemRepository : IExternalSystemRepository
    {
        private readonly UninetContext _context;

        public ExternalSystemRepository(UninetContext context)
        {
            _context = context;
        }

        public ExternalSystem GetById(int externalSystemId)
        {
            return _context.ExternalSystem
                .FirstOrDefault(es => es.ExternalSystemID == externalSystemId);
        }
    }
}
