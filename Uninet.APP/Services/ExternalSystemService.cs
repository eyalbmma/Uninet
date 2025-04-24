using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Entities;
namespace Uninet.APP.Services
{
    public class ExternalSystemService : IExternalSystemService
    {
        private readonly IExternalSystemRepository _repository;

        public ExternalSystemService(IExternalSystemRepository repository)
        {
            _repository = repository;
        }
        public ExternalSystem GetSystemByApiKey(string apiKey)
        {
            return _repository.GetSystemByApiKey(apiKey);
        }
        public ExternalSystem GetSystemByGuid(Guid systemGuid)
        {
            return _repository.GetSystemByGuid(systemGuid);
        }

    }
}
