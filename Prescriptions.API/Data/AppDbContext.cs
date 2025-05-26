using Microsoft.EntityFrameworkCore;
using Prescriptions.API.Entities;

namespace Prescriptions.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Medicament> Medicaments => Set<Medicament>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments => Set<PrescriptionMedicament>();

        protected override void OnModelCreating(ModelBuilder m)
        {
            m.Entity<Doctor>().HasKey(d => d.IdDoctor);
            m.Entity<Patient>().HasKey(p => p.IdPatient);
            m.Entity<Medicament>().HasKey(m => m.IdMedicament);
            m.Entity<Prescription>().HasKey(pr => pr.IdPrescription);
            m.Entity<PrescriptionMedicament>()
                .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });

            m.Entity<Prescription>()
                .HasOne(pr => pr.Doctor)
                .WithMany(d => d.Prescriptions)
                .HasForeignKey(pr => pr.IdDoctor);

            m.Entity<Prescription>()
                .HasOne(pr => pr.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(pr => pr.IdPatient);

            m.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(pr => pr.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdPrescription);

            m.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdMedicament);
        }
    }
}