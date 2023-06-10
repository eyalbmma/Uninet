using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Constants
{
    public class ConstUninetStoredprocedure
    {
        public const string SP_updateInsertOTP = "SP_updateInsertOTP";
        public const string SP_VerifyUserByOtpUserIdAndTimeStamp = "SP_VerifyUserByOtpUserIdAndTimeStamp";
        public const string SP_GetRefreshToken = "SP_GetRefreshToken";
        public const string SP_GetUserIdByRefreshToken = "SP_GetUserIdByRefreshToken";
        public const string SP_UpdateInsertRefreshToken = "SP_UpdateInsertRefreshToken";
        public const string SP_SaveIndicationOfSentApprovalMailToCustomer = "SP_SaveIndicationOfSentApprovalMailToCustomer";
        public const string SP_SaveRefreshToken= "SP_SaveRefreshToken";
        public const string SP_AddBusinessesToUser = "SP_AddBusinessesToUser";
        public const string SP_InsertGreenvoiceJsonDetailsIntoDB = "SP_InsertGreenvoiceJsonDetailsIntoDB";
        public const string SP_GetHtmlBody = "SP_GetHtmlBody";
        public const string SP_SaveUsersExternalSystemDynamicFieldsData = "SP_SaveUsersExternalSystemDynamicFieldsData";
        

    }
}
