namespace ConstructionMS.Services;

/// <summary>
/// Holds the display data for one material row in the Stock Status report.
/// Produced by <see cref="InventoryService.GetStockStatusReport"/>.
/// </summary>
public class StockStatusLine
{
    /// <summary>Gets or sets the material name.</summary>
    public string  MaterialName  { get; set; } = "";

    /// <summary>Gets or sets the unit of measure (e.g. "bags", "m³").</summary>
    public string  Unit          { get; set; } = "";

    /// <summary>Gets or sets the current stock level.</summary>
    public decimal CurrentStock  { get; set; }

    /// <summary>Gets or sets the reorder threshold.</summary>
    public decimal ReorderPoint  { get; set; }

    /// <summary>
    /// Gets or sets the computed status string:
    /// "Out of Stock", "Low Stock", "Getting Low", or "OK".
    /// </summary>
    public string  Status        { get; set; } = "OK";
}
