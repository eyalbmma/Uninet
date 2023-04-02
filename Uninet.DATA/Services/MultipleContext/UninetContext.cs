using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.DATA.Services.MultipleContext
{
    public partial class UninetContext : DbContext
    {
        public UninetContext(DbContextOptions options) : base(options)
        {



        }

        public virtual DbSet<TestResponse> TestResponse { get; set; }

        //public virtual DbSet<HospitalUserModel> HospitalUserModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {

            base.OnModelCreating(modelBuilder);

            //builder.Entity<PatientData>().HasNoKey();

            //builder.Entity<HospitalUserModel>().HasNoKey();

            OnModelCreatingPartial(modelBuilder);
        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);





    }


}
