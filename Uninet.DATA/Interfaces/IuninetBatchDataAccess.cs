using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.DATA.Interfaces
{
    public interface IuninetBatchDataAccess
    {
        public Task<string> PullUserDatafromExternalSystem(int Userid);
        
    }
}
