using EpicMock.Data;
using EpicMock.DTOs;
using EpicMock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpicMock.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly HimDbContext context;

        public AppointmentsController(HimDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await context.Appointments.Where(a => a.Status != "Cancelled").ToListAsync();

            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound($"Appointment with ID:{id} not found.");
            }
            return Ok(appointment);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatientId(int patientId)
        {
            var patientExists = await context.Patients.AnyAsync(p => p.PatientId == patientId && p.IsActive);
            if (!patientExists)
            {
                return NotFound($"Patient with ID {patientId} not found.");
            }

            var appointments = await context.Appointments
                .Where(a => a.PatientId == patientId && a.Status != "Cancelled").OrderBy(a => a.StartTime).ToListAsync();

            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentIntakeDTO dto)
        {
            if (dto.EndTime <= dto.StartTime)
            {
                return BadRequest("EndTime must be after StartTime.");
            }

            bool hasConflict = await context.Appointments.AnyAsync(a =>
            a.Status != "Cancelled" &&
            (a.ProviderId == dto.ProviderId || (dto.RoomId != null && a.RoomId == dto.RoomId)) &&
            dto.StartTime < a.EndTime &&
            dto.EndTime > a.StartTime);

            if (hasConflict)
            {
                return Conflict("This provider or room is already booked for the requested time.");
            }

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                ProviderId = dto.ProviderId,
                RoomId = dto.RoomId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Reason = dto.Reason,
                Status = "Scheduled",
                CreatedByUserId = 1 // temp fix
            };

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();
            return Ok(appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> RescheduleAppointment(int id, AppointmentUpdateDTO dto)
        {
            var appointment = await context.Appointments.FindAsync(id);
            if (appointment == null || appointment.Status == "Cancelled")
            {
                return NotFound($"Appointment with ID:{id} not found.");
            }

            if (dto.EndTime <= dto.StartTime)
            {
                return BadRequest("EndTime must be after StartTime.");
            }

            bool hasConflict = await context.Appointments.AnyAsync(a =>
            a.AppointmentId != id &&
            a.Status != "Cancelled" &&
            (a.ProviderId == dto.ProviderId || (dto.RoomId != null && a.RoomId == dto.RoomId)) &&
            dto.StartTime < a.EndTime &&
            dto.EndTime > a.StartTime);

            if (hasConflict)
            {
                return Conflict("This provider or room is already booked for the requested time.");
            }

            appointment.ProviderId = dto.ProviderId;
            appointment.RoomId = dto.RoomId;
            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.EndTime;
            appointment.Reason = dto.Reason;
            appointment.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(appointment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await context.Appointments.FindAsync(id);
            if (appointment == null || appointment.Status == "Cancelled")
            {
                return NotFound($"Appointment with ID:{id} not found.");
            }

            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
