using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;

namespace Uninet.APP.Services
{
    public  class UninetSimulateGreenVoiceAppServices: IUninetSimulateGreenVoiceAppServices
    {
        private readonly IUninetSimulateGreenVoiceServiceDataAccess _uninetSimulateGreenVoiceServiceDataAccess;

        public UninetSimulateGreenVoiceAppServices(IUninetSimulateGreenVoiceServiceDataAccess uninetSimulateGreenVoiceServiceDataAccess)
        {
            _uninetSimulateGreenVoiceServiceDataAccess = uninetSimulateGreenVoiceServiceDataAccess;
        }
        public async Task<string> GeneralQueryBynameValue(string name, string value)
        {
            return await _uninetSimulateGreenVoiceServiceDataAccess.GeneralQueryBynameValue(name, value);
        }
    }
}
