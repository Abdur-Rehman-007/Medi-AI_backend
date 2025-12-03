using Backend_APIs.DTOs;
using Backend_APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly MediaidbContext _context;

        public DoctorsController(MediaidbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all doctors with user details
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Select(d => new
                {
                    d.Id,
                    d.UserId,
                    d.Specialization,
                    d.LicenseNumber,
                    d.Qualification,
                    d.Experience,
                    d.ConsultationFee,
                    d.RoomNumber,
                    d.Bio,
                    d.AverageRating,
                    d.TotalRatings,
                    d.IsAvailable,
                    d.CreatedAt,
                    User = new
                    {
                        d.User.Id,
                        d.User.FullName,
                        d.User.Email,
                        d.User.PhoneNumber,
                        d.User.ProfileImageUrl
                    }
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>
        /// Get doctor by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Doctorschedules)
                .Include(d => d.Doctorreviews)
                    .ThenInclude(r => r.Patient)
                .Where(d => d.Id == id)
                .Select(d => new
                {
                    d.Id,
                    d.UserId,
                    d.Specialization,
                    d.LicenseNumber,
                    d.Qualification,
                    d.Experience,
                    d.ConsultationFee,
                    d.RoomNumber,
                    d.Bio,
                    d.AverageRating,
                    d.TotalRatings,
                    d.IsAvailable,
                    d.CreatedAt,
                    User = new
                    {
                        d.User.Id,
                        d.User.FullName,
                        d.User.Email,
                        d.User.PhoneNumber,
                        d.User.ProfileImageUrl,
                        d.User.Gender,
                        d.User.Department
                    },
                    Schedules = d.Doctorschedules.Select(s => new
                    {
                        s.Id,
                        s.DayOfWeek,
                        s.StartTime,
                        s.EndTime,
                        s.IsActive
                    }),
                    Reviews = d.Doctorreviews.Select(r => new
                    {
                        r.Id,
                        r.Rating,
                        r.Review,
                        r.CreatedAt,
                        PatientName = r.Patient.FullName
                    })
                })
                .FirstOrDefaultAsync();

            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }

            return Ok(doctor);
        }

        /// <summary>
        /// Get doctors by specialization
        /// </summary>
        [HttpGet("specialization/{specialization}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetDoctorsBySpecialization(string specialization)
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.Specialization.ToLower().Contains(specialization.ToLower()))
                .Select(d => new
                {
                    d.Id,
                    d.UserId,
                    d.Specialization,
                    d.Qualification,
                    d.Experience,
                    d.ConsultationFee,
                    d.AverageRating,
                    d.TotalRatings,
                    d.IsAvailable,
                    User = new
                    {
                        d.User.FullName,
                        d.User.ProfileImageUrl
                    }
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>
        /// Update doctor availability
        /// </summary>
        [HttpPatch("{id}/availability")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] bool isAvailable)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }

            doctor.IsAvailable = isAvailable;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Availability updated successfully", isAvailable });
        }

        /// <summary>
        /// Update doctor profile
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "doctor,admin")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var existingDoctor = await _context.Doctors.FindAsync(id);
            if (existingDoctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }

            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.LicenseNumber = doctor.LicenseNumber;
            existingDoctor.Qualification = doctor.Qualification;
            existingDoctor.Experience = doctor.Experience;
            existingDoctor.ConsultationFee = doctor.ConsultationFee;
            existingDoctor.RoomNumber = doctor.RoomNumber;
            existingDoctor.Bio = doctor.Bio;
            existingDoctor.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating doctor" });
            }

            return Ok(new { message = "Doctor updated successfully" });
        }

        /// <summary>
        /// Get available doctors for appointment booking
        /// </summary>
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsAvailable == true && d.User.IsActive == true)
                .Select(d => new
                {
                    d.Id,
                    d.Specialization,
                    d.ConsultationFee,
                    d.AverageRating,
                    d.Experience,
                    User = new
                    {
                        d.User.FullName,
                        d.User.ProfileImageUrl
                    }
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>
        /// Search doctors by name, specialization, or other criteria
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchDoctors(
            [FromQuery] string? query,
            [FromQuery] string? specialization,
            [FromQuery] bool? availableOnly = false)
        {
            try
            {
                var doctorsQuery = _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Doctorreviews)
                    .Where(d => d.User.IsActive == true)
                    .AsQueryable();

                // Filter by availability
                if (availableOnly == true)
                {
                    doctorsQuery = doctorsQuery.Where(d => d.IsAvailable == true);
                }

                // Filter by specialization
                if (!string.IsNullOrWhiteSpace(specialization))
                {
                    doctorsQuery = doctorsQuery.Where(d => 
                        d.Specialization.ToLower().Contains(specialization.ToLower()));
                }

                // Search by name or specialization
                if (!string.IsNullOrWhiteSpace(query))
                {
                    doctorsQuery = doctorsQuery.Where(d =>
                        d.User.FullName.ToLower().Contains(query.ToLower()) ||
                        d.Specialization.ToLower().Contains(query.ToLower()));
                }

                var doctors = await doctorsQuery
                    .Select(d => new DoctorSearchDto
                    {
                        DoctorId = d.Id,
                        DoctorName = d.User.FullName,
                        Specialization = d.Specialization,
                        Qualifications = d.Qualification,
                        ExperienceYears = d.Experience,
                        ConsultationFee = d.ConsultationFee,
                        IsAvailable = d.IsAvailable ?? false,
                        AverageRating = d.AverageRating.HasValue ? (double)d.AverageRating.Value : null,
                        TotalReviews = d.TotalRatings,
                        ProfileImageUrl = d.User.ProfileImageUrl,
                        Bio = d.Bio
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<DoctorSearchDto>>
                {
                    Success = true,
                    Message = $"Found {doctors.Count} doctor(s)",
                    Data = doctors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Search failed: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get list of all specializations with doctor count
        /// </summary>
        [HttpGet("specializations")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSpecializations()
        {
            try
            {
                var specializations = await _context.Doctors
                    .Where(d => d.User.IsActive == true && !string.IsNullOrEmpty(d.Specialization))
                    .GroupBy(d => d.Specialization)
                    .Select(g => new SpecializationDto
                    {
                        Specialization = g.Key,
                        DoctorCount = g.Count()
                    })
                    .OrderBy(s => s.Specialization)
                    .ToListAsync();

                return Ok(new ApiResponse<List<SpecializationDto>>
                {
                    Success = true,
                    Message = "Specializations retrieved successfully",
                    Data = specializations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to retrieve specializations: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get available time slots for a doctor on a specific date
        /// </summary>
        [HttpGet("{id}/available-slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSlots(int id, [FromQuery] string date)
        {
            try
            {
                // Validate date
                if (!DateOnly.TryParse(date, out var requestedDate))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid date format. Use yyyy-MM-dd",
                        Data = null
                    });
                }

                // Check if date is in the past
                if (requestedDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot get slots for past dates",
                        Data = null
                    });
                }

                // Get doctor
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Doctorschedules)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (doctor == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Doctor not found",
                        Data = null
                    });
                }

                // Get day of week
                var dayOfWeek = requestedDate.DayOfWeek.ToString();

                // Get doctor's schedule for that day
                var schedule = doctor.Doctorschedules
                    .FirstOrDefault(s => s.DayOfWeek == dayOfWeek && s.IsActive == true);

                if (schedule == null)
                {
                    return Ok(new ApiResponse<AvailableSlotsResponseDto>
                    {
                        Success = true,
                        Message = "Doctor is not available on this day",
                        Data = new AvailableSlotsResponseDto
                        {
                            Date = date,
                            DoctorId = id,
                            DoctorName = doctor.User.FullName,
                            Slots = new List<AvailableSlotDto>()
                        }
                    });
                }

                // Get existing appointments for this date
                var existingAppointments = await _context.Appointments
                    .Where(a => a.DoctorId == id 
                        && a.AppointmentDate == requestedDate 
                        && a.Status != "Cancelled")
                    .Select(a => a.AppointmentTime)
                    .ToListAsync();

                // Generate time slots
                var slots = new List<AvailableSlotDto>();
                var startTime = schedule.StartTime;
                var endTime = schedule.EndTime;
                var slotDuration = 30; // 30 minutes per slot

                var currentTime = startTime;
                while (currentTime < endTime)
                {
                    var isBooked = existingAppointments.Any(apt => apt == currentTime);
                    
                    slots.Add(new AvailableSlotDto
                    {
                        Time = currentTime.ToString("HH:mm"),
                        Duration = slotDuration,
                        Available = !isBooked
                    });

                    currentTime = currentTime.AddMinutes(slotDuration);
                }

                var response = new AvailableSlotsResponseDto
                {
                    Date = date,
                    DoctorId = id,
                    DoctorName = doctor.User.FullName,
                    Slots = slots
                };

                return Ok(new ApiResponse<AvailableSlotsResponseDto>
                {
                    Success = true,
                    Message = "Available slots retrieved successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to retrieve available slots: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get doctor's schedule for the week
        /// </summary>
        [HttpGet("{id}/schedule")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorSchedule(int id)
        {
            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.User)
                    .Include(d => d.Doctorschedules)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (doctor == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Doctor not found",
                        Data = null
                    });
                }

                var schedules = doctor.Doctorschedules
                    .Where(s => s.IsActive == true)
                    .Select(s => new DoctorScheduleDto
                    {
                        ScheduleId = s.Id,
                        DayOfWeek = s.DayOfWeek,
                        StartTime = s.StartTime.ToString("HH:mm"),
                        EndTime = s.EndTime.ToString("HH:mm"),
                        IsAvailable = s.IsActive ?? false
                    })
                    .OrderBy(s => GetDayOrder(s.DayOfWeek))
                    .ToList();

                return Ok(new ApiResponse<List<DoctorScheduleDto>>
                {
                    Success = true,
                    Message = "Schedule retrieved successfully",
                    Data = schedules
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to retrieve schedule: {ex.Message}",
                    Data = null
                });
            }
        }

        private int GetDayOrder(string dayOfWeek)
        {
            return dayOfWeek switch
            {
                "Monday" => 1,
                "Tuesday" => 2,
                "Wednesday" => 3,
                "Thursday" => 4,
                "Friday" => 5,
                "Saturday" => 6,
                "Sunday" => 7,
                _ => 8
            };
        }
    }
}
