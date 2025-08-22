using System.ComponentModel.DataAnnotations.Schema;

namespace NurseScheduler.Api.Data.Models;

public class ShiftAssignment
{
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Shift")]
    public int ShiftId { get; set; }
    public Shift Shift { get; set; }
}