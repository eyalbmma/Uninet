using MongoDB.Bson;
using System;
using System.Collections.Generic;
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
        public async Task<List<DigitalDocumentToApprove>> GetDigitalDocumentToApproveListByUser(int UserID)
        {
            try
            {
                return await _uninetOutPutDataAccess.GetDigitalDocumentToApproveListByUser(UserID);
            }
            catch(Exception ex) { return null; }
        }

        public async Task<bool> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest expensesUserDoRequest, int userId)
        {
            try
            {


                return await _uninetOutPutDataAccess.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest, userId);



            }
            catch (Exception ex) { return false; }

        }
       
    }
}
