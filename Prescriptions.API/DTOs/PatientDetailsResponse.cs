using System;
using System.Collections.Generic;

namespace Prescriptions.API.DTOs
{
    public class PatientDetailsResponse
    {
        public int IdPatient       { get; set; }
        public string FirstName   { get; set; } = null!;
        public string LastName    { get; set; } = null!;
        public string Address     { get; set; } = null!;
        public List<PrescriptionDto> Prescriptions { get; set; } = new();
    }

    public class PrescriptionDto
    {
        public int IdPrescription { get; set; }
        public DateTime Date      { get; set; }
        public DateTime DueDate   { get; set; }
        public DoctorDto Doctor   { get; set; } = null!;
        public List<MedicamentDto> Medicaments { get; set; } = new();
    }

    public class DoctorDto
    {
        public int IdDoctor     { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName  { get; set; } = null!;
        public string Email     { get; set; } = null!;
    }
}