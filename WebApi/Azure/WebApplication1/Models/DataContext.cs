using Azure.DataObjects;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Models
{
    /// <summary>
    /// our Database connection to Azure SQL
    /// </summary>
    public class DataContext : DbContext
    {

        private const string connectionStringName = "Name=MS_TableConnectionString";

        public DataContext() : base(connectionStringName)
        {
        }

        //Our tables in our database
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<DiagnosisCode> DiagnosisCodes { get; set; }
        public DbSet<ProcedureCode> ProcedureCodes { get; set; }
        public DbSet<PatientChatLog> PatientChatLogs { get; set; }
        public DbSet<PatientImaging> PatientImagings { get; set; }
        public DbSet<Biometric>  Biometrics { get; set; }
        public DbSet<PatientProcedure> PatientProcedures { get; set; }
        public DbSet<PatientProvider> PatientProviders{ get; set; }

        //Represents the relationships our tables have to each other
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
             .HasMany(x => x.PatientImagings)
             .WithRequired(x => x.Patient)
             .HasForeignKey(x => x.PatientId);

            modelBuilder.Entity<Patient>()
              .HasMany(x => x.PatientChatLogs)
              .WithRequired(x => x.Patient)
              .HasForeignKey(x => x.PatientId);

            modelBuilder.Entity<Patient>()
             .HasMany(x => x.Biometrics)
             .WithRequired(x => x.Patient)
             .HasForeignKey(x => x.PatientId);

            modelBuilder.Entity<Patient>()
             .HasMany(x => x.PatientProcedures)
             .WithRequired(x => x.Patient)
             .HasForeignKey(x => x.PatientId);

            modelBuilder.Entity<Provider>()
             .HasMany(x => x.PatientProcedures)
             .WithOptional(x => x.Provider)
             .HasForeignKey(x => x.ProviderId);

            modelBuilder.Entity<Patient>()
               .HasMany(x => x.PatientProviders)
               .WithRequired(x => x.Patient)
               .HasForeignKey(x => x.PatientId);

            modelBuilder.Entity<DiagnosisCode>()
                .HasMany(x => x.Patients)
                .WithRequired(x=>x.DiagnosisCode)
                .HasForeignKey(x=>x.DiagnosisCodeId);
        }
    }
}
