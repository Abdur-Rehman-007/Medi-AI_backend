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
    public class MedicineRemindersController : ControllerBase
    {
        private readonly MediaidbContext _context;

        public MedicineRemindersController(MediaidbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all medicine reminders for current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMyReminders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminders = await _context.Medicinereminders
                .Where(r => r.StudentId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.MedicineName,
                    r.Dosage,
                    r.Frequency,
                    r.CustomFrequency,
                    r.Times,
                    r.StartDate,
                    r.EndDate,
                    r.Notes,
                    r.IsActive,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reminders);
        }

        /// <summary>
        /// Get active medicine reminders
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<object>>> GetActiveReminders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var today = DateOnly.FromDateTime(DateTime.Today);

            var reminders = await _context.Medicinereminders
                .Where(r => r.StudentId == userId 
                    && r.IsActive == true
                    && r.StartDate <= today
                    && (r.EndDate == null || r.EndDate >= today))
                .Select(r => new
                {
                    r.Id,
                    r.MedicineName,
                    r.Dosage,
                    r.Frequency,
                    r.Times,
                    r.StartDate,
                    r.EndDate,
                    r.Notes
                })
                .ToListAsync();

            return Ok(reminders);
        }

        /// <summary>
        /// Get reminder by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReminder(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminder = await _context.Medicinereminders
                .Include(r => r.Medicinereminderlogs)
                .Where(r => r.Id == id && r.StudentId == userId)
                .Select(r => new
                {
                    r.Id,
                    r.MedicineName,
                    r.Dosage,
                    r.Frequency,
                    r.CustomFrequency,
                    r.Times,
                    r.StartDate,
                    r.EndDate,
                    r.Notes,
                    r.IsActive,
                    r.CreatedAt,
                    r.UpdatedAt,
                    Logs = r.Medicinereminderlogs
                        .OrderByDescending(l => l.ScheduledTime)
                        .Take(10)
                        .Select(l => new
                        {
                            l.Id,
                            l.ScheduledTime,
                            l.TakenTime,
                            l.Status,
                            l.Notes
                        })
                })
                .FirstOrDefaultAsync();

            if (reminder == null)
            {
                return NotFound(new { message = "Reminder not found" });
            }

            return Ok(reminder);
        }

        /// <summary>
        /// Create a new medicine reminder
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Medicinereminder>> CreateReminder([FromBody] CreateReminderDto reminderDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminder = new Medicinereminder
            {
                StudentId = userId,
                MedicineName = reminderDto.MedicineName,
                Dosage = reminderDto.Dosage,
                Frequency = reminderDto.Frequency,
                CustomFrequency = reminderDto.CustomFrequency,
                Times = reminderDto.Times,
                StartDate = reminderDto.StartDate,
                EndDate = reminderDto.EndDate,
                Notes = reminderDto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Medicinereminders.Add(reminder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReminder), new { id = reminder.Id }, 
                new { message = "Medicine reminder created successfully", reminderId = reminder.Id });
        }

        /// <summary>
        /// Update medicine reminder
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, [FromBody] UpdateReminderDto reminderDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminder = await _context.Medicinereminders
                .FirstOrDefaultAsync(r => r.Id == id && r.StudentId == userId);

            if (reminder == null)
            {
                return NotFound(new { message = "Reminder not found" });
            }

            reminder.MedicineName = reminderDto.MedicineName;
            reminder.Dosage = reminderDto.Dosage;
            reminder.Frequency = reminderDto.Frequency;
            reminder.CustomFrequency = reminderDto.CustomFrequency;
            reminder.Times = reminderDto.Times;
            reminder.StartDate = reminderDto.StartDate;
            reminder.EndDate = reminderDto.EndDate;
            reminder.Notes = reminderDto.Notes;
            reminder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Reminder updated successfully" });
        }

        /// <summary>
        /// Toggle reminder active status
        /// </summary>
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleReminder(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminder = await _context.Medicinereminders
                .FirstOrDefaultAsync(r => r.Id == id && r.StudentId == userId);

            if (reminder == null)
            {
                return NotFound(new { message = "Reminder not found" });
            }

            reminder.IsActive = !reminder.IsActive;
            reminder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = reminder.IsActive == true ? "Reminder activated" : "Reminder deactivated",
                isActive = reminder.IsActive
            });
        }

        /// <summary>
        /// Delete medicine reminder
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminder = await _context.Medicinereminders
                .FirstOrDefaultAsync(r => r.Id == id && r.StudentId == userId);

            if (reminder == null)
            {
                return NotFound(new { message = "Reminder not found" });
            }

            _context.Medicinereminders.Remove(reminder);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reminder deleted successfully" });
        }

        /// <summary>
        /// Log medicine intake
        /// </summary>
        [HttpPost("{id}/log")]
        public async Task<IActionResult> LogIntake(int id, [FromBody] LogIntakeDto logDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);

            var reminder = await _context.Medicinereminders
                .FirstOrDefaultAsync(r => r.Id == id && r.StudentId == userId);

            if (reminder == null)
            {
                return NotFound(new { message = "Reminder not found" });
            }

            var log = new Medicinereminderlog
            {
                ReminderId = id,
                ScheduledTime = logDto.ScheduledTime,
                TakenTime = DateTime.UtcNow,
                Status = logDto.Status ?? "taken",
                Notes = logDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Medicinereminderlogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Intake logged successfully" });
        }

        /// <summary>
        /// Get today's medicine schedule
        /// </summary>
        [HttpGet("today")]
        public async Task<ActionResult<IEnumerable<object>>> GetTodaySchedule()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var today = DateOnly.FromDateTime(DateTime.Today);

            var reminders = await _context.Medicinereminders
                .Where(r => r.StudentId == userId 
                    && r.IsActive == true
                    && r.StartDate <= today
                    && (r.EndDate == null || r.EndDate >= today))
                .Select(r => new
                {
                    r.Id,
                    r.MedicineName,
                    r.Dosage,
                    r.Times,
                    r.Notes
                })
                .ToListAsync();

            return Ok(reminders);
        }
    }

    // DTOs for medicine reminders
    public class CreateReminderDto
    {
        public string MedicineName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string? CustomFrequency { get; set; }
        public string Times { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateReminderDto
    {
        public string MedicineName { get; set; } = null!;
        public string Dosage { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public string? CustomFrequency { get; set; }
        public string Times { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Notes { get; set; }
    }

    public class LogIntakeDto
    {
        public DateTime ScheduledTime { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
}
