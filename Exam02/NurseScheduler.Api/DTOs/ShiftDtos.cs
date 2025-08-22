namespace NurseScheduler.Api.DTOs
{

    public record ShiftCreateDto(DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime);
    public record ShiftAssignmentDto(int UserId, int ShiftId);
    public record MyScheduleDto(int AssignmentId, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, string LeaveStatus);
}
