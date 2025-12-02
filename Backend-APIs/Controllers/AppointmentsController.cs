using Backend_APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly MediaidbContext _context;

        public AppointmentsController(MediaidbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all appointments (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Duration,
                    a.Status,
                    a.Symptoms,
                    a.CreatedAt,
                    Patient = new
                    {
                        a.Patient.Id,
                        a.Patient.FullName,
                        a.Patient.Email,
                        a.Patient.PhoneNumber
                    },
                    Doctor = new
                    {
                        a.Doctor.Id,
                        a.Doctor.Specialization,
                        DoctorName = a.Doctor.User.FullName
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        /// <summary>
        /// Get appointments for current user (patient or doctor)
        /// </summary>
        [HttpGet("my-appointments")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<Appointment> query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User);

            if (userRole == "doctor")
            {
                // Get appointments for this doctor
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor profile not found" });
                }
                query = query.Where(a => a.DoctorId == doctor.Id);
            }
            else
            {
                // Get appointments for this patient
                query = query.Where(a => a.PatientId == userId);
            }

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Duration,
                    a.Status,
                    a.Symptoms,
                    a.Notes,
                    a.CreatedAt,
                    Patient = new
                    {
                        a.Patient.Id,
                        a.Patient.FullName,
                        a.Patient.PhoneNumber
                    },
                    Doctor = new
                    {
                        a.Doctor.Id,
                        a.Doctor.Specialization,
                        a.Doctor.ConsultationFee,
                        a.Doctor.RoomNumber,
                        DoctorName = a.Doctor.User.FullName
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Prescriptions)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Duration,
                    a.Status,
                    a.Symptoms,
                    a.Notes,
                    a.CancellationReason,
                    a.CreatedAt,
                    a.UpdatedAt,
                    Patient = new
                    {
                        a.Patient.Id,
                        a.Patient.FullName,
                        a.Patient.Email,
                        a.Patient.PhoneNumber,
                        a.Patient.DateOfBirth,
                        a.Patient.Gender
                    },
                    Doctor = new
                    {
                        a.Doctor.Id,
                        a.Doctor.Specialization,
                        a.Doctor.ConsultationFee,
                        a.Doctor.RoomNumber,
                        DoctorName = a.Doctor.User.FullName,
                        DoctorPhone = a.Doctor.User.PhoneNumber
                    },
                    Prescriptions = a.Prescriptions.Select(p => new
                    {
                        p.Id,
                        p.Diagnosis,
                        p.Notes,
                        p.CreatedAt
                    })
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            return Ok(appointment);
        }

        /// <summary>
        /// Book a new appointment
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "patient")]
        public async Task<ActionResult<Appointment>> BookAppointment([FromBody] Appointment appointment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            // Validate doctor exists
            var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
            if (doctor == null)
            {
                return BadRequest(new { message = "Doctor not found" });
            }

            // Check if doctor is available
            if (doctor.IsAvailable == false)
            {
                return BadRequest(new { message = "Doctor is not available" });
            }

            // Check for conflicting appointments
            var conflictingAppointment = await _context.Appointments
                .Where(a => a.DoctorId == appointment.DoctorId
                    && a.AppointmentDate == appointment.AppointmentDate
                    && a.AppointmentTime == appointment.AppointmentTime
                    && a.Status != "cancelled")
                .FirstOrDefaultAsync();

            if (conflictingAppointment != null)
            {
                return BadRequest(new { message = "This time slot is already booked" });
            }

            appointment.PatientId = userId;
            appointment.Status = "scheduled";
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, 
                new { message = "Appointment booked successfully", appointmentId = appointment.Id });
        }

        /// <summary>
        /// Update appointment status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusDto statusDto)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            var validStatuses = new[] { "scheduled", "confirmed", "completed", "cancelled", "no-show" };
            if (!validStatuses.Contains(statusDto.Status.ToLower()))
            {
                return BadRequest(new { message = "Invalid status" });
            }

            appointment.Status = statusDto.Status.ToLower();
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment status updated successfully", status = appointment.Status });
        }

        /// <summary>
        /// Cancel appointment
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentDto cancelDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            if (appointment.Status == "cancelled")
            {
                return BadRequest(new { message = "Appointment is already cancelled" });
            }

            appointment.Status = "cancelled";
            appointment.CancellationReason = cancelDto.Reason;
            appointment.CancelledBy = userId;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment cancelled successfully" });
        }

        /// <summary>
        /// Get upcoming appointments
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<object>>> GetUpcomingAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var today = DateOnly.FromDateTime(DateTime.Today);

            IQueryable<Appointment> query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate >= today && a.Status != "cancelled");

            if (userRole == "doctor")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor profile not found" });
                }
                query = query.Where(a => a.DoctorId == doctor.Id);
            }
            else
            {
                query = query.Where(a => a.PatientId == userId);
            }

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Take(5)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    a.Symptoms,
                    Patient = new
                    {
                        a.Patient.FullName,
                        a.Patient.PhoneNumber
                    },
                    Doctor = new
                    {
                        a.Doctor.Specialization,
                        DoctorName = a.Doctor.User.FullName,
                        a.Doctor.RoomNumber
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }

    // DTOs for update operations
    public class UpdateStatusDto
    {
        public string Status { get; set; } = null!;
    }

    public class CancelAppointmentDto
    {
        public string Reason { get; set; } = null!;
    }
}
