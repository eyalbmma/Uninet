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

        public virtual DbSet<TestResponse> TestResponse { get; set; }
        public virtual DbSet<LoginWithOtpResponse> LoginWithOtpResponse { get; set; }
        public virtual DbSet<SaveRefreshTokenResponse> SaveRefreshTokenResponse { get; set; }
        public virtual DbSet<RefreshResponse> RefreshResponse { get; set; }
        public virtual DbSet<AdminUsers> AdminUsers { get; set; }
        public virtual DbSet<VerifyUserByOtpUserIdAndTimeStampResponse> VerifyUserByOtpUserIdAndTimeStampResponse { get; set; }
        
        public virtual DbSet<ApprovalMailIndication> ApprovalMailIndication { get; set; }
        public virtual DbSet<Businesses> Businesses { get; set; }
        //public virtual DbSet<HospitalUserModel> HospitalUserModel { get; set; }
        
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
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SystemsEndpoints>().HasKey(u => new { u.Id, u.ExternalSystemId, u.Endpoint ,u.MethodeType });
            
            modelBuilder.Entity<CompanyPulledDataLog>().HasKey(u => new { u.CompanyVatid});
            modelBuilder.Entity<VerifyUserByOtpUserIdAndTimeStampResponse>().HasNoKey();

            modelBuilder.Entity<VerifyUserByOtpUserIdAndTimeStampResponse>().HasNoKey();
            modelBuilder.Entity<ApprovalMailIndication>().HasNoKey();
            modelBuilder.Entity<LoginWithOtpResponse>().HasNoKey();
            modelBuilder.Entity<SaveRefreshTokenResponse>().HasNoKey();
            modelBuilder.Entity<RefreshResponse>().HasNoKey();
            modelBuilder.Entity<AdminUsers>().HasKey(u => new { u.AdminUserid, u.Email, u.PhoneNumber });
            modelBuilder.Entity<Businesses>().HasKey(u => new { u.AdminUserid, u.BusinessId });
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

            OnModelCreatingPartial(modelBuilder);


        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





    }


}
