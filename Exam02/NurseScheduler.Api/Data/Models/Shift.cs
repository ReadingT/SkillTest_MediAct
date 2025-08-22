namespace NurseScheduler.Api.Data.Models;

public class Shift
{
    public int Id { get; set; }
    public DateOnly ShiftDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}