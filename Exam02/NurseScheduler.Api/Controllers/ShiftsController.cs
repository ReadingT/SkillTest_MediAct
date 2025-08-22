using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NurseScheduler.Api.Data;
using NurseScheduler.Api.Data.Models;
using NurseScheduler.Api.DTOs;

namespace NurseScheduler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "HeadNurse")] // ทุก Action ใน Controller นี้ต้องเป็น HeadNurse
    public class ShiftsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShiftsController(AppDbContext context)
        {
            _context = context;
        }

        // POST /shifts
        [HttpPost]
        public async Task<IActionResult> CreateShift([FromBody] ShiftCreateDto shiftDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newShift = new Shift
            {
                ShiftDate = shiftDto.ShiftDate,
                StartTime = shiftDto.StartTime,
                EndTime = shiftDto.EndTime
            };

            await _context.Shifts.AddAsync(newShift);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateShift), new { id = newShift.Id }, newShift);
        }

        // POST /shift-assignments
        [HttpPost("assignments")]
        public async Task<IActionResult> AssignShift([FromBody] ShiftAssignmentDto assignmentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _context.Users.AnyAsync(u => u.Id == assignmentDto.UserId && u.Role == "Nurse");
            if (!userExists) return NotFound($"Nurse with ID {assignmentDto.UserId} not found.");

            var shiftExists = await _context.Shifts.AnyAsync(s => s.Id == assignmentDto.ShiftId);
            if (!shiftExists) return NotFound($"Shift with ID {assignmentDto.ShiftId} not found.");

            var assignmentExists = await _context.ShiftAssignments
                .AnyAsync(sa => sa.UserId == assignmentDto.UserId && sa.ShiftId == assignmentDto.ShiftId);
            if (assignmentExists) return Conflict("This nurse is already assigned to this shift.");

            var newAssignment = new ShiftAssignment
            {
                UserId = assignmentDto.UserId,
                ShiftId = assignmentDto.ShiftId
            };

            await _context.ShiftAssignments.AddAsync(newAssignment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Shift assigned successfully." });
        }
    }
}