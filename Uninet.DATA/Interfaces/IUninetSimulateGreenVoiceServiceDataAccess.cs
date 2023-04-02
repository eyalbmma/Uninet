using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.DATA.Interfaces
{
    public interface IUninetSimulateGreenVoiceServiceDataAccess
    {
        Task<List<BsonDocument>> GeneralQueryBynameValue(string name, string value);
    }
}
