namespace ConstructionMS.Models;

/// <summary>
/// Records a single inflow or outflow of stock for a material.
/// </summary>
public class StockMovement
{
    public int      MovementId   { get; set; }
    public int      MaterialId   { get; set; }
    public string   MovementType { get; set; } = "";
    public decimal  Quantity     { get; set; }
    public DateTime Date         { get; set; }
}
