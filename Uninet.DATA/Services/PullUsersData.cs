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
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
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

            //await WriteToTableAsync(1, "JobStarted", "");
            ////List<AdminUsers> res = _repository.GetListOfObjects<AdminUsers>(b => b.AdminUserid == 325 || b.AdminUserid == 327);


            //List<AdminUsers> res = _repository.GetListOfObjects<AdminUsers>(b => b.AdminUserid == 563);
            //remark eya;
            /*
             AdminUserid 563 is eliavh25@gmail.comthe sign in to uninet 
             his company is 1322 also known as 025482111 
             this is the company detials
                uninetsuplier
                ת.ז./ע.מ./ח.פ.: 025482111

                this means that the prcess behind the scene
                get all dicument created by this company and insert  them into BusinessData table
                and the job loops on this table and send  to their clients email  regarding their digital document that was created 


             */

            //List<AdminUsers> res = _repository.GetListOfObjects<AdminUsers>(b => true);
            //foreach (var user in res)
            //{
            //    string tt = await _uninetBatchDataAccess.PullUserDatafromExternalSystem(user.AdminUserid);
            //}

            ////////////////////////////////////////////////////////



            //////here we insert data into businessdata
            // List<Businesses> res2 = _repository.GetListOfObjects<Businesses>(b => b.AdminUserid == 325 || b.AdminUserid==327);
             //List<Businesses> res2 = _repository.GetListOfObjects<Businesses>(b => b.AdminUserid == 563);

            //List<Businesses> res2 = _repository.GetListOfObjects<Businesses>(b => true);

            //foreach (var Business in res2)
            //{
            //    var businessRequest = new BusinessRequestFoeExpenses
            //    {
            //        BusinessId = Business.BusinessId.ToString(),
            //        AdminUserid = Business.AdminUserid.ToString(),
            //        FirstName = Business.FirstName,
            //        LastName = Business.LastName

            //    };

            //    string tt = await _uninetBatchDataAccess.ExtractUserCompanyLogicExpensesAndSendAsExpensesToSideB(businessRequest);
            //}
            //await WriteToTableAsync(2, "JobEnded", "");
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

    private async Task WriteToTableAsync(int Taskid ,string TaskDesc ,string text)
    {
        try
        {
            // Define the time zone ID for Israel
            string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

            // Get the Israel time zone
            TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

            // Convert server's DateTime.Now to Israel local time
            DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
            var entity = new Jobbatchlog // Replace YourTableName with the appropriate class name
            {
                TaskId = Taskid,
                TaskDesc = TaskDesc,
                date = israelNow,
                text = text
            };
            await _repository.CreateAsync(entity);
        }
        catch(Exception ex) { }
    }

}
