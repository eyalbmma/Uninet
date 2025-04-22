using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Entities;
namespace Uninet.APP.Interfaces
{
    public interface IExternalSystemService
    {
       

        // חדש:
        ExternalSystem GetSystemByApiKey(string apiKey);
    }
}
