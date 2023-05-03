using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Models;

namespace Uninet.APP.Services
{
    public class UninetInputAppService : IUninetInputAppService
    {
        readonly IUninetInputDataAccess _uninetInputDataAccess = null;
        public UninetInputAppService(IUninetInputDataAccess uninetInputDataAccess)
        {
            // _logger = logger;
            _uninetInputDataAccess = uninetInputDataAccess;
        }
        public async Task<List<TestResponse>> GetTestResponse()
        {
            try
            {

                return await _uninetInputDataAccess.GetTestResponse();

            }
            catch (Exception ex) { throw new Exception(); }

        }


        public async Task<bool> SavegreenvoicedocumentIntoUninet(List<BsonDocument> InputData)
        {
            try
            {


                return await _uninetInputDataAccess.SavegreenvoicedocumentIntoUninet(InputData);



            }
            catch (Exception ex) { return false; }

        }
        //public async Task<string> PullUserDatafromExternalSystem(int Userid)
        //{
        //    try
        //    {
        //        return await _uninetInputDataAccess.PullUserDatafromExternalSystem(Userid);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception();
        //    }
        //}
        public async Task<List<BsonDocument>> GetLandingPageContent(string language)
        {
            try
            {
                return await _uninetInputDataAccess.GetLandingPageContent( language);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<BsonDocument> GetQuestion(int questionNumber, string language)
        {
            try
            {
                return await _uninetInputDataAccess.GetQuestion(questionNumber, language);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }




        }
    }
}
