using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prescriptions.API.Data;
using Prescriptions.API.DTOs;
using Prescriptions.API.Entities;

namespace Prescriptions.API.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly AppDbContext _db;
        public PrescriptionService(AppDbContext db) => _db = db;

        public async Task<PatientDetailsResponse> GetPatientDetailsAsync(int idPatient)
        {
            var patient = await _db.Patients
                .AsNoTracking()
                .Include(p => p.Prescriptions).ThenInclude(pr => pr.Doctor)
                .Include(p => p.Prescriptions).ThenInclude(pr => pr.PrescriptionMedicaments).ThenInclude(pm => pm.Medicament)
                .SingleOrDefaultAsync(p => p.IdPatient == idPatient);

            if (patient == null)
                throw new KeyNotFoundException($"Patient {idPatient} not found");

            return new PatientDetailsResponse
            {
                IdPatient   = patient.IdPatient,
                FirstName   = patient.FirstName,
                LastName    = patient.LastName,
                Address     = patient.Address,
                Prescriptions = patient.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date           = pr.Date,
                        DueDate        = pr.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor  = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName,
                            LastName  = pr.Doctor.LastName,
                            Email     = pr.Doctor.Email
                        },
                        Medicaments = pr.PrescriptionMedicaments
                            .Select(pm => new MedicamentDto
                            {
                                IdMedicament = pm.IdMedicament,
                                Name         = pm.Medicament.Name,
                                Dose         = pm.Dose,
                                Description  = pm.Description,
                                Type         = pm.Medicament.Type
                            })
                            .ToList()
                    })
                    .ToList()
            };
        }

        public async Task AddPrescriptionAsync(AddPrescriptionRequest req)
        {
            if (req.DueDate < req.Date)
                throw new ArgumentException("DueDate must be >= Date");

            if (req.Medicaments.Count < 1 || req.Medicaments.Count > 10)
                throw new ArgumentException("Medicaments count must be between 1 and 10");

            var doctor = await _db.Doctors.FindAsync(req.DoctorId)
                         ?? throw new KeyNotFoundException($"Doctor {req.DoctorId} not found");

            var patient = await _db.Patients
                .FirstOrDefaultAsync(p =>
                    p.FirstName == req.FirstName &&
                    p.LastName  == req.LastName  &&
                    p.Address   == req.Address);

            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = req.FirstName,
                    LastName  = req.LastName,
                    Address   = req.Address
                };
                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();
            }

            var medIds = req.Medicaments.Select(m => m.IdMedicament).ToList();
            var validIds = await _db.Medicaments
                .Where(m => medIds.Contains(m.IdMedicament))
                .Select(m => m.IdMedicament)
                .ToListAsync();

            var missing = medIds.Except(validIds).ToList();
            if (missing.Any())
                throw new KeyNotFoundException($"Medicaments not found: {string.Join(", ", missing)}");

            var prescription = new Prescription
            {
                Date      = req.Date,
                DueDate   = req.DueDate,
                IdDoctor  = doctor.IdDoctor,
                IdPatient = patient.IdPatient
            };
            _db.Prescriptions.Add(prescription);
            await _db.SaveChangesAsync();

            var pmList = req.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament   = m.IdMedicament,
                Dose           = m.Dose,
                Description    = m.Description
            }).ToList();

            _db.PrescriptionMedicaments.AddRange(pmList);
            await _db.SaveChangesAsync();
        }
    }
}
