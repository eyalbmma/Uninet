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
        Task<string> GeneralQueryBynameValue(string name, string value);
        Task<string> PullBusinessDataByTaxid(string Taxid);
    }
}
