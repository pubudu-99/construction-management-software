namespace ConstructionMS.Models;

/// <summary>
/// Represents a scheduled task within a project.
/// Named ProjectTask to avoid collision with System.Threading.Tasks.Task.
/// </summary>
public class ProjectTask
{
    public int       TaskId     { get; set; }
    public int       ProjectId  { get; set; }
    public string    Name       { get; set; } = "";
    public DateTime  StartDate  { get; set; }
    public DateTime  EndDate    { get; set; }
    public int?      AssigneeId { get; set; }
    public string    Status     { get; set; } = "";
}
