using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public interface IUninetInputAppService
    {
        Task<List<TestResponse>> GetTestResponse();
    }
}
