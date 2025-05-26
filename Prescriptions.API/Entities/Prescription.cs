namespace Prescriptions.API.Entities
{
    public class Prescription
    {
        public int IdPrescription { get; set; }
        public DateTime Date      { get; set; }
        public DateTime DueDate   { get; set; }

        public int IdDoctor  { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int IdPatient  { get; set; }
        public Patient Patient { get; set; } = null!;

        public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = new List<PrescriptionMedicament>();
    }
}