using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prescriptions.API.Data;
using Prescriptions.API.DTOs;
using Prescriptions.API.Entities;
using Prescriptions.API.Services;
using Xunit;

namespace Prescriptions.API.Tests.Services
{
    public class PrescriptionServiceTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new AppDbContext(options);
            ctx.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "Doc", LastName = "Tor", Email = "doc@example.com" });
            ctx.Medicaments.Add(new Medicament { IdMedicament = 1, Name = "Med1", Description = "Desc", Type = "T" });
            ctx.SaveChanges();
            return ctx;
        }

        [Fact]
        public async Task AddPrescription_CreatesNewPatientAndPrescription()
        {
            using var ctx = CreateContext();
            var svc = new PrescriptionService(ctx);
            var req = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Address = "Addr",
                Medicaments = new List<MedicamentDto>
                {
                    new() { IdMedicament = 1, Dose = 2, Description = "D" }
                }
            };

            await svc.AddPrescriptionAsync(req);

            Assert.Single(ctx.Patients);
            Assert.Single(ctx.Prescriptions);
            Assert.Single(ctx.PrescriptionMedicaments);
        }

        [Fact]
        public async Task AddPrescription_ExistingPatient_ReusesPatient()
        {
            using var ctx = CreateContext();
            ctx.Patients.Add(new Patient { FirstName = "Alice", LastName = "Smith", Address = "Addr" });
            ctx.SaveChanges();
            var svc = new PrescriptionService(ctx);
            var req = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Address = "Addr",
                Medicaments = new List<MedicamentDto>
                {
                    new() { IdMedicament = 1, Dose = 2, Description = "D" }
                }
            };

            await svc.AddPrescriptionAsync(req);

            Assert.Single(ctx.Patients);
            Assert.Single(ctx.Prescriptions);
        }

        [Fact]
        public async Task AddPrescription_InvalidMedicament_ThrowsKeyNotFound()
        {
            using var ctx = CreateContext();
            var svc = new PrescriptionService(ctx);
            var req = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Bob",
                LastName = "Builder",
                Address = "Addr",
                Medicaments = new List<MedicamentDto>
                {
                    new() { IdMedicament = 999, Dose = 1, Description = "" }
                }
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.AddPrescriptionAsync(req));
        }

        [Fact]
        public async Task AddPrescription_TooManyMedicaments_ThrowsArgumentException()
        {
            using var ctx = CreateContext();
            var svc = new PrescriptionService(ctx);
            var meds = Enumerable.Range(1, 11)
                .Select(i => new MedicamentDto { IdMedicament = 1, Dose = 1, Description = "" })
                .ToList();
            var req = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Bob",
                LastName = "Builder",
                Address = "Addr",
                Medicaments = meds
            };

            await Assert.ThrowsAsync<ArgumentException>(() => svc.AddPrescriptionAsync(req));
        }

        [Fact]
        public async Task GetPatientDetails_ReturnsCorrectDto()
        {
            using var ctx = CreateContext();
            var patient = new Patient { FirstName = "P", LastName = "Q", Address = "A" };
            ctx.Patients.Add(patient);
            ctx.SaveChanges();
            var pres = new Prescription
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today,
                IdDoctor = 1,
                IdPatient = patient.IdPatient
            };
            ctx.Prescriptions.Add(pres);
            ctx.SaveChanges();
            ctx.PrescriptionMedicaments.Add(new PrescriptionMedicament
            {
                IdPrescription = pres.IdPrescription,
                IdMedicament = 1,
                Dose = 3,
                Description = "D"
            });
            ctx.SaveChanges();

            var svc = new PrescriptionService(ctx);
            var dto = await svc.GetPatientDetailsAsync(patient.IdPatient);

            Assert.Equal(patient.IdPatient, dto.IdPatient);
            Assert.Single(dto.Prescriptions);
            Assert.Single(dto.Prescriptions[0].Medicaments);
        }

        [Fact]
        public async Task GetPatientDetails_NonExisting_ThrowsKeyNotFound()
        {
            using var ctx = CreateContext();
            var svc = new PrescriptionService(ctx);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => svc.GetPatientDetailsAsync(999));
        }
    }
}
