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
    public async Task<ActionResult<Animal>> CreateAppointment([FromBody] Appointment appointment)
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

        await _appointmentRepository.AddAsync(appointment);

        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(Guid id)
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
    public async Task<ActionResult<List<AppointmentDTO>>> ListVetAppointments(Guid vetId, DateTime startDate, DateTime endDate)
    {
        if(startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }
        var appointments = await _appointmentRepository
            .QueryAsync().Where(a => a.VeterinarianId == vetId && a.StartTime > startDate && a.StartTime <= endDate)
            .Include(a => a.Veterinarian)
            .Include(a => a.Animal)
            .Include(a => a.Customer)
            .ToListAsync();

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
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, a => a.Customer, a => a.Veterinarian, a => a.Animal);
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
            if (appointment.Customer != null) 
            {
                await _emailService.SendEmailAsync(appointment.Customer.Email);
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