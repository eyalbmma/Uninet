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
                    textResponse = requestRejectDocument.Lang == 1 ? "error occured Document wasnt rejected " : "ארעה שגיאה המבמך לא נדחה "
                };
                return res;
            }
        }

        public async Task<List<DigitalDocumentToApprove>> GetDigitalDocumentToApproveListByUser(int UserID,string Typelist)
        {
            try
            {
                return await _uninetOutPutDataAccess.GetDigitalDocumentToApproveListByUser(UserID, Typelist);
            }
            catch(Exception ex) { return null; }
        }

        public async Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest expensesUserDoRequest, int userId)
        {
            try
            {


                return await _uninetOutPutDataAccess.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest, userId);



            }
            catch (Exception ex) { return null; }

        }
       
    }
}
