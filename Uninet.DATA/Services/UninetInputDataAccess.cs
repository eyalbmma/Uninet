using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;

namespace Uninet.DATA.Services
{
    public class UninetInputDataAccess : IUninetInputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;

        public UninetInputDataAccess(IRepository<UninetContext> repository)//, IloginRepository loginRepository
        {
            

            _repository = repository;
           
        }

        public async Task<List<TestResponse>> GetTestResponse()
        {
            try
            {
                List<TestResponse> res = new List<TestResponse>();
                res = _repository.ExecuteGetSP<TestResponse>("dbo.GetTestData").ToList();
                return res;
            }
            catch (Exception ex) { return null; }
           
        }
    }
}
