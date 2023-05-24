using Microsoft.AspNetCore.Mvc;
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
using Uninet.Domain.StoredProcedures.Requests;
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

        public async Task<bool> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId)
        {
            try
            {
                var jsonObject = new
                {
                    listInputLabelDetails = spInputExternalSystemCompanyDetails.ListInputLabelDetails,
                    userid = UserId,
                    ExternalSystemId = spInputExternalSystemCompanyDetails.ExternalSystemId.ToString()
                };

                // Convert the JSON object to string
                var jsonString = JsonConvert.SerializeObject(jsonObject);
                var UserParam = new
                {
                    jsonInput = jsonString
                    
                };
                
               


                bool spresult = ExecuteGetSP(ConstUninetStoredprocedure.SP_SaveUsersExternalSystemDynamicFieldsData, UserParam);

               

                    return spresult;
                

            }
            catch (Exception ex) { return false; };
        }

        public async Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId)
        {
            try
            {
                /*
                ExternalSystemCustomizeFieldResult

                public string BusinessLogUrl { get; set; }
                public  List<CustomizedDataLIst> listdata { get; set; }  
                public string VideoLink { get; set; }
                */
                List<CustomizedDataLIst> listdata = new List<CustomizedDataLIst>();
                var res = _repository.GetListOfObjects<ExternalSystemDynamicFields>(x => x.ExternalSystemId == ExternalSystemId);
                var res_logo_video= _repository.GetFirstObject< LUT_UninetExternalSystems>(x=>x.SyestemId== ExternalSystemId);
                foreach (var item in res)
                {
                    var customizedData = new CustomizedDataLIst
                    {
                        FieldLabelName = item.FieldLabelName,
                        FieldLabelValue=item.FieldLabelValue
                    };

                    listdata.Add(customizedData);
                }
                var ExternalSystemCustomizeFieldResultResult = new ExternalsystemCompanyTotalDetails
                {
                    ListInputLabelDetails = listdata,
                    LogoIcon = res_logo_video.Logo,
                    VideoLink = res_logo_video.Video
                };


                return ExternalSystemCustomizeFieldResultResult;

            }
            catch (Exception ex) { return null; };
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
        //public async Task<bool> VerifyEmailLink(string Userguid)
        //{
        //    try
        //    {
        //        var Adminuserobj = _repository.GetFirstObject<AdminUsers>(x => x.GuidVerification == Userguid );// && x.Password == model.P
        //        if (Adminuserobj != null) 
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex) { return false; }
        //}

        public bool ExecuteGetSP(string spName, object parameters)
        {
            var result = _repository.ExecuteGetSP<AddBusinessToUserResult>(spName, parameters);
            return result.ToList()[0].Result;
        }


        public async Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses)
        {
            try
            {
                var businessobjects = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == userBusinesses.Userid).ToList();
               

                        var existingCompanyInnerId = (await _repository.GetAllAsync<LutCompanies>())
                  .Where(b => businessobjects.Any(r => r.BusinessId == b.CompanyInnerId))
                  .Select(b => b.CompanyInnerId)
                  .ToList(); // Convert to List

               






                //get number of companies from json 
                int count = userBusinesses.BusinessRequests.Count;
                if (existingCompanyInnerId.Count != count )
                {
                    if (count > 0)
                    {

                        foreach (var businessRequest in userBusinesses.BusinessRequests)
                        {
                            var newCompany = new LutCompanies
                            {
                                CompanyName = null,
                                CompanyEmail = null
                                // Set other properties as needed
                            };

                            _repository.Create<LutCompanies>(newCompany);
                            int lastInsertedCompanyinneridId = newCompany.CompanyInnerId;
                            businessRequest.BusinessId = lastInsertedCompanyinneridId;




                        }


                    }



                    var existingBusinessIds = (await _repository.GetAllAsync<Businesses>())
                 .Where(b => userBusinesses.BusinessRequests.Any(r => r.BusinessId == b.BusinessId))
                 .Select(b => b.BusinessId)
                 .ToList(); // Convert to List

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
                        var dataTable = new DataTable();
                        dataTable.Columns.Add("BusinessId", typeof(int));
                        dataTable.Columns.Add("BusinessType", typeof(int));
                        dataTable.Columns.Add("OrganizationRole", typeof(string));
                        dataTable.Columns.Add("OrganizationName", typeof(string));
                        dataTable.Columns.Add("OrganizationType", typeof(int));
                        dataTable.Columns.Add("ExternalSystemId", typeof(int));

                        foreach (var businessRequest in userBusinesses.BusinessRequests)
                        {
                            dataTable.Rows.Add(
                                businessRequest.BusinessId,
                                businessRequest.BusinessType,
                                businessRequest.OrganizationRole,
                                businessRequest.OrganizationName,
                                businessRequest.OrganizationType,
                                businessRequest.ExternalSystemId
                            );
                        }

                        var parameter = new SqlParameter("@BusinessRequests", SqlDbType.NVarChar)
                        {
                            Value = JsonConvert.SerializeObject(dataTable, Formatting.None)
                        };

                        var UserParam = new
                        {
                            UserId = userBusinesses.Userid,
                            BusinessRequests = parameter.Value
                        };

                        bool spresult = ExecuteGetSP(ConstUninetStoredprocedure.SP_AddBusinessesToUser, UserParam);

                        var result = new AddBusinessToUserResult
                        {
                            Result = spresult,
                            BusinessRequests = existingBusinessRequests
                        };

                        return result;


                    }
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
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return null;
            }
        }

        public async Task<bool> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp)
        {
            try
            {
                var UserParam = new { userid = Userid , Otp = otp };

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

                    //FirstName = RegisterUserReq.FirstName,
                    //LastName = RegisterUserReq.LastName,
                    //PhoneNumber = RegisterUserReq.PhoneNumber,
                    DateCreated = DateTime.Now,
                    ValidUser = false,
                    Email = RegisterUserReq.Email,
                    passwordEncrypted=RegisterUserReq.Password
                };

                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == RegisterUserReq.Email );//&& x.passwordEncrypted == RegisterUserReq.Password
                if (result != null)
                {
                    result.passwordEncrypted = RegisterUserReq.Password;
                    _repository.Update(result);
                    return result.AdminUserid;//user exist
                }
                else
                { 
                    _repository.Create<AdminUsers>(NewUser);//new user
                    // Retrieve the last inserted identity value

                    int lastInsertedId = NewUser.AdminUserid;
                   // int lastInsertedId = _repository.GetLastInsertedId(NewUser);
                    // Return the AdminUserId
                    return lastInsertedId;
                   
                }
                   
               
            }
            catch (Exception ex)
            {
                return 0;//exception
            }
        }

        //
        public async Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest)
        {
            try
            {
                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == _LoginWithEmailPasswordRequest.Email && x.passwordEncrypted== _LoginWithEmailPasswordRequest.Password);// && x.Password == model.Password x.Email == "admin@abc.com
                var Response = new LoginWithEmailandPasswordResponse
                {

                    Userid = result.AdminUserid
                };
               return Response;
            }
            catch (Exception ex)
            {
                return null;
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
                        Userid = res[0].Userid
                       
                       
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
