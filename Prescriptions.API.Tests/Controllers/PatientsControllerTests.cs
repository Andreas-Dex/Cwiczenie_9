using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prescriptions.API.Controllers;
using Prescriptions.API.DTOs;
using Prescriptions.API.Services;
using Xunit;

namespace Prescriptions.API.Tests.Controllers
{
    public class PatientsControllerTests
    {
        [Fact]
        public async Task GetExistingPatient_ReturnsOkWithDto()
        {
            var mockService = new Mock<IPrescriptionService>();
            var dto = new PatientDetailsResponse
            {
                IdPatient = 1,
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = "Addr",
                Prescriptions = new List<PrescriptionDto>()
            };
            mockService.Setup(s => s.GetPatientDetailsAsync(1)).ReturnsAsync(dto);

            var controller = new PatientsController(mockService.Object);
            var result = await controller.Get(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetNonExistingPatient_ReturnsNotFound()
        {
            var mockService = new Mock<IPrescriptionService>();
            mockService.Setup(s => s.GetPatientDetailsAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException());

            var controller = new PatientsController(mockService.Object);
            var result = await controller.Get(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}