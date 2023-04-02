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
    public class UninetInputAppService: IUninetInputAppService
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
    }
}
