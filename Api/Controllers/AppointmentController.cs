using Api.Data;
using Api.DTOs;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IEmailService _emailService;

    public AppointmentController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public ActionResult<Animal> CreateAppointment([FromBody] Appointment appointment)
    {
        if (appointment == null)
        {
            return BadRequest("Appointment cannot be null.");
        }

        if (appointment.AnimalId == Guid.Empty || appointment.CustomerId == Guid.Empty)
        {
            return BadRequest("AnimalId and CustomerId are required.");
        }

        appointment.Id = Guid.NewGuid();

        AppointmentData.Appointments.Add(appointment);

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpGet("{id}")]
    public ActionResult<Appointment> GetAppointment(Guid id)
    {
        var appointment = AppointmentData.Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }
        return Ok(appointment);
    }

    [HttpGet("appointments/{vetId}")]
    public ActionResult<List<AppointmentDTO>> ListVetAppointments(Guid vetId, DateTime startDate, DateTime endDate)
    {
        if(startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }
        var appointments = AppointmentData.Appointments.Where(a => a.VeterinarianId == vetId && a.StartTime > startDate && a.StartTime <= endDate);
        var appointmentData = appointments.Select(a => GetAppointmentData(a)).ToList();
        return Ok(appointmentData);
    }

    [HttpPatch("appointments/update/{appointmentId}")]
    public async Task<ActionResult<Appointment>> UpdateAppointmentStatus(Guid appointmentId, AppointmentStatus newStatus)
    {
        if(newStatus != AppointmentStatus.Scheduled && newStatus != AppointmentStatus.Completed && newStatus != AppointmentStatus.Cancelled)
        {
            return BadRequest("Appointment status must be set to Scheduled, Completed or Canceled");
        }
        var appointment = AppointmentData.Appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null)
        {
            return NotFound();
        }
        if(newStatus == AppointmentStatus.Cancelled)
        {
            if (appointment.StartTime > DateTime.Now && appointment.StartTime - DateTime.Now < TimeSpan.FromHours(1))
            {
                return BadRequest("Cannot cancel an appointment less than 1 hour before start time");
            }

            var animal = AnimalData.Animals.FirstOrDefault(a => a.OwnerId == appointment.CustomerId);
            if(animal != null)
            {
                //Send email to user
                await _emailService.SendEmailAsync(animal.OwnerEmail);
            }
        }
        appointment.Status = newStatus;
        return Ok(appointment);
    }

    private AppointmentDTO GetAppointmentData(Appointment appointment)
    {
        AppointmentDTO appointmentDTO = new AppointmentDTO
        {
            StartTime = appointment.StartTime,
            AppointmentStatus = appointment.Status
        };
        var animal = AnimalData.Animals.FirstOrDefault(a => a.Id == appointment.AnimalId);
        if(animal != null)
        {
            appointmentDTO.OwnerName = animal.OwnerName;
            appointmentDTO.AnimalName = animal.Name;
        }
        return appointmentDTO;

    }
}