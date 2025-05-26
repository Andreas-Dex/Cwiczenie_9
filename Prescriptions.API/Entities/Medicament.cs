namespace Prescriptions.API.Entities
{
    public class Medicament
    {
        public int IdMedicament { get; set; }
        public string Name      { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Type      { get; set; } = null!;

        public ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = new List<PrescriptionMedicament>();
    }
}