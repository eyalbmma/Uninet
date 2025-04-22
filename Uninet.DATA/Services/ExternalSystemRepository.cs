using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        public ExternalSystem GetSystemByApiKey(string apiKey)
        {
            return _context.ExternalSystem
                .FirstOrDefault(x => x.ApiKey == apiKey && x.IsActive);
        }

        
    }
}
