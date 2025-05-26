using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prescriptions.API.Controllers;
using Prescriptions.API.DTOs;
using Prescriptions.API.Services;
using Xunit;

namespace Prescriptions.API.Tests.Controllers
{
    public class PrescriptionControllerTests
    {
        [Fact]
        public async Task Post_ValidRequest_ReturnsOk()
        {
            var mockService = new Mock<IPrescriptionService>();
            var controller = new PrescriptionController(mockService.Object);
            var request = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = "Test Addr",
                Medicaments = new List<MedicamentDto>()
            };

            var result = await controller.Post(request);

            Assert.IsType<OkResult>(result);
            mockService.Verify(s => s.AddPrescriptionAsync(request), Times.Once);
        }

        [Fact]
        public async Task Post_TooManyMedicaments_ReturnsBadRequest()
        {
            var mockService = new Mock<IPrescriptionService>();
            var controller = new PrescriptionController(mockService.Object);
            var request = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = "Test Addr",
                Medicaments = Enumerable.Range(1, 11)
                    .Select(i => new MedicamentDto { IdMedicament = i, Dose = 1, Description = "" })
                    .ToList()
            };
            mockService.Setup(s => s.AddPrescriptionAsync(request))
                .ThrowsAsync(new ArgumentException("Max 10 medicaments"));

            var result = await controller.Post(request);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Max 10 medicaments", bad.Value);
        }

        [Fact]
        public async Task Post_InvalidMedicaments_ReturnsNotFound()
        {
            var mockService = new Mock<IPrescriptionService>();
            var controller = new PrescriptionController(mockService.Object);
            var request = new AddPrescriptionRequest
            {
                Date = DateTime.Today,
                DueDate = DateTime.Today.AddDays(1),
                DoctorId = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = "Test Addr",
                Medicaments = new List<MedicamentDto>()
            };
            mockService.Setup(s => s.AddPrescriptionAsync(request))
                .ThrowsAsync(new KeyNotFoundException("Some medicaments not found"));

            var result = await controller.Post(request);

            var nf = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Some medicaments not found", nf.Value);
        }
    }
}
