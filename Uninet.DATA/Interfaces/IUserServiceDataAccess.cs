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
        public Task<bool> SendOtpByPhone(SendOtpRequest _sendOtpRequest);
        public Task<LoginWithOtpResponse> LoginWithOtp(string otp);
        public Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest);
        public Task<int> RegisterUser(RegisterUserRequest RegisterUserReq);


        public Task<bool> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp);

        //public Task<bool> VerifyEmailLink(string Userguid);

        public Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses);

        public Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId);


        public Task<bool> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId);
    }
}
