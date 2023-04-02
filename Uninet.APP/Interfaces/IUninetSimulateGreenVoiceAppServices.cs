using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.APP.Interfaces
{
    public interface IUninetSimulateGreenVoiceAppServices
    {
        Task<List<BsonDocument>> GeneralQueryBynameValue(string name, string value);
    }
}
