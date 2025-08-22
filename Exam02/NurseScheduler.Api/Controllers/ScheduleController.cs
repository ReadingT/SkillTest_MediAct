using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NurseScheduler.Api.Data;
using NurseScheduler.Api.DTOs;

namespace NurseScheduler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ScheduleController(AppDbContext context)
        {
            _context = context;
        }

        // GET /my-schedule
        [HttpGet("my-schedule")]
        [Authorize(Roles = "Nurse")]
        public async Task<ActionResult<IEnumerable<MyScheduleDto>>> GetMySchedule()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var schedule = await _context.ShiftAssignments
                .Where(sa => sa.UserId == userId)
                .Include(sa => sa.Shift)
                .Select(sa => new MyScheduleDto(
                    sa.Id,
                    sa.Shift.ShiftDate,
                    sa.Shift.StartTime,
                    sa.Shift.EndTime,
                    // ดึงสถานะการลาล่าสุดของเวรนี้ (ถ้ามี)
                    _context.LeaveRequests
                        .Where(lr => lr.ShiftAssignmentId == sa.Id)
                        .Select(lr => lr.Status)
                        .FirstOrDefault() ?? "N/A" // ถ้าไม่เคยลา จะเป็น N/A
                ))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            return Ok(schedule);
        }
    }
}