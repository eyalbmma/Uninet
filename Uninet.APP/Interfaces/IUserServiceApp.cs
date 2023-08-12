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
        public Task<ResponseResendOtp> ResendOtp(ResentOtpRequest resentOtpRequest, int DecryptedUserId);
        public Task<bool> ResetPassword(ResetPasswordRequestcs resetpasswordrequest);
        public Task<GoogleSigninResponse> GoogleSignIn(GoogleSignInModel googlesignInrequest);
        public Task<bool>  SendOtpByPhone(SendOtpRequest _sendOtpRequest);
        public Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp,string EncryptedUser);
        public Task<ApprovalMailIndication> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp);

        public Task<ReturnRegisterUser> RegisterUser(RegisterUserRequest RegisterUserReq);

        public Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest);

        // public Task<bool> VerifyEmailLink(string Userguid);

        public Task<Dictionary<int, string>> GetExternalSystems();
        public Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses);

        public Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId);

        public Task<bool> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails,string UserId);


        public Task<bool> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);

    }
}
