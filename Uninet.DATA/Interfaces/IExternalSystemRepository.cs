using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Entities;
namespace Uninet.DATA.Interfaces
{
    public interface IExternalSystemRepository
    {
        ExternalSystem GetById(Guid ExternalSystemGuid);
    }
}
