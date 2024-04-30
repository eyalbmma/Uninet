using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Requests;
using Uninet.Domain.StoredProcedures.Responses;

namespace Uninet.APP.Interfaces
{
    public interface IUserServiceApp
    {

        public Task<BusinessPartnerLists> InviteBusinessPartners(int userid,int Lang);
        public  Task<Q1_Q4_Result> GetQ1_Q4_Indication(Q1_Q4_Request q1q4request);
        public Task<ResponseResendOtp> ResendOtp(ResentOtpRequest resentOtpRequest, int DecryptedUserId,int Lang);
        public Task<ResetPasswordReponse> ResetPassword(ResetPasswordRequestcs resetpasswordrequest);
        public Task<GoogleSigninResponse> GoogleSignIn(GoogleSignInModel googlesignInrequest);
        public Task<bool>  SendOtpByPhone(SendOtpRequest _sendOtpRequest);
        public Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp,string EncryptedUser,int Lang);
        public Task<ApprovalMailIndication> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp);

        public Task<ReturnRegisterUser> RegisterUser(RegisterUserRequest RegisterUserReq);

        public Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest);

        // public Task<bool> VerifyEmailLink(string Userguid);
        
        public Task<string> GetWelcomeToUninet(int Lang, string userId);
        public Task<Dictionary<int, string>> GetExternalSystems(int Lang);
        public Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses);

        public Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId);

        public Task<ResSaveExternalCustomized> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails,string UserId);


        public Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);


        public Task<LoginWithEmailandPasswordResponse> VerifyEmailLink(string EmailGuidVerification);

        public Task<ActiveTabResponse> GetActiveTab(GetActiveTabRequest getActiveTabRequest);

    }
}
