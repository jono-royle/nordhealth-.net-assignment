using Api.Controllers;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetAppUnitTests
{
    [TestClass]
    public sealed class AppointmentControllerTests
    {
        private AppointmentController _controller;
        private Mock<IDBRepository<Appointment>> _dbRepository;
        private Mock<IEmailService> _emailService;

        private static Guid _appointmentId = Guid.NewGuid();
        private static Guid _appointmentWithinHourId = Guid.NewGuid();
        private static Animal _animal = new Animal
        {
            Id = Guid.NewGuid(),
            Name = "TestAnimal"
        };
        private static Veterinarian _vet = new Veterinarian
        {
            Id = Guid.NewGuid(),
            Name = "TestVet"
        };
        private static Customer _customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = "TestCustomer",
            Email = "Test@test.com"
        };
        private Appointment _appointment = new Appointment
        {
            Id = _appointmentId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            AnimalId = _animal.Id,
            Animal = _animal,
            CustomerId = _customer.Id,
            Customer = _customer,
            VeterinarianId = _vet.Id,
            Veterinarian = _vet,
            Status = AppointmentStatus.Scheduled
        };

        private Appointment _appointmentWithinHour = new Appointment
        {
            Id = _appointmentWithinHourId,
            StartTime = DateTime.UtcNow.AddMinutes(5),
            EndTime = DateTime.UtcNow.AddHours(1),
            AnimalId = _animal.Id,
            Animal = _animal,
            CustomerId = _customer.Id,
            Customer = _customer,
            VeterinarianId = _vet.Id,
            Veterinarian = _vet,
            Status = AppointmentStatus.Scheduled
        };

        [TestInitialize]
        public void Setup()
        {
            _dbRepository = new Mock<IDBRepository<Appointment>>();
            _emailService = new Mock<IEmailService>();
            _dbRepository.Setup(r => r.GetByIdAsync(_appointmentId, a => a.Customer, a => a.Veterinarian, a => a.Animal)).ReturnsAsync(_appointment);
            _dbRepository.Setup(r => r.GetByIdAsync(_appointmentWithinHourId, a => a.Customer, a => a.Veterinarian, a => a.Animal)).ReturnsAsync(_appointmentWithinHour);
            _controller = new AppointmentController(_emailService.Object, _dbRepository.Object);
        }

        [TestMethod]
        public async Task GetAppointment_ReturnsOkAppointment()
        {
            var result = await _controller.GetAppointment(_appointmentId);
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            var returned = ok.Value as AppointmentDTO;
            Assert.IsNotNull(returned);
            Assert.AreEqual(_appointment.StartTime, returned.StartTime);
        }

        [TestMethod]
        public async Task GetNullAppointment_ReturnsNotFound()
        {
            var result = await _controller.GetAppointment(Guid.NewGuid());
            var notFound = result.Result as NotFoundResult;
            Assert.IsNotNull(notFound);
        }

        [TestMethod]
        public async Task UpdateAppointmentStatus_ReturnsOk()
        {
            var result = await _controller.UpdateAppointmentStatus(_appointmentId, AppointmentStatus.Cancelled);
            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            var returned = ok.Value as AppointmentDTO;
            Assert.IsNotNull(returned);
            Assert.AreEqual(AppointmentStatus.Cancelled, returned.AppointmentStatus);
            _dbRepository.Verify(r => r.UpdateAsync(It.IsAny<Appointment>()), Times.Once);
            _emailService.Verify(e => e.SendEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAppointmentStatusNotAllowedValue_ReturnsBadRequest()
        {
            var result = await _controller.UpdateAppointmentStatus(_appointmentId, AppointmentStatus.InProgress);
            var badRequestInProgress = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestInProgress);
            result = await _controller.UpdateAppointmentStatus(_appointmentId, AppointmentStatus.NoShow);
            var noShowInProgress = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(noShowInProgress);
        }

        [TestMethod]
        public async Task UpdateAppointmentStatusToCancelledWithinHour_ReturnsBadRequest()
        {
            var result = await _controller.UpdateAppointmentStatus(_appointmentWithinHourId, AppointmentStatus.Cancelled);
            var badRequestInProgress = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestInProgress);
        }
    }
}
