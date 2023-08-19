using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Requests;
using Uninet.Domain.StoredProcedures.Responses;

namespace Uninet.DATA.Interfaces
{
    public interface IUserServiceDataAccess
    {
        public Task<ResponseResendOtp> ResendOtp(ResentOtpRequest resentOtpRequest,int DecryptedUserId,int Lang);
        public Task<ResetPasswordReponse> ResetPassword(ResetPasswordRequestcs resetpasswordrequest);
        public Task<GoogleSigninResponse> GoogleSignIn(GoogleSignInModel googlesignInrequest);
        public Task<Dictionary<int, string>> GetExternalSystems();
        public Task<bool> SendOtpByPhone(SendOtpRequest _sendOtpRequest);
        public Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp, string EncryptedUser, int Lang);
        public Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest);
        public Task<ReturnRegisterUser> RegisterUser(RegisterUserRequest RegisterUserReq);


        public Task<ApprovalMailIndication> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp);

        //public Task<bool> VerifyEmailLink(string Userguid);

        public Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses);

        public Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId);


        public Task<ResSaveExternalCustomized> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId);
        public Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
    }
}
