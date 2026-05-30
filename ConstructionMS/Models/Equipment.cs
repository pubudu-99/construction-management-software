namespace ConstructionMS.Models;

/// <summary>
/// Represents a piece of equipment or vehicle tracked by the system.
/// </summary>
public class Equipment
{
    public int       EquipmentId      { get; set; }
    public string    Name             { get; set; } = "";
    public string    Type             { get; set; } = "";
    public string    Status           { get; set; } = "";
    public string?   CurrentSite      { get; set; }
    public DateTime? LastMaintenance  { get; set; }
    public DateTime? NextMaintenance  { get; set; }
}
