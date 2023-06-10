using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;

namespace Uninet.DATA.Services
{
    public class PushExpensesToCompanyClient: IHostedService, IDisposable
    {
        private readonly ILogger<PushExpensesToCompanyClient> _logger;
        private Timer _timer;
        private string _filePath;
        private readonly IBatchRepository<UninetBatchContext> _repository;
        private readonly IuninetBatchDataAccess _uninetBatchDataAccess;
        private bool _isRunning = false;

        private bool _isWorking = false;
        private readonly object _workLock = new object();

        public PushExpensesToCompanyClient(ILogger<PushExpensesToCompanyClient> logger, IuninetBatchDataAccess uninetBatchDataAccess, IBatchRepository<UninetBatchContext> repository)
        {
            _logger = logger;
            _filePath = @"C:\\LogFileTest\log.txt"; // Set the file path here
                                                    // _repository = repository;
            _uninetBatchDataAccess = uninetBatchDataAccess;
            _repository = repository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("File check service is starting.");
            _isRunning = true;
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            lock (_workLock)
            {
                if (_isWorking)
                {
                    return;
                }

                _isWorking = true;
            }

            try
            {
                //this processes go to mongodb amd get tha invoce data for each user 
                //1 we need to create a function that get all registered user
                //2 for each user need to call a function that get his system credentials
                //3 we 


                List<Businesses> res = _repository.GetListOfObjects<Businesses>(b => true);
               
                foreach (var Business in res)
                {
                    var businessRequest = new BusinessRequestFoeExpenses
                    {
                        BusinessId = Business.BusinessId.ToString(),
                        AdminUserid = Business.AdminUserid.ToString(),
                        FirstName = Business.FirstName,
                        LastName= Business.LastName

                    };

                    string tt = await _uninetBatchDataAccess.ExtractUserCompanyLogicExpensesAndSendAsExpensesToSideB(businessRequest);
                }
                Console.WriteLine("test");

                ////
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing user data.");
            }
            finally
            {
                lock (_workLock)
                {
                    _isWorking = false;
                }
            }

            if (!_isRunning)
            {
                _logger.LogInformation("Stopping data processing loop.");
                return;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("File check service is stopping.");
            _isRunning = false;
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    }
}
