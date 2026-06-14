using ConstructionMS.Models;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="StockMovement"/> entity.
/// Each insert also adjusts the parent material's stock within the same transaction.
/// </summary>
public class StockMovementRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public StockMovementRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Inserts a stock movement record and adjusts the material's stock level
    /// in a single transaction.  "In" movements increase stock; "Out" movements
    /// decrease it.
    /// </summary>
    public void Insert(StockMovement sm)
    {
        decimal delta = sm.MovementType == "Out" ? -sm.Quantity : sm.Quantity;

        using var conn = _factory.Open();
        using var tx   = conn.BeginTransaction();

        using var cmd1 = conn.CreateCommand();
        cmd1.Transaction = tx;
        cmd1.CommandText = @"
            INSERT INTO StockMovements (MaterialId, MovementType, Quantity, Date)
            VALUES ($mid, $mt, $qty, $date);
        ";
        cmd1.Parameters.AddWithValue("$mid",  sm.MaterialId);
        cmd1.Parameters.AddWithValue("$mt",   sm.MovementType);
        cmd1.Parameters.AddWithValue("$qty",  (double)sm.Quantity);
        cmd1.Parameters.AddWithValue("$date", sm.Date.ToString("yyyy-MM-dd"));
        cmd1.ExecuteNonQuery();

        using var cmd2 = conn.CreateCommand();
        cmd2.Transaction = tx;
        cmd2.CommandText = @"
            UPDATE Materials
            SET    Stock = Stock + $delta
            WHERE  MaterialId = $mid;
        ";
        cmd2.Parameters.AddWithValue("$delta", (double)delta);
        cmd2.Parameters.AddWithValue("$mid",   sm.MaterialId);
        cmd2.ExecuteNonQuery();

        tx.Commit();
    }

    /// <summary>
    /// Returns all stock movements for a given material, most recent first.
    /// </summary>
    public List<StockMovement> GetForMaterial(int materialId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT MovementId, MaterialId, MovementType, Quantity, Date
            FROM   StockMovements
            WHERE  MaterialId = $mid
            ORDER  BY Date DESC, MovementId DESC;
        ";
        cmd.Parameters.AddWithValue("$mid", materialId);
        var list = new List<StockMovement>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(new StockMovement
            {
                MovementId   = rd.GetInt32(0),
                MaterialId   = rd.GetInt32(1),
                MovementType = rd.GetString(2),
                Quantity     = (decimal)rd.GetDouble(3),
                Date         = DateTime.Parse(rd.GetString(4))
            });
        return list;
    }
}
