using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;

public class PullUsersData : IHostedService, IDisposable
{
    private readonly ILogger<PullUsersData> _logger;
    private Timer _timer;
    private string _filePath;
    private readonly IBatchRepository<UninetBatchContext> _repository;
    private readonly IuninetBatchDataAccess _uninetBatchDataAccess;
    private bool _isRunning = false;

    private bool _isWorking = false;
    private readonly object _workLock = new object();

    //, IUninetInputDataAccess UninetInputDataAccess, IRepository<UninetContext> repository
    public PullUsersData(ILogger<PullUsersData> logger, IuninetBatchDataAccess uninetBatchDataAccess, IBatchRepository<UninetBatchContext> repository)
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
            //List<AdminUsers> res = _repository.GetListOfObjects<AdminUsers>(b => b.AdminUserid == 325);
            //foreach (var user in res)
            //{
            //    string tt = await _uninetBatchDataAccess.PullUserDatafromExternalSystem(user.AdminUserid);
            //}

            //////////////////////////////////////////////////////
            ///


            //here we insert data into businessdata
            List<Businesses> res2 = _repository.GetListOfObjects<Businesses>(b => b.AdminUserid == 325 );

            foreach (var Business in res2)
            {
                var businessRequest = new BusinessRequestFoeExpenses
                {
                    BusinessId = Business.BusinessId.ToString(),
                    AdminUserid = Business.AdminUserid.ToString(),
                    FirstName = Business.FirstName,
                    LastName = Business.LastName

                };

                string tt = await _uninetBatchDataAccess.ExtractUserCompanyLogicExpensesAndSendAsExpensesToSideB(businessRequest);
            }
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
