namespace NurseScheduler.Api.DTOs
{

    public record LeaveRequestCreateDto(int ShiftAssignmentId, string Reason);
    public record LeaveRequestUpdateDto(string Status); // "Approved" or "Rejected"
    public record LeaveRequestViewDto(int Id, string NurseName, DateOnly ShiftDate, string Reason, string Status);
}