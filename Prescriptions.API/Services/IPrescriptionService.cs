using System.Threading.Tasks;
using Prescriptions.API.DTOs;

namespace Prescriptions.API.Services
{
    public interface IPrescriptionService
    {
        Task<PatientDetailsResponse> GetPatientDetailsAsync(int idPatient);
        Task AddPrescriptionAsync(AddPrescriptionRequest request);
    }
}