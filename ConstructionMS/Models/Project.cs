namespace ConstructionMS.Models;

/// <summary>
/// Represents a construction project with a budget and timeline.
/// </summary>
public class Project
{
    public int      ProjectId { get; set; }
    public string   Name      { get; set; } = "";
    public decimal  Budget    { get; set; }
    public decimal  Spent     { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate   { get; set; }

    /// <summary>Lifecycle state: "Active" (the one current project) or "Completed".</summary>
    public string Status { get; set; } = "Active";

    /// <summary>Date the project was marked complete, or <c>null</c> while active.</summary>
    public DateTime? CompletedDate { get; set; }
}
