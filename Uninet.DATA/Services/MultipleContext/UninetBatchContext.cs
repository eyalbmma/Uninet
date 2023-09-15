    using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Entities;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Responses;

namespace Uninet.DATA.Services.MultipleContext
{
  
    public partial class UninetBatchContext : DbContext
    {
        public UninetBatchContext(DbContextOptions options) : base(options)
        {



        }

        public virtual DbSet<TestResponse> TestResponse { get; set; }
        public virtual DbSet<LoginWithOtpResponse> LoginWithOtpResponse { get; set; }
        public virtual DbSet<SaveRefreshTokenResponse> SaveRefreshTokenResponse { get; set; }
        public virtual DbSet<RefreshResponse> RefreshResponse { get; set; }
        public virtual DbSet<BusinessData> BusinessData { get; set; }
        
        public virtual DbSet<AdminUsers> AdminUsers { get; set; }
        public virtual DbSet<Jobbatchlog> Jobbatchlog { get; set; }
        
        public virtual DbSet<ApprovalMailIndication> ApprovalMailIndication { get; set; }
        public virtual DbSet<Businesses> Businesses { get; set; }
        //public virtual DbSet<HospitalUserModel> HospitalUserModel { get; set; }

        public virtual DbSet<AddBusinessToUserResult> AddBusinessToUserResult { get; set; }

        public virtual DbSet<AddBusinessDataToSQLFromGreenINvoiceResponse> AddBusinessDataToSQLFromGreenINvoiceResponse { get; set; }
        public virtual DbSet<OTPHtmlBody> OTPHtmlBody { get; set; }
        public virtual DbSet<SystemsEndpoints> SystemsEndpoints { get; set; }


        public virtual DbSet<SendOtpViaMailResponse> SendOtpViaMailResponse { get; set; }
        public virtual DbSet<UsersExternalSystemDynamicFields> UsersExternalSystemDynamicFields { get; set; }

        public virtual DbSet<CompanyPulledDataLog> CompanyPulledDataLog { get; set; }

        public virtual DbSet<ExternalSystemDynamicFields> ExternalSystemDynamicFields { get; set; }

        public virtual DbSet<InsertBusinessData_Result> InsertBusinessData_Result { get; set; }
        public virtual DbSet<UpdateBusinessDataEmailSent_Response> UpdateBusinessDataEmailSent_Response { get; set; }
        
            
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SystemsEndpoints>().HasKey(u => new { u.Id, u.ExternalSystemId, u.Endpoint, u.MethodeType });
            modelBuilder.Entity<CompanyPulledDataLog>().HasKey(u => new { u.CompanyVatid });
            modelBuilder.Entity<ApprovalMailIndication>().HasNoKey();
            modelBuilder.Entity<LoginWithOtpResponse>().HasNoKey();
            modelBuilder.Entity<SaveRefreshTokenResponse>().HasNoKey();
            modelBuilder.Entity<RefreshResponse>().HasNoKey();
            modelBuilder.Entity<AdminUsers>().HasKey(u => new { u.AdminUserid, u.Email, u.PhoneNumber });
            modelBuilder.Entity<BusinessData>().HasKey(u => new { u.UserId, u.BusinessId, u.JsonDocumentid });
            modelBuilder.Entity<Businesses>().HasKey(u => new { u.AdminUserid, u.BusinessId });
            modelBuilder.Entity<Jobbatchlog>().HasKey(u => new { u.id});
            
            modelBuilder.Entity<UsersExternalSystemDynamicFields>().HasKey(u => new { u.Companyid, u.Userid ,u.ExternalSystemId,u.FieldLabelName });


            modelBuilder.Entity<AddBusinessToUserResult>().HasNoKey();
            modelBuilder.Entity<SendOtpViaMailResponse>().HasNoKey();

            modelBuilder.Entity<AddBusinessDataToSQLFromGreenINvoiceResponse>().HasNoKey();

            modelBuilder.Entity<OTPHtmlBody>().HasNoKey();

            modelBuilder.Entity<InsertBusinessData_Result>().HasNoKey();
            modelBuilder.Entity<UpdateBusinessDataEmailSent_Response>().HasNoKey();
            
                
            //builder.Entity<HospitalUserModel>().HasNoKey();
            modelBuilder.Entity<ExternalSystemDynamicFields>().HasNoKey();



            OnModelCreatingPartial(modelBuilder);
        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





    }
}
