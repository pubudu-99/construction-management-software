using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for FR6 — Materials and Stock Management.
/// </summary>
public class InventoryService
{
    private readonly MaterialRepository      _matRepo;
    private readonly StockMovementRepository _movRepo;

    /// <summary>Initialises the service with material and stock-movement repositories.</summary>
    public InventoryService(MaterialRepository matRepo, StockMovementRepository movRepo)
    {
        _matRepo = matRepo;
        _movRepo = movRepo;
    }

    /// <summary>
    /// Validates and inserts a new material record.
    /// </summary>
    /// <returns><see cref="StockResult.Ok"/> or a <see cref="StockResult.Fail"/> with a reason.</returns>
    public StockResult AddMaterial(Material m)
    {
        if (string.IsNullOrWhiteSpace(m.Name))
            return StockResult.Fail("Material name is required.");
        if (string.IsNullOrWhiteSpace(m.Unit))
            return StockResult.Fail("Unit is required.");
        if (m.Stock < 0)
            return StockResult.Fail("Initial stock cannot be negative.");
        if (m.ReorderPoint < 0)
            return StockResult.Fail("Reorder point cannot be negative.");

        _matRepo.Insert(m);
        return StockResult.Ok();
    }

    /// <summary>
    /// Records a stock movement (In or Out) and adjusts the material's stock level.
    /// Returns <see cref="StockResult.OkWithWarning"/> if the new stock level
    /// is at or below the reorder point.
    /// </summary>
    public StockResult RecordMovement(StockMovement sm)
    {
        if (sm.Quantity <= 0)
            return StockResult.Fail("Quantity must be greater than zero.");

        _movRepo.Insert(sm);

        // Check low-stock after the adjustment.
        var mat = _matRepo.GetById(sm.MaterialId);
        if (mat is not null && mat.ReorderPoint > 0 && mat.Stock <= mat.ReorderPoint)
            return StockResult.OkWithWarning(
                $"Low stock: {mat.Name} is now at {mat.Stock:0.##} {mat.Unit} " +
                $"(reorder at {mat.ReorderPoint:0.##}).");

        return StockResult.Ok();
    }

    /// <summary>
    /// Returns low-stock alerts for all materials at or below their reorder point.
    /// </summary>
    public List<LowStockAlert> GetLowStockAlerts() =>
        _matRepo.GetLowStockAlerts()
            .Select(m => new LowStockAlert
            {
                MaterialName = m.Name,
                Unit         = m.Unit,
                CurrentStock = m.Stock,
                ReorderPoint = m.ReorderPoint
            })
            .ToList();

    /// <summary>
    /// Returns a full stock status report for every material.
    /// Status is computed using the <c>LowStockWarningBuffer</c> from App.config
    /// (default 1.5): stock &lt;= reorder × buffer is flagged "Getting Low".
    /// </summary>
    public List<StockStatusLine> GetStockStatusReport()
    {
        decimal buffer = AppConfig.LowStockWarningBuffer;
        var lines = new List<StockStatusLine>();

        foreach (var m in _matRepo.GetAll())
        {
            string status;
            if (m.Stock == 0)
                status = "Out of Stock";
            else if (m.ReorderPoint > 0 && m.Stock <= m.ReorderPoint)
                status = "Low Stock";
            else if (m.ReorderPoint > 0 && m.Stock <= m.ReorderPoint * buffer)
                status = "Getting Low";
            else
                status = "OK";

            lines.Add(new StockStatusLine
            {
                MaterialName = m.Name,
                Unit         = m.Unit,
                CurrentStock = m.Stock,
                ReorderPoint = m.ReorderPoint,
                Status       = status
            });
        }

        return lines;
    }
}
