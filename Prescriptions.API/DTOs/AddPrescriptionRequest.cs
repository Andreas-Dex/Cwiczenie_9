using System;
using System.Collections.Generic;

namespace Prescriptions.API.DTOs
{
    public class AddPrescriptionRequest
    {
        public int DoctorId     { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName  { get; set; } = null!;
        public string Address   { get; set; } = null!;
        public DateTime Date    { get; set; }
        public DateTime DueDate { get; set; }

        public List<MedicamentDto> Medicaments { get; set; } = new();
    }
}