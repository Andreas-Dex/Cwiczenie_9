namespace Prescriptions.API.Entities
{
    public class Patient
    {
        public int IdPatient  { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName  { get; set; } = null!;
        public string Address   { get; set; } = null!;
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}