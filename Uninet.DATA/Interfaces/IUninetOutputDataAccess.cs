using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.DATA.Interfaces
{
    public interface IUninetOutputDataAccess
    {
        Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId);
        Task<responseTest> test(int UserID, string Typelist);
        Task<RejectDocumenResponse> RejectDocument(RequestRejectDocument requestRejectDocument);
        Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest expensesUserDoRequest, int userId);
        Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId);
        Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID,string Typelist, int? subCompanyId);
    }
}
