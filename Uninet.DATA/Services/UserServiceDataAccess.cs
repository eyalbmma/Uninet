using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Classes;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using static System.Net.WebRequestMethods;

namespace Uninet.DATA.Services
{
    public class UserServiceDataAccess: IUserServiceDataAccess
    {
        private readonly IRepository<UninetContext> _repository;

        public UserServiceDataAccess(IRepository<UninetContext> repository)//, IloginRepository loginRepository
        {


            _repository = repository;

        }

        protected string Generate_otp()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            for (int i = 0; i < 4; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString())) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }

        protected async Task<bool> SendOtp(string from, string To, string Body)
        {
            try
            {
                string accountSid = "ACcdd2a5b796d15d9d0f1bf28214de5adf";// Environment.GetEnvironmentVariable("ACcdd2a5b796d15d9d0f1bf28214de5adf");
                string authToken = "e18d4227bb3ace9310f71971e28866c8";// Environment.GetEnvironmentVariable("e18d4227bb3ace9310f71971e28866c8");

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                  body: "מספר חד פעמי הוא :" + Body,
                  from: new Twilio.Types.PhoneNumber(from),//("+19142668348"),
                  to: new Twilio.Types.PhoneNumber(To)//("+972526350902")
              );
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> VerifyEmailLink(string Userguid)
        {
            try
            {
                var Adminuserobj = _repository.GetFirstObject<AdminUsers>(x => x.GuidVerification == Userguid );// && x.Password == model.P
                if (Adminuserobj != null) 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { return false; }
        }

        public bool ExecuteGetSP(string spName, object parameters)
        {
            var result = _repository.ExecuteGetSP<AddBusinessToUserResult>(spName, parameters);
            return result.ToList()[0].Result;
        }


        public async Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses)
        {
            try
            {


                //AddBusinessToUserResult res = new AddBusinessToUserResult();
                var existingBusinessIds = (await _repository.GetAllAsync<Businesses>())
                .Where(b => userBusinesses.BusinessRequests.Any(r => r.BusinessId == b.BusinessId))
                .Select(b => b.BusinessId);

                var existingBusinessRequests = userBusinesses.BusinessRequests
                    .Where(br => existingBusinessIds.Contains(br.BusinessId))
                    .ToList();


                if (existingBusinessRequests.Count > 0)
                {
                    var result = new AddBusinessToUserResult
                    {
                        Result = false,
                        BusinessRequests = existingBusinessRequests
                    };
                    return result;
                }
                else
                {


                    ////////////////////////////////////////////////////
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("BusinessId", typeof(int));
                    dataTable.Columns.Add("BusinessName", typeof(string));
                    dataTable.Columns.Add("BusinessEmail", typeof(string));
                    dataTable.Columns.Add("DelearType", typeof(int));
                    dataTable.Columns.Add("BusinessType", typeof(int));
                    foreach (var businessRequest in userBusinesses.BusinessRequests)
                    {

                        dataTable.Rows.Add(
                            businessRequest.BusinessId,
                            businessRequest.BusinessName,
                            businessRequest.BusinessEmail,
                            businessRequest.DelearType,
                            businessRequest.BusinessType);
                    }
                    var json = JsonConvert.SerializeObject(dataTable, Formatting.None);
                    var parameter = new SqlParameter("@BusinessRequests", SqlDbType.NVarChar)
                    {
                        Value = json
                    };
                    var UserParam = new
                    {
                        UserId = userBusinesses.Userid,
                        BusinessRequests = parameter.Value // retrieve the value of the parameter
                    };
                    //var spresult = _repository.ExecuteGetSP<AddBusinessToUserResult>(ConstUninetStoredprocedure.SP_AddBusinessesToUser, UserParam);
                   bool spresult=ExecuteGetSP(ConstUninetStoredprocedure.SP_AddBusinessesToUser, UserParam);

                    if (spresult)
                    {
                        var result = new AddBusinessToUserResult
                        {
                            Result = true,
                            BusinessRequests = existingBusinessRequests
                        };
                        return result;
                    }
                    else
                    {
                        var result = new AddBusinessToUserResult
                        {
                            Result = false,
                            BusinessRequests = null
                        };
                        return result;
                    }
                   
                }

            }
            catch (Exception ex)
            {
                return null;
            }

            
        }
        public async Task<bool> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string usernewguid)
        {
            try
            {
                var UserParam = new { userid = Userid ,newguid= usernewguid };

                var res = _repository.ExecuteGetSP<ApprovalMailIndication>(ConstUninetStoredprocedure.SP_SaveIndicationOfSentApprovalMailToCustomer, UserParam);
                return res.ToList()[0].result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<int> RegisterUser(RegisterUserRequest RegisterUserReq)
        {
            try
            {
                var NewUser = new AdminUsers
                {

                    FirstName = RegisterUserReq.FirstName,
                    LastName = RegisterUserReq.LastName,
                    PhoneNumber = RegisterUserReq.PhoneNumber,
                    DateCreated = DateTime.Now,
                    ValidUser = false,
                    Email = RegisterUserReq.Email
                };
              
                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == RegisterUserReq.Email && x.PhoneNumber== RegisterUserReq.PhoneNumber);
                if (result != null)
                {
                    return 0;
                }
                else
                { 
                    _repository.Create<AdminUsers>(NewUser);
                    // Retrieve the last inserted identity value

                    int lastInsertedId = NewUser.AdminUserid;
                   // int lastInsertedId = _repository.GetLastInsertedId(NewUser);
                    // Return the AdminUserId
                    return lastInsertedId;
                   
                }
                   
               
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public async Task<LoginWithOtpResponse> LoginWithOtp(string otp)
        {
            try
            {
                var Otparam = new { Otp = otp };
                var res = _repository.ExecuteGetSP<LoginWithOtpResponse>(ConstUninetStoredprocedure.SP_GetUserByOtp, Otparam).ToList();
                if (res != null)
                {
                    return new LoginWithOtpResponse()
                    {
                        Userid = res[0].Userid,
                        FirstName = res[0].FirstName,
                       
                    };
                }
                else
                    return null;




            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<bool> SendOtpByPhone(SendOtpRequest _sendOtpRequest)
        {
            try
            {
                var result = _repository.GetFirstObject<AdminUsers>(x => x.PhoneNumber == _sendOtpRequest.Phone);// && x.Password == model.Password x.Email == "admin@abc.com
                if (result != null)
                {
                    string Otp = "";
                    Otp = Generate_otp();
                    bool SendOtpres =await SendOtp("+19142668348", "+972526350902", Otp);
                    if (SendOtpres)
                    {

                        var OtpRaw = new { Userid = result.AdminUserid, otp = Otp };
                        var res = _repository.ExecuteGetSP<UpdateOtpResponse>(ConstUninetStoredprocedure.SP_updateInsertOTP, OtpRaw).ToList();



                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
