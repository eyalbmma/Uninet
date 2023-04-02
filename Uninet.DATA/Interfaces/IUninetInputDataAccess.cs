using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.DATA.Interfaces
{
    public interface IUninetInputDataAccess
    {
        Task<List<TestResponse>> GetTestResponse();
    }
}
