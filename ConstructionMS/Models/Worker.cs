namespace ConstructionMS.Models;

/// <summary>
/// Represents a laborer or worker employed on construction sites.
/// </summary>
public class Worker
{
    public int     WorkerId   { get; set; }
    public string  Name       { get; set; } = "";
    public decimal HourlyRate { get; set; }
    public bool    IsActive   { get; set; }
}
