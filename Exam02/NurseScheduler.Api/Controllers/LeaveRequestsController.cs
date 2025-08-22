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
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveRequestsController(AppDbContext context)
        {
            _context = context;
        }

        // POST /leave-requests — (พยาบาล) ขออนุมัติลา
        [HttpPost]
        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] LeaveRequestCreateDto requestDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            var assignment = await _context.ShiftAssignments
                .FirstOrDefaultAsync(sa => sa.Id == requestDto.ShiftAssignmentId);

            if (assignment == null) return NotFound("Shift assignment not found.");

            // **Security Check:** ตรวจสอบให้แน่ใจว่าพยาบาลกำลังขอลาในเวรของตัวเองเท่านั้น
            if (assignment.UserId != userId) return Forbid("You can only request leave for your own shifts.");

            var existingRequest = await _context.LeaveRequests
                .AnyAsync(lr => lr.ShiftAssignmentId == requestDto.ShiftAssignmentId);
            if (existingRequest) return Conflict("A leave request for this shift already exists.");

            var leaveRequest = new Data.Models.LeaveRequest
            {
                ShiftAssignmentId = requestDto.ShiftAssignmentId,
                Reason = requestDto.Reason,
                Status = "Pending"
            };

            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateLeaveRequest), new { id = leaveRequest.Id }, leaveRequest);
        }

        // GET /leave-requests — (หัวหน้า) ดูคำขอลาทั้งหมด (สามารถ filter ตาม status ได้)
        [HttpGet]
        [Authorize(Roles = "HeadNurse")]
        public async Task<ActionResult<IEnumerable<LeaveRequestViewDto>>> GetAllLeaveRequests([FromQuery] string? status)
        {
            var query = _context.LeaveRequests
                .Include(lr => lr.ShiftAssignment.User) // ดึงข้อมูล User (พยาบาลที่ขอ)
                .Include(lr => lr.ShiftAssignment.Shift) // ดึงข้อมูล Shift (เวรที่ขอลา)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && (status.ToLower() == "pending" || status.ToLower() == "approved" || status.ToLower() == "rejected"))
            {
                query = query.Where(lr => lr.Status.ToLower() == status.ToLower());
            }

            var requests = await query
                .OrderByDescending(lr => lr.CreatedAt)
                .Select(lr => new LeaveRequestViewDto(
                    lr.Id,
                    lr.ShiftAssignment.User.Name,
                    lr.ShiftAssignment.Shift.ShiftDate,
                    lr.Reason,
                    lr.Status
                ))
                .ToListAsync();

            return Ok(requests);
        }

        // PATCH /leave-requests/{id}/approve — อนุมัติ/ปฏิเสธคำขอลา
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = "HeadNurse")]
        public async Task<IActionResult> ApproveOrRejectLeaveRequest(int id, [FromBody] LeaveRequestUpdateDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var headNurseIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(headNurseIdStr, out var headNurseId)) return Unauthorized();

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound("Leave request not found.");

            if (leaveRequest.Status != "Pending")
            {
                return Conflict($"This request has already been {leaveRequest.Status.ToLower()}.");
            }

            if (updateDto.Status != "Approved" && updateDto.Status != "Rejected")
            {
                return BadRequest("Invalid status. Must be 'Approved' or 'Rejected'.");
            }

            leaveRequest.Status = updateDto.Status;
            leaveRequest.ApprovedBy = headNurseId;

            _context.LeaveRequests.Update(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Leave request has been {updateDto.Status.ToLower()}." });
        }
    }
}