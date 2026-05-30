namespace ConstructionMS.Services;

/// <summary>
/// Represents a low-stock alert for a material item.
/// Displayed on the Dashboard "Low Stock" card.
/// </summary>
public class LowStockAlert
{
    /// <summary>Gets or sets the material name.</summary>
    public string  MaterialName { get; set; } = "";

    /// <summary>Gets or sets the unit of measure (e.g. "kg", "bags").</summary>
    public string  Unit         { get; set; } = "";

    /// <summary>Gets or sets the current stock level.</summary>
    public decimal CurrentStock { get; set; }

    /// <summary>Gets or sets the reorder point threshold.</summary>
    public decimal ReorderPoint { get; set; }
}
