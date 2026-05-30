namespace ConstructionMS.Services;

/// <summary>
/// Represents a deadline alert for a task that is due within 3 days
/// or already overdue. Displayed on the Dashboard.
/// </summary>
public class TaskAlert
{
    /// <summary>Gets or sets the name of the task.</summary>
    public string TaskName  { get; set; } = "";

    /// <summary>
    /// Gets or sets the number of days until the deadline.
    /// Negative values mean the task is overdue.
    /// </summary>
    public int    DueIn     { get; set; }

    /// <summary>Gets or sets whether the task deadline has already passed.</summary>
    public bool   IsOverdue { get; set; }

    /// <summary>
    /// Gets or sets a human-readable countdown string such as "2d 3h", "45m", or "-1h 20m".
    /// </summary>
    public string RemainingText { get; set; } = "";
}
