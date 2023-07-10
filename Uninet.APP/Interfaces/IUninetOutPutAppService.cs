using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public interface IUninetOutPutAppService
    {
        Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest expensesUserDoRequest,int userId);
        
            Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId);
        Task<List<DigitalDocumentToApprove>> GetDigitalDocumentToApproveListByUser(int UserID);
    }
}
