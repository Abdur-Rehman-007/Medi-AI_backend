using Backend_APIs.DTOs;
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
        /// Book a new appointment
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentDto appointmentDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Data = null
                    });
                }

                var userId = int.Parse(userIdClaim.Value);

                // Parse doctor ID
                if (!int.TryParse(appointmentDto.DoctorId, out int doctorId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid doctor ID",
                        Data = null
                    });
                }

                // Validate doctor exists
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .FirstOrDefaultAsync(d => d.Id == doctorId);

                if (doctor == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Doctor not found",
                        Data = null
                    });
                }

                // Check if doctor is available
                if (doctor.IsAvailable == false)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Doctor is not available",
                        Data = null
                    });
                }

                // Check for conflicting appointments
                var appointmentDateTime = DateTime.Parse(appointmentDto.DateTime);
                var appointmentDate = DateOnly.FromDateTime(appointmentDateTime);
                var appointmentTime = TimeOnly.FromDateTime(appointmentDateTime);

                var conflictingAppointment = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId
                        && a.AppointmentDate == appointmentDate
                        && a.AppointmentTime == appointmentTime
                        && a.Status != "Cancelled")
                    .FirstOrDefaultAsync();

                if (conflictingAppointment != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "This time slot is already booked",
                        Data = null
                    });
                }

                // Get patient info
                var patient = await _context.Users.FindAsync(userId);
                if (patient == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Patient not found",
                        Data = null
                    });
                }

                // Create appointment
                var appointment = new Appointment
                {
                    PatientId = userId,
                    DoctorId = doctorId,
                    AppointmentDate = appointmentDate,
                    AppointmentTime = appointmentTime,
                    Duration = 30, // Default duration
                    Status = "Pending",
                    Symptoms = appointmentDto.Symptoms,
                    Notes = appointmentDto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                var responseData = new AppointmentResponseDto
                {
                    Id = appointment.Id.ToString(),
                    PatientId = userId.ToString(),
                    PatientName = patient.FullName,
                    DoctorId = doctor.Id.ToString(),
                    DoctorName = doctor.User.FullName,
                    Specialization = doctor.Specialization,
                    DateTime = appointmentDateTime.ToString("o"),
                    Status = appointment.Status,
                    Symptoms = appointment.Symptoms,
                    Notes = appointment.Notes,
                    Prescription = null,
                    CreatedAt = appointment.CreatedAt.HasValue ? appointment.CreatedAt.Value.ToString("o") : null
                };

                return Ok(new ApiResponse<AppointmentResponseDto>
                {
                    Success = true,
                    Message = "Appointment booked successfully",
                    Data = responseData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to book appointment: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get student/patient upcoming appointments
        /// </summary>
        [HttpGet("student/{studentId}/upcoming")]
        public async Task<IActionResult> GetStudentUpcomingAppointments(string studentId)
        {
            try
            {
                // Verify requesting user
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Data = null
                    });
                }

                if (!int.TryParse(studentId, out int patientId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid student ID",
                        Data = null
                    });
                }

                var today = DateOnly.FromDateTime(DateTime.Today);

                var appointmentList = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(a => a.Prescriptions)
                    .Where(a => a.PatientId == patientId && a.AppointmentDate >= today)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();

                var appointments = appointmentList.Select(a => new AppointmentResponseDto
                {
                    Id = a.Id.ToString(),
                    PatientId = a.PatientId.ToString(),
                    PatientName = a.Patient.FullName,
                    DoctorId = a.DoctorId.ToString(),
                    DoctorName = a.Doctor.User.FullName,
                    Specialization = a.Doctor.Specialization,
                    DateTime = new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day,
                                          a.AppointmentTime.Hour, a.AppointmentTime.Minute, a.AppointmentTime.Second).ToString("o"),
                    Status = a.Status,
                    Symptoms = a.Symptoms,
                    Notes = a.Notes,
                    Prescription = a.Prescriptions.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.Notes,
                    CreatedAt = a.CreatedAt.HasValue ? a.CreatedAt.Value.ToString("o") : null
                }).ToList();

                return Ok(new ApiResponse<List<AppointmentResponseDto>>
                {
                    Success = true,
                    Message = "Appointments retrieved successfully",
                    Data = appointments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to retrieve appointments: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get faculty/doctor appointments
        /// </summary>
        [HttpGet("Faculty/appointments")]
        [Authorize(Roles = "doctor,Faculty,admin")]
        public async Task<IActionResult> GetFacultyAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Data = null
                    });
                }

                var userId = int.Parse(userIdClaim.Value);

                // Find doctor profile
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Doctor profile not found",
                        Data = null
                    });
                }

                var appointmentList = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(a => a.Prescriptions)
                    .Where(a => a.DoctorId == doctor.Id)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToListAsync();

                var appointments = appointmentList.Select(a => new AppointmentResponseDto
                {
                    Id = a.Id.ToString(),
                    PatientId = a.PatientId.ToString(),
                    PatientName = a.Patient.FullName,
                    DoctorId = a.DoctorId.ToString(),
                    DoctorName = a.Doctor.User.FullName,
                    Specialization = a.Doctor.Specialization,
                    DateTime = new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day,
                                          a.AppointmentTime.Hour, a.AppointmentTime.Minute, a.AppointmentTime.Second).ToString("o"),
                    Status = a.Status,
                    Symptoms = a.Symptoms,
                    Notes = a.Notes,
                    Prescription = a.Prescriptions.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.Notes,
                    CreatedAt = a.CreatedAt.HasValue ? a.CreatedAt.Value.ToString("o") : null
                }).ToList();

                return Ok(new ApiResponse<List<AppointmentResponseDto>>
                {
                    Success = true,
                    Message = "Appointments retrieved successfully",
                    Data = appointments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to retrieve appointments: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{appointmentId}")]
        public async Task<IActionResult> GetAppointment(string appointmentId)
        {
            try
            {
                if (!int.TryParse(appointmentId, out int id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid appointment ID",
                        Data = null
                    });
                }

                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                        .ThenInclude(d => d.User)
                    .Include(a => a.Prescriptions)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                var responseData = new AppointmentResponseDto
                {
                    Id = appointment.Id.ToString(),
                    PatientId = appointment.PatientId.ToString(),
                    PatientName = appointment.Patient.FullName,
                    DoctorId = appointment.DoctorId.ToString(),
                    DoctorName = appointment.Doctor.User.FullName,
                    Specialization = appointment.Doctor.Specialization,
                    DateTime = new DateTime(appointment.AppointmentDate.Year, appointment.AppointmentDate.Month, appointment.AppointmentDate.Day,
                                          appointment.AppointmentTime.Hour, appointment.AppointmentTime.Minute, appointment.AppointmentTime.Second).ToString("o"),
                    Status = appointment.Status,
                    Symptoms = appointment.Symptoms,
                    Notes = appointment.Notes,
                    Prescription = appointment.Prescriptions.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.Notes,
                    CreatedAt = appointment.CreatedAt.HasValue ? appointment.CreatedAt.Value.ToString("o") : null
                };

                return Ok(new ApiResponse<AppointmentResponseDto>
                {
                    Success = true,
                    Message = "Appointment retrieved successfully",
                    Data = responseData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to retrieve appointment: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update appointment status
        /// </summary>
        [HttpPut("{appointmentId}/status")]
        [Authorize(Roles = "doctor,Faculty,admin")]
        public async Task<IActionResult> UpdateAppointmentStatus(string appointmentId, [FromBody] UpdateAppointmentStatusDto statusDto)
        {
            try
            {
                if (!int.TryParse(appointmentId, out int id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid appointment ID",
                        Data = null
                    });
                }

                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                var validStatuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };
                if (!validStatuses.Contains(statusDto.Status))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid status. Valid values: Pending, Confirmed, Completed, Cancelled",
                        Data = null
                    });
                }

                appointment.Status = statusDto.Status;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Appointment status updated successfully",
                    Data = new { status = appointment.Status }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to update status: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Cancel appointment
        /// </summary>
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(string appointmentId)
        {
            try
            {
                if (!int.TryParse(appointmentId, out int id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid appointment ID",
                        Data = null
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Data = null
                    });
                }

                var userId = int.Parse(userIdClaim.Value);

                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                if (appointment.Status == "Cancelled")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Appointment is already cancelled",
                        Data = null
                    });
                }

                appointment.Status = "Cancelled";
                appointment.CancelledBy = userId;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Appointment cancelled successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to cancel appointment: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Add prescription to appointment
        /// </summary>
        [HttpPut("{appointmentId}/prescription")]
        [Authorize(Roles = "doctor,Faculty,admin")]
        public async Task<IActionResult> AddPrescription(string appointmentId, [FromBody] AddPrescriptionDto prescriptionDto)
        {
            try
            {
                if (!int.TryParse(appointmentId, out int id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid appointment ID",
                        Data = null
                    });
                }

                var appointment = await _context.Appointments
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Appointment not found",
                        Data = null
                    });
                }

                // Create prescription
                var prescription = new Prescription
                {
                    AppointmentId = id,
                    Diagnosis = "Consultation completed",
                    Notes = prescriptionDto.Prescription,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Prescriptions.Add(prescription);

                // Update appointment status to Completed
                appointment.Status = "Completed";
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Prescription added successfully",
                    Data = new { prescriptionId = prescription.Id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to add prescription: {ex.Message}",
                    Data = null
                });
            }
        }
    }

    // DTOs for Appointments
    public class CreateAppointmentDto
    {
        public string PatientId { get; set; } = null!;
        public string? PatientName { get; set; }
        public string DoctorId { get; set; } = null!;
        public string? DoctorName { get; set; }
        public string? Specialization { get; set; }
        public string DateTime { get; set; } = null!; // ISO 8601 format
        public string? Symptoms { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class AppointmentResponseDto
    {
        public string Id { get; set; } = null!;
        public string PatientId { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string DoctorId { get; set; } = null!;
        public string DoctorName { get; set; } = null!;
        public string Specialization { get; set; } = null!;
        public string DateTime { get; set; } = null!;
        public string? Status { get; set; }
        public string? Symptoms { get; set; }
        public string? Notes { get; set; }
        public string? Prescription { get; set; }
        public string? CreatedAt { get; set; }
    }

    public class UpdateAppointmentStatusDto
    {
        public string Status { get; set; } = null!;
    }

    public class AddPrescriptionDto
    {
        public string Prescription { get; set; } = null!;
    }
}
