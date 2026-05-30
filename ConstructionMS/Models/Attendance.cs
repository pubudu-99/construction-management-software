namespace ConstructionMS.Models;

/// <summary>
/// Records daily attendance and hours worked for a single worker on a single date.
/// </summary>
public class Attendance
{
    public int      AttendanceId { get; set; }
    public int      WorkerId     { get; set; }
    public DateTime Date         { get; set; }
    public decimal  Hours        { get; set; }
}
