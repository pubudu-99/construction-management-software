using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Material"/> entity.
/// </summary>
public class MaterialRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public MaterialRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>Returns all materials ordered by name.</summary>
    public List<Material> GetAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT MaterialId, Name, Unit, Stock, ReorderPoint, SupplierId
            FROM   Materials
            ORDER  BY Name ASC;
        ";
        var list = new List<Material>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) list.Add(MapRow(rd));
        return list;
    }

    /// <summary>Returns a single material by ID, or null if not found.</summary>
    public Material? GetById(int id)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT MaterialId, Name, Unit, Stock, ReorderPoint, SupplierId
            FROM   Materials WHERE MaterialId = $id LIMIT 1;
        ";
        cmd.Parameters.AddWithValue("$id", id);
        using var rd = cmd.ExecuteReader();
        return rd.Read() ? MapRow(rd) : null;
    }

    /// <summary>Inserts a new material record.</summary>
    public void Insert(Material m)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Materials (Name, Unit, Stock, ReorderPoint, SupplierId)
            VALUES ($n, $u, $s, $rp, $sup);
        ";
        BindParams(cmd, m);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Updates all fields of an existing material record.</summary>
    public void Update(Material m)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Materials
            SET    Name=$n, Unit=$u, Stock=$s, ReorderPoint=$rp, SupplierId=$sup
            WHERE  MaterialId=$id;
        ";
        cmd.Parameters.AddWithValue("$id", m.MaterialId);
        BindParams(cmd, m);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns materials whose stock is at or below the reorder point,
    /// excluding items where no reorder point has been configured (ReorderPoint = 0).
    /// </summary>
    public List<Material> GetLowStockAlerts()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT MaterialId, Name, Unit, Stock, ReorderPoint, SupplierId
            FROM   Materials
            WHERE  ReorderPoint > 0
              AND  Stock <= ReorderPoint
            ORDER  BY Name ASC;
        ";
        var list = new List<Material>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) list.Add(MapRow(rd));
        return list;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void BindParams(SqliteCommand cmd, Material m)
    {
        cmd.Parameters.AddWithValue("$n",   m.Name);
        cmd.Parameters.AddWithValue("$u",   m.Unit);
        cmd.Parameters.AddWithValue("$s",   (double)m.Stock);
        cmd.Parameters.AddWithValue("$rp",  (double)m.ReorderPoint);
        cmd.Parameters.AddWithValue("$sup", (object?)m.SupplierId ?? DBNull.Value);
    }

    private static Material MapRow(SqliteDataReader rd) => new()
    {
        MaterialId   = rd.GetInt32(0),
        Name         = rd.GetString(1),
        Unit         = rd.GetString(2),
        Stock        = (decimal)rd.GetDouble(3),
        ReorderPoint = (decimal)rd.GetDouble(4),
        SupplierId   = rd.IsDBNull(5) ? null : rd.GetInt32(5)
    };
}
