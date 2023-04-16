using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Classes;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Responses;
using static System.Net.WebRequestMethods;

namespace Uninet.APP.Services
{
    public class UserServiceApp : IUserServiceApp
    {
        private readonly IUserServiceDataAccess _userServiceDataAccess;
        public UserServiceApp(IUserServiceDataAccess userServiceDataAccess)
        {
            _userServiceDataAccess = userServiceDataAccess;
            //_loginRepository = loginRepository;
        }
        public async Task<bool> SendOtpByPhone(SendOtpRequest _sendOtpRequest)
        {
            try
            {
                return  await _userServiceDataAccess.SendOtpByPhone(_sendOtpRequest);
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
                return await _userServiceDataAccess.RegisterUser(RegisterUserReq);




            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public async Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses)
        {
            return await _userServiceDataAccess.RegisterBusinessToUser(userBusinesses);
        }
        public async Task<bool> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp)
        {
            return await _userServiceDataAccess.SaveIndicationOfSentApprovalMailToCustomer(Userid, otp);
        }
        //public async Task<bool> VerifyEmailLink(string Userguid)
        //{
        //    try
        //    {
        //        return await _userServiceDataAccess.VerifyEmailLink(Userguid);




        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public async Task<LoginWithOtpResponse> LoginWithOtp(string otp)
        {
            try
            {
                return await _userServiceDataAccess.LoginWithOtp(otp);




            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
