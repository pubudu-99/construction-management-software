namespace ConstructionMS.Models;

/// <summary>
/// Represents a single audit-trail entry recording an important user action
/// (sign-in, payment, task creation, etc.) with its timestamp and actor.
/// </summary>
public class ActivityLogEntry
{
    public int      LogId     { get; set; }
    public DateTime Timestamp { get; set; }
    public string   Username  { get; set; } = "";
    public string   Action    { get; set; } = "";
    public string   Details   { get; set; } = "";
}
