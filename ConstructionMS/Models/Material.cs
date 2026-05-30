namespace ConstructionMS.Models;

/// <summary>
/// Represents a stock item (e.g. cement, steel) managed in the materials register.
/// </summary>
public class Material
{
    public int      MaterialId   { get; set; }
    public string   Name         { get; set; } = "";
    public string   Unit         { get; set; } = "";
    public decimal  Stock        { get; set; }
    public decimal  ReorderPoint { get; set; }
    public int?     SupplierId   { get; set; }
}
