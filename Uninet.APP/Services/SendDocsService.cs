using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Models;

namespace Uninet.APP.Services
{
    public class SendDocsService : ISendDocsService
    {
        private readonly ISendDocsRepository _repository;

        public SendDocsService(ISendDocsRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> SaveDocumentAsync(SendDoc document)
        {
            if (await _repository.DocumentExistsAsync(document))
            {
                return false; // Conflict
            }

            await _repository.InsertDocumentAsync(document);
            return true;
        }
    }
}
