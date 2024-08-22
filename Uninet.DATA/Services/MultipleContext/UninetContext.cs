using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Entities;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Responses;

namespace Uninet.DATA.Services.MultipleContext
{
    public partial class UninetContext : DbContext
    {
        public UninetContext(DbContextOptions options) : base(options)
        {



        }
        
            
            


        //public virtual DbSet<UserCreditCardHolderList> UserCreditCardHolderList { get; set; }
        //public virtual DbSet<UserDetails> UserDetails { get; set; }
        public virtual DbSet<UserCreditCardHolder> UserCreditCardHolder { get; set; }

        public virtual DbSet<TestResponse> TestResponse { get; set; }
        public virtual DbSet<LoginWithOtpResponse> LoginWithOtpResponse { get; set; }
        public virtual DbSet<SaveRefreshTokenResponse> SaveRefreshTokenResponse { get; set; }
        public virtual DbSet<RefreshResponse> RefreshResponse { get; set; }
        public virtual DbSet<AddUserCredentialsSystemResult> AddUserCredentialsSystemResult { get; set; }
        
        public virtual DbSet<InsertdatatoJobbatchlogResult> InsertdatatoJobbatchlogResult { get; set; }
        
           
        public virtual DbSet<AdminUsers> AdminUsers { get; set; }
        public virtual DbSet<LUTIcountSourceWebhookCompanyMapping> LUTIcountSourceWebhookCompanyMapping { get; set; }
        public virtual DbSet<Jobbatchlog> Jobbatchlog { get; set; }
        public virtual DbSet<VerifyUserByOtpUserIdAndTimeStampResponse> VerifyUserByOtpUserIdAndTimeStampResponse { get; set; }
        public virtual DbSet<UsersExternalSystemDynamicFields> UsersExternalSystemDynamicFields { get; set; }

        public virtual DbSet<SubUserCredentials> SubUserCredentials { get; set; }
        
         public virtual DbSet<ExternalSystem> ExternalSystem { get; set; }
        public virtual DbSet<BusinessPartnersEmails> BusinessPartnersEmails { get; set; }

        public virtual DbSet<InviteBusinessPartnerResult> InviteBusinessPartnerResult { get; set; }
        public virtual DbSet<ApprovalMailIndication> ApprovalMailIndication { get; set; }
        public virtual DbSet<Businesses> Businesses { get; set; }

        public virtual DbSet<BusinessData> BusinessData { get; set; }
        //public virtual DbSet<HospitalUserModel> HospitalUserModel { get; set; }
        
        public virtual DbSet<MainSubCopmaniesMasters> MainSubCopmaniesMasters { get; set; }
        public virtual DbSet<SubCompanysID> SubCompanysID { get; set; }
        public virtual DbSet<SaveIndicationOfSentApprovalMailToCustomerResponse> SaveIndicationOfSentApprovalMailToCustomerResponse { get; set; }
        public virtual DbSet<AddBusinessToUserResult> AddBusinessToUserResult { get; set; }

        public virtual DbSet<AddBusinessDataToSQLFromGreenINvoiceResponse> AddBusinessDataToSQLFromGreenINvoiceResponse { get; set; }
        public virtual DbSet<OTPHtmlBody> OTPHtmlBody { get; set; }
        public virtual DbSet<ExternalSystemDynamicFields> ExternalSystemDynamicFields { get; set; }
        public virtual DbSet<LUT_UninetExternalSystems> LUT_UninetExternalSystems { get; set; }
        public virtual DbSet<LutCompanies> LutCompanies { get; set; }
        public virtual DbSet<SendOtpViaMailResponse> SendOtpViaMailResponse { get; set; }
        public virtual DbSet<SystemsEndpoints> SystemsEndpoints { get; set; }

        public virtual DbSet<CompanyPulledDataLog> CompanyPulledDataLog { get; set; }
        public virtual DbSet<GetUserIdByRefreshTokenResponse> GetUserIdByRefreshTokenResponse { get; set; }
        public DbSet<LUT_ExtrenalFieldsType> LUT_ExtrenalFieldsTypes { get; set; }
        public DbSet<FirstTimeConsoleIndication> FirstTimeConsoleIndication { get; set; }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SystemsEndpoints>().HasKey(u => new { u.Id, u.ExternalSystemId, u.Endpoint ,u.MethodeType });
            
                modelBuilder.Entity<FirstTimeConsoleIndication>().HasKey(u => new { u.Userid, u.Mainorganization, u.Subcompanyid });
            modelBuilder.Entity<CompanyPulledDataLog>().HasKey(u => new { u.CompanyVatid});
            modelBuilder.Entity<VerifyUserByOtpUserIdAndTimeStampResponse>().HasNoKey();

            modelBuilder.Entity<UsersExternalSystemDynamicFields>()
        .HasKey(e => new { e.Companyid,e.SubCompayId, e.Userid, e.ExternalSystemId, e.FieldLabelName });

            

            modelBuilder.Entity<InsertdatatoJobbatchlogResult>().HasNoKey();
            modelBuilder.Entity<InviteBusinessPartnerResult>().HasNoKey();
            modelBuilder.Entity<VerifyUserByOtpUserIdAndTimeStampResponse>().HasNoKey();
            modelBuilder.Entity<ApprovalMailIndication>().HasNoKey();
            modelBuilder.Entity<LoginWithOtpResponse>().HasNoKey();
            modelBuilder.Entity<SaveRefreshTokenResponse>().HasNoKey();
            modelBuilder.Entity<RefreshResponse>().HasNoKey();
            modelBuilder.Entity<AddUserCredentialsSystemResult>().HasNoKey();
            modelBuilder.Entity<AdminUsers>().HasKey(u => new { u.AdminUserid, u.Email, u.PhoneNumber });
            modelBuilder.Entity<Businesses>().HasKey(u => new { u.AdminUserid, u.BusinessId });
            modelBuilder.Entity<ExternalSystem>().HasKey(u => new { u.ExternalSystemID });
           
            


            //modelBuilder.Entity<UserDetails>().HasNoKey();
            //modelBuilder.Entity<UserCreditCardHolderList>().HasNoKey();
            modelBuilder.Entity<UserCreditCardHolder>().HasKey(u => new { u.UserId, u.BusinessId, u.SubCompanyId ,u.ExternalSystemId });
            modelBuilder.Entity<LUTIcountSourceWebhookCompanyMapping>().HasKey(u => new { u.WebHookSourceid });

            
            modelBuilder.Entity<MainSubCopmaniesMasters>().HasKey(u => new { u.MainCompanyId,u.SubCopmanyId });
            modelBuilder.Entity<SubCompanysID>().HasKey(u => new { u.SubCompanyid });
            modelBuilder.Entity<SubUserCredentials>().HasKey(u => new { u.Userid, u.CompanyId, u.SubCompanyId,u.SubUserId });
            modelBuilder.Entity<BusinessData>().HasKey(u => new { u.UserId, u.BusinessId,u.JsonDocumentid });

            

            modelBuilder.Entity<BusinessPartnersEmails>().HasKey(u => new { u.VatId, u.OrganizationId, u.UserId,u.SubCompanyId,u.Email });
            modelBuilder.Entity<AddBusinessToUserResult>().HasNoKey();
            modelBuilder.Entity<SendOtpViaMailResponse>().HasNoKey();
            modelBuilder.Entity<SaveIndicationOfSentApprovalMailToCustomerResponse>().HasNoKey();
            
            modelBuilder.Entity<AddBusinessDataToSQLFromGreenINvoiceResponse>().HasNoKey();
            
            modelBuilder.Entity<OTPHtmlBody>().HasNoKey();
            modelBuilder.Entity<ExternalSystemDynamicFields>().HasNoKey();
            modelBuilder.Entity<GetUserIdByRefreshTokenResponse>().HasNoKey();
            

            modelBuilder.Entity<LUT_UninetExternalSystems>().HasNoKey();
            modelBuilder.Entity<LutCompanies>().HasKey(u => new { u.CompanyInnerId });
            //builder.Entity<HospitalUserModel>().HasNoKey();
            modelBuilder.Entity<LUT_ExtrenalFieldsType>().HasKey(u => new { u.FieldType });
            modelBuilder.Entity<Jobbatchlog>().HasKey(u => new { u.id });
            OnModelCreatingPartial(modelBuilder);


        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





    }


}
