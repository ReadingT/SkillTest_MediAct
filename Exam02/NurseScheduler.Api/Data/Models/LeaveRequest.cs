using System.ComponentModel.DataAnnotations.Schema;

namespace NurseScheduler.Api.Data.Models;

public class LeaveRequest
{
    public int Id { get; set; }

    [ForeignKey("ShiftAssignment")]
    public int ShiftAssignmentId { get; set; }
    public ShiftAssignment ShiftAssignment { get; set; }

    public string Reason { get; set; }
    public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

    [ForeignKey("Approver")]
    public int? ApprovedBy { get; set; }
    public User? Approver { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}