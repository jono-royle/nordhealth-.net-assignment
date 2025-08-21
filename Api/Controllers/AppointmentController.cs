using Api.Data;
using Api.DTOs;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly IDBRepository<Appointment> _appointmentRepository;

    public AppointmentController(IEmailService emailService, IDBRepository<Appointment> appRepository)
    {
        _emailService = emailService;
        _appointmentRepository = appRepository;
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDTO>> CreateAppointment([FromBody] Appointment appointment)
    {
        if (appointment == null)
        {
            return BadRequest("Appointment cannot be null.");
        }

        if (appointment.AnimalId == Guid.Empty || appointment.CustomerId == Guid.Empty || appointment.VeterinarianId == Guid.Empty)
        {
            return BadRequest("AnimalId and CustomerId and VeterinarianId are required.");
        }

        if(appointment.StartTime >= appointment.EndTime)
        {
            return BadRequest("Start time must be before end time");
        }

        var overlappingAppointments = await _appointmentRepository
            .ScanAsync(s => s.Where(a =>
        (a.VeterinarianId == appointment.VeterinarianId || a.CustomerId == appointment.CustomerId) &&
        (
            (appointment.StartTime >= a.StartTime && appointment.StartTime < a.EndTime) ||   // starts during
            (appointment.EndTime > a.StartTime && appointment.EndTime <= a.EndTime) ||       // ends during
            (appointment.StartTime <= a.StartTime && appointment.EndTime >= a.EndTime)       // fully covers
        )));

        if (overlappingAppointments.Any())
        {
            return BadRequest("New appointment overlaps an existing appointment for Veterinarian or Customer");
        }


        appointment.Id = Guid.NewGuid();
        appointment.Status = AppointmentStatus.Scheduled;
        appointment.StartTime = appointment.StartTime.ToUniversalTime();
        appointment.EndTime = appointment.EndTime.ToUniversalTime();

        await _appointmentRepository.AddAsync(appointment);

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, GetAppointmentData(appointment));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDTO>> GetAppointment(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, a => a.Customer, a => a.Veterinarian, a => a.Animal);
        if (appointment == null)
        {
            return NotFound();
        }
        var dto = GetAppointmentData(appointment);
        return Ok(dto);
    }

    [HttpGet("appointments/{vetId}")]
    public async Task<ActionResult<List<AppointmentDTO>>> ListVetAppointments(Guid vetId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if(startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }
        var appointments = await _appointmentRepository
            .ScanAsync(s => s.Where(a => a.VeterinarianId == vetId && a.StartTime > startDate && a.StartTime <= endDate)
            .Include(a => a.Veterinarian)
            .Include(a => a.Animal)
            .Include(a => a.Customer));

        var appointmentData = appointments.Select(a => GetAppointmentData(a)).ToList();
        return Ok(appointmentData);
    }

    [HttpPatch("appointments/update/{appointmentId}")]
    public async Task<ActionResult<AppointmentDTO>> UpdateAppointmentStatus(Guid appointmentId, AppointmentStatus newStatus)
    {
        if(newStatus != AppointmentStatus.Scheduled && newStatus != AppointmentStatus.Completed && newStatus != AppointmentStatus.Cancelled)
        {
            return BadRequest("Appointment status must be set to Scheduled, Completed or Canceled");
        }
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, a => a.Customer, a => a.Veterinarian, a => a.Animal);
        if (appointment == null)
        {
            return NotFound();
        }
        if(newStatus == AppointmentStatus.Cancelled)
        {
            if (appointment.StartTime > DateTime.UtcNow && appointment.StartTime - DateTime.UtcNow < TimeSpan.FromHours(1))
            {
                return BadRequest("Cannot cancel an appointment less than 1 hour before start time");
            }
            if (appointment.Customer != null && !string.IsNullOrEmpty(appointment.Customer.Email)) 
            {
                try
                {
                    await _emailService.SendEmailAsync(appointment.Customer.Email);
                }
                catch (Exception ex) 
                {
                    //Using console in place of error logging solution
                    Console.WriteLine($"Email failed to send, exception: {ex}");
                }
            }
        }
        appointment.Status = newStatus;
        await _appointmentRepository.UpdateAsync(appointment);
        var dto = GetAppointmentData(appointment);
        return Ok(dto);
    }

    private AppointmentDTO GetAppointmentData(Appointment appointment)
    {
        AppointmentDTO appointmentDTO = new AppointmentDTO
        {
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            AppointmentStatus = appointment.Status,
            OwnerName = appointment.Customer?.Name,
            AnimalName = appointment.Animal?.Name,
            VetName = appointment.Veterinarian?.Name
        };

        return appointmentDTO;

    }
}