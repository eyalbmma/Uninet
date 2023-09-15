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
        public async Task<LoginWithEmailandPasswordResponse> VerifyEmailLink(string EmailGuidVerification)
        {
            try
            {
                return await _userServiceDataAccess.VerifyEmailLink(EmailGuidVerification);
            }
            catch
            {
                return null;
            }
        }


        public async Task<ActiveTabResponse> GetActiveTab(GetActiveTabRequest getActiveTabRequest)
        {
            try
            {
                return await _userServiceDataAccess.GetActiveTab(getActiveTabRequest);
            }
            catch(Exception ex)
            {
                var res = new ActiveTabResponse
                {
                    ActiveTabNaem = "",
                    textResponse = getActiveTabRequest.Lang == 1 ? "Failed to retriev active tab" : "כשלון באיחזור טאב פעיל "
                };
                return res;
            }
        }
        public async Task<InviteBusinessPartnerResult> InviteBusinessPartners(int userid, int Lang)
        {
            try
            {
                return await _userServiceDataAccess.InviteBusinessPartners(userid, Lang);
            }
            catch
            {
                var res = new InviteBusinessPartnerResult
                {
                    Success = false,
                    textResponse = Lang == 1 ? "Failed to send Emails" : "נכשל בשליחת המיילים "
                };
                return res;
            }
        }
        public async Task<Q1_Q4_Result> GetQ1_Q4_Indication(Q1_Q4_Request q1q4request)
        {
            try
            {
                return await _userServiceDataAccess.GetQ1_Q4_Indication(q1q4request);
            }
            catch (Exception ex) 
            {
                var Response = new Q1_Q4_Result
                {
                    Q1_Q2_InidicationRes = false,
                    Q3_InidicationRes = false
                };
                return Response;
            }
        }
        public async Task<Dictionary<int, string>> GetExternalSystems(int Lang)
        {
            try
            {
               return await _userServiceDataAccess.GetExternalSystems(Lang);    
            }
            catch (Exception ex) { return null; }

            
        }
        public async Task<ResponseResendOtp> ResendOtp(ResentOtpRequest resentOtpRequest, int DecryptedUserId, int Lang)
        {
            try
            {
               return  await _userServiceDataAccess.ResendOtp(resentOtpRequest, DecryptedUserId, Lang);
            }
            catch (Exception ex)
            {

                var res = new ResponseResendOtp
                {
                    Success = false,
                    Desc = ex.Message
                };
                return res;
            }
        }
        public async Task<ResetPasswordReponse> ResetPassword(ResetPasswordRequestcs resetpasswordrequest)
        {
            try
            {
                return await _userServiceDataAccess.ResetPassword(resetpasswordrequest);
            }
            catch (Exception ex)
            {
                var ResResetPassword = new ResetPasswordReponse
                {
                    success = false,
                    textResponse = resetpasswordrequest.Lang == 1 ? "an eror occured" : "ארעה שדיאה הסיסמה לא אופסה "
                };
                return ResResetPassword;
            }
        }

        
        public async Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                return await _userServiceDataAccess.ForgotPassword(forgotPasswordRequest);
            }
            catch (Exception ex)
            {
                var ForgotPasswordResponse = new ForgotPasswordResponse
                {
                    Success = false,
                    textResponse = forgotPasswordRequest.Lang == 1 ? "there was an error reseting your password" : "ארעה שגיאה באיפוס הסיסמה"

                };
                return ForgotPasswordResponse;
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
                    textResponse = googlesignInrequest.lang == 1 ? "verfication failed8" + ex.InnerException + ex.Message : " האימות נכשל "
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

        public async Task<ResSaveExternalCustomized> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId)
        {

            try
            {
                return await _userServiceDataAccess.SaveExternalCustomizedExternalSystemId(spInputExternalSystemCompanyDetails, UserId);
            }

            catch (Exception ex)
            {
                var res = new ResSaveExternalCustomized
                {
                    Success = false,
                    textResponse = ""
                };
                return res;
            }
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
        public async Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp, string EncryptedUser,int Lang)
        {
            try
            {
                return await _userServiceDataAccess.RegisterWithOtpAndEncryptedUser(otp, EncryptedUser, Lang);




            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
