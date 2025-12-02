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
    }
}
