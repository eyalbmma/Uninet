using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;
namespace Uninet.DATA.Interfaces
{
    public interface ISendDocsRepository
    {
        Task<bool> DocumentExistsAsync(SendDoc document);
        Task InsertDocumentAsync(SendDoc document);
    }
}
