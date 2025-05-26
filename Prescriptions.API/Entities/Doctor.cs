namespace Prescriptions.API.Entities
{
    public class Doctor
    {
        public int IdDoctor { get; set; }
        public string FirstName  { get; set; } = null!;
        public string LastName   { get; set; } = null!;
        public string Email      { get; set; } = null!;
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}