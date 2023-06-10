using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.DATA.Interfaces
{
    public interface IuninetBatchDataAccess
    {
        public Task<string> PullUserDatafromExternalSystem(int Userid);
        public Task<string> SendRequest(string endpointUrl, HttpMethod method, string jwtToken = null);

        public Task<string> ExtractUserCompanyLogicExpensesAndSendAsExpensesToSideB(BusinessRequestFoeExpenses businessRequest);
       
    }
}
