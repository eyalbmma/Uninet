using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.Domain.Entities;
using Uninet.Domain.Models;

namespace Uninet.APP.Services
{
    public  class UninetOutPutAppService: IUninetOutPutAppService
    {
        readonly IUninetOutputDataAccess _uninetOutPutDataAccess = null;

        public UninetOutPutAppService(IUninetOutputDataAccess uninetOutPutDataAccess)
        {
            // _logger = logger;
            _uninetOutPutDataAccess = uninetOutPutDataAccess;
        }
        public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        {
            try
            {
                return await _uninetOutPutDataAccess.ShowDigitalDocumentDetails(expensesUserDoRequest, userId);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<responseTest> test(int UserID, string Typelist)
        {
            try
            {
                return await _uninetOutPutDataAccess.test(UserID, Typelist);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId)
        {
            try
            {
                return await _uninetOutPutDataAccess.AddexpenseType(addexpenseTypeRequest, userId);
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }

        public async Task<RejectDocumenResponse> RejectDocument(RequestRejectDocument requestRejectDocument)
        {
            try
            {
                return await _uninetOutPutDataAccess.RejectDocument(requestRejectDocument);
            }
            catch (Exception ex)
            {
                var res = new RejectDocumenResponse
                {
                    Success = false,
                    textResponse = requestRejectDocument.Lang == 1 ? "An error occurred, the document was not rejected. " : "אירעה שגיאה המסמך לא נדחה "
                };
                return res;
            }
        }

        public async Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID,string Typelist, int? subCompanyId,int pageNumber, int pageSize) 
        {
            try
            {
                return await _uninetOutPutDataAccess.GetDigitalDocumentToApproveListByUser(UserID, Typelist, subCompanyId, pageNumber, pageSize);
            }
            catch(Exception ex)
            { 
              return null; 
            }
        }
        

        public async Task<TotalBusinessPartnerProp> GetBusinessPartnersByFilter(int filterType, int userId,int subCopmanyId, int pageNumber, int pageSize, int Lang)
        {
            try
            {
                return await _uninetOutPutDataAccess.GetBusinessPartnersByFilter(filterType, userId, subCopmanyId,  pageNumber,  pageSize, Lang);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest expensesUserDoRequest, int userId)
        {
            try
            {
                return await _uninetOutPutDataAccess.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest, userId);
            }
            catch (Exception ex)
            {
                return new createExpenseApiResponse
                {
                    status = false,
                    reason = $"Unexpected error: {ex.Message}"
                };
            }
        }


    }
}
