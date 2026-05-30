using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Equipment"/> entity.
/// </summary>
public class EquipmentRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public EquipmentRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>Returns all equipment items ordered by name.</summary>
    public List<Equipment> GetAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT EquipmentId, Name, Type, Status, CurrentSite,
                   LastMaintenance, NextMaintenance
            FROM   Equipment
            ORDER  BY Name ASC;
        ";
        var list = new List<Equipment>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) list.Add(MapRow(rd));
        return list;
    }

    /// <summary>Returns a single equipment item by ID, or null if not found.</summary>
    public Equipment? GetById(int id)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT EquipmentId, Name, Type, Status, CurrentSite,
                   LastMaintenance, NextMaintenance
            FROM   Equipment WHERE EquipmentId = $id LIMIT 1;
        ";
        cmd.Parameters.AddWithValue("$id", id);
        using var rd = cmd.ExecuteReader();
        return rd.Read() ? MapRow(rd) : null;
    }

    /// <summary>Inserts a new equipment record.</summary>
    public void Insert(Equipment e)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Equipment
                   (Name, Type, Status, CurrentSite, LastMaintenance, NextMaintenance)
            VALUES ($n,   $t,   $s,     $site,       $lm,            $nm);
        ";
        BindParams(cmd, e);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Updates all fields of an existing equipment record.</summary>
    public void Update(Equipment e)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Equipment
            SET    Name=$n, Type=$t, Status=$s, CurrentSite=$site,
                   LastMaintenance=$lm, NextMaintenance=$nm
            WHERE  EquipmentId=$id;
        ";
        cmd.Parameters.AddWithValue("$id", e.EquipmentId);
        BindParams(cmd, e);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns equipment items whose next maintenance date falls within
    /// <paramref name="warningDays"/> days and are not already under maintenance.
    /// </summary>
    public List<Equipment> GetMaintenanceDueAlerts(int warningDays)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT EquipmentId, Name, Type, Status, CurrentSite,
                   LastMaintenance, NextMaintenance
            FROM   Equipment
            WHERE  Status         != 'Under Maintenance'
              AND  NextMaintenance IS NOT NULL
              AND  NextMaintenance <= date('now', '+' || $days || ' days')
            ORDER  BY NextMaintenance ASC;
        ";
        cmd.Parameters.AddWithValue("$days", warningDays);
        var list = new List<Equipment>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) list.Add(MapRow(rd));
        return list;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void BindParams(SqliteCommand cmd, Equipment e)
    {
        cmd.Parameters.AddWithValue("$n",    e.Name);
        cmd.Parameters.AddWithValue("$t",    e.Type);
        cmd.Parameters.AddWithValue("$s",    e.Status);
        cmd.Parameters.AddWithValue("$site", (object?)e.CurrentSite ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$lm",   e.LastMaintenance.HasValue
            ? e.LastMaintenance.Value.ToString("yyyy-MM-dd") : DBNull.Value);
        cmd.Parameters.AddWithValue("$nm",   e.NextMaintenance.HasValue
            ? e.NextMaintenance.Value.ToString("yyyy-MM-dd") : DBNull.Value);
    }

    private static Equipment MapRow(SqliteDataReader rd) => new()
    {
        EquipmentId     = rd.GetInt32(0),
        Name            = rd.GetString(1),
        Type            = rd.GetString(2),
        Status          = rd.GetString(3),
        CurrentSite     = rd.IsDBNull(4) ? null : rd.GetString(4),
        LastMaintenance = rd.IsDBNull(5) ? null : DateTime.Parse(rd.GetString(5)),
        NextMaintenance = rd.IsDBNull(6) ? null : DateTime.Parse(rd.GetString(6))
    };
}
