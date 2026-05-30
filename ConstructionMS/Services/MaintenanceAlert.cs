namespace ConstructionMS.Services;

/// <summary>
/// Represents a maintenance due alert for an equipment item.
/// Displayed on the Dashboard "Maintenance Due" card.
/// </summary>
public class MaintenanceAlert
{
    /// <summary>Gets or sets the equipment name.</summary>
    public string   EquipmentName   { get; set; } = "";

    /// <summary>Gets or sets the scheduled next maintenance date.</summary>
    public DateTime NextMaintenance { get; set; }

    /// <summary>
    /// Gets or sets days until maintenance is due.
    /// Negative values indicate the item is already overdue.
    /// </summary>
    public int      DaysUntilDue    { get; set; }
}
