using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public interface IUninetOutPutAppService
    {
        Task<responseTest> test(int UserID, string Typelist);
        Task<RejectDocumenResponse> RejectDocument(RequestRejectDocument requestRejectDocument);
        Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest expensesUserDoRequest,int userId);
        Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId);
        Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId);
        Task<List<BusinessPartnerProp>> GetBusinessPartnersByFilter(int filterType, int userId,int subCopmanyId);

        Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID, string Typelist, int? subCompanyId, int pageNumber, int pageSize);

        Task<SendOtpViaMailResponse> SendEmail(string VatId, string userId,int Lang);

    }
}
