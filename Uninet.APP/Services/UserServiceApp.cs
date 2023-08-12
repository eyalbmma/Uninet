using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.Domain.Classes;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Requests;
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

        public async Task<Dictionary<int, string>> GetExternalSystems()
        {
            try
            {
               return await _userServiceDataAccess.GetExternalSystems();    
            }
            catch (Exception ex) { return null; }

            
        }
        public async Task<ResponseResendOtp> ResendOtp(ResentOtpRequest resentOtpRequest, int DecryptedUserId)
        {
            try
            {
               return  await _userServiceDataAccess.ResendOtp(resentOtpRequest, DecryptedUserId);
            }
            catch (Exception ex)
            {

                var res = new ResponseResendOtp
                {
                    Success = true,
                    Desc = ex.Message
                };
                return res;
            }
        }
        public async Task<bool> ResetPassword(ResetPasswordRequestcs resetpasswordrequest)
        {
            try
            {
                return await _userServiceDataAccess.ResetPassword(resetpasswordrequest);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        public async Task<bool> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                return await _userServiceDataAccess.ForgotPassword(forgotPasswordRequest);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<GoogleSigninResponse> GoogleSignIn(GoogleSignInModel googlesignInrequest)
        {
            try
            {
                return await _userServiceDataAccess.GoogleSignIn(googlesignInrequest);
            }
            catch(Exception ex)
            {
                var resgoogleSignin = new GoogleSigninResponse
                {
                    Success = true,
                    Message = ex.Message
                };

                return resgoogleSignin;
               
            }
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
        public async Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId)
        {
            try
            {
                return await _userServiceDataAccess.GetExternalCustomizedFieldByExternaLSystemID(ExternalSystemId);
            }
            catch (Exception ex) { return null; }
        }

        public async Task<bool> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId)
        {

            try
            {
                return await _userServiceDataAccess.SaveExternalCustomizedExternalSystemId(spInputExternalSystemCompanyDetails,  UserId);
            }
            catch (Exception ex) { return false; }
        }
        public async Task<ReturnRegisterUser> RegisterUser(RegisterUserRequest RegisterUserReq)
        {
            try
            {
                return await _userServiceDataAccess.RegisterUser(RegisterUserReq);




            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses)
        {
            return await _userServiceDataAccess.RegisterBusinessToUser(userBusinesses);
        }
        public async Task<ApprovalMailIndication> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp)
        {
            return await _userServiceDataAccess.SaveIndicationOfSentApprovalMailToCustomer( Userid,  otp);
        }


        public async Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest)
        {
            return await _userServiceDataAccess.LoginWithEmailPasswordRequest(_LoginWithEmailPasswordRequest);
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
        public async Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp, string EncryptedUser)
        {
            try
            {
                return await _userServiceDataAccess.RegisterWithOtpAndEncryptedUser(otp, EncryptedUser);




            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
