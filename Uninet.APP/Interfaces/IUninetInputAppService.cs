using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public interface IUninetInputAppService
    {
        Task<List<TestResponse>> GetTestResponse();

        Task<bool> SavegreenvoicedocumentIntoUninet(List<BsonDocument> InputData);

         Task<BsonDocument> GetQuestion(int questionNumber, string language);
        Task<List<BsonDocument>> GetLandingPageContent(string language);

        Task<bool> MorningReceiveWebhook(string json);
        Task<bool> ReceiveWebhook(string json, string WebHookSourceid);
        //Task<string> PullUserDatafromExternalSystem(int Userid);
    }
}
