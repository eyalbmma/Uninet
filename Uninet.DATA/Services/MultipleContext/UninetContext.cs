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
            public virtual DbSet<ApprovalMailIndication> ApprovalMailIndication { get; set; }
        
        //public virtual DbSet<HospitalUserModel> HospitalUserModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            
                modelBuilder.Entity<ApprovalMailIndication>().HasNoKey();
            modelBuilder.Entity<LoginWithOtpResponse>().HasNoKey();
            modelBuilder.Entity<SaveRefreshTokenResponse>().HasNoKey();
            modelBuilder.Entity<RefreshResponse>().HasNoKey();
            modelBuilder.Entity<AdminUsers>().HasKey(u => new { u.AdminUserid, u.Email, u.PhoneNumber });

            //builder.Entity<HospitalUserModel>().HasNoKey();

            OnModelCreatingPartial(modelBuilder);
        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





    }


}
