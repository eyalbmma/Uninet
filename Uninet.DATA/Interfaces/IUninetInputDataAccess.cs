using MongoDB.Bson;
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
       public  Task<List<TestResponse>> GetTestResponse();
        public Task<bool> SavegreenvoicedocumentIntoUninet(List<BsonDocument> InputData);

        public Task<BsonDocument> GetQuestion(int questionNumber, string language);

        public Task<List<BsonDocument>> GetLandingPageContent(string language);

        public Task<string> PullUserDatafromExternalSystem(int Userid);
    }
}
