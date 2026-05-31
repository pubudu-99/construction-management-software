using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Worker"/> entity.
/// </summary>
public class WorkerRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public WorkerRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>Returns all active workers ordered by name.</summary>
    public List<Worker> GetActive()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT WorkerId, Name, HourlyRate, IsActive, NIC, Phone
            FROM   Workers
            WHERE  IsActive = 1
            ORDER  BY Name;
        ";
        return ReadList(cmd);
    }

    /// <summary>Returns every worker (active and inactive) ordered by name.</summary>
    public List<Worker> GetAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT WorkerId, Name, HourlyRate, IsActive, NIC, Phone
            FROM   Workers
            ORDER  BY Name;
        ";
        return ReadList(cmd);
    }

    /// <summary>Returns a single worker by ID, or null if not found.</summary>
    public Worker? GetById(int id)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT WorkerId, Name, HourlyRate, IsActive, NIC, Phone
            FROM   Workers WHERE WorkerId = $id LIMIT 1;
        ";
        cmd.Parameters.AddWithValue("$id", id);
        using var rd = cmd.ExecuteReader();
        return rd.Read() ? MapRow(rd) : null;
    }

    /// <summary>
    /// Inserts a new worker and returns the auto-generated WorkerId.
    /// Blank NIC/Phone values are stored as NULL so the unique NIC index
    /// permits multiple workers without a recorded NIC.
    /// </summary>
    public int Insert(Worker w)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Workers (Name, HourlyRate, IsActive, NIC, Phone)
            VALUES ($name, $rate, $active, $nic, $phone);
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("$name",   w.Name);
        cmd.Parameters.AddWithValue("$rate",   (double)w.HourlyRate);
        cmd.Parameters.AddWithValue("$active", w.IsActive ? 1 : 0);
        cmd.Parameters.AddWithValue("$nic",    NullIfBlank(w.NIC));
        cmd.Parameters.AddWithValue("$phone",  NullIfBlank(w.Phone));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>Updates an existing worker's editable fields.</summary>
    public void Update(Worker w)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Workers
            SET    Name=$name, HourlyRate=$rate, IsActive=$active,
                   NIC=$nic, Phone=$phone
            WHERE  WorkerId=$id;
        ";
        cmd.Parameters.AddWithValue("$name",   w.Name);
        cmd.Parameters.AddWithValue("$rate",   (double)w.HourlyRate);
        cmd.Parameters.AddWithValue("$active", w.IsActive ? 1 : 0);
        cmd.Parameters.AddWithValue("$nic",    NullIfBlank(w.NIC));
        cmd.Parameters.AddWithValue("$phone",  NullIfBlank(w.Phone));
        cmd.Parameters.AddWithValue("$id",     w.WorkerId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Sets a worker's active flag.</summary>
    public void SetActive(int workerId, bool active)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "UPDATE Workers SET IsActive=$active WHERE WorkerId=$id;";
        cmd.Parameters.AddWithValue("$active", active ? 1 : 0);
        cmd.Parameters.AddWithValue("$id",     workerId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns true if another worker already holds the given (non-empty) NIC.
    /// </summary>
    /// <param name="nic">The NIC to check.</param>
    /// <param name="excludeWorkerId">A worker ID to ignore (the one being edited).</param>
    public bool NicExists(string nic, int excludeWorkerId = 0)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT COUNT(*) FROM Workers
            WHERE  NIC = $nic AND WorkerId != $exclude;
        ";
        cmd.Parameters.AddWithValue("$nic",     nic);
        cmd.Parameters.AddWithValue("$exclude", excludeWorkerId);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static List<Worker> ReadList(SqliteCommand cmd)
    {
        var list = new List<Worker>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(MapRow(rd));
        return list;
    }

    private static Worker MapRow(SqliteDataReader rd) => new()
    {
        WorkerId   = rd.GetInt32(0),
        Name       = rd.GetString(1),
        HourlyRate = (decimal)rd.GetDouble(2),
        IsActive   = rd.GetInt32(3) == 1,
        NIC        = rd.IsDBNull(4) ? null : rd.GetString(4),
        Phone      = rd.IsDBNull(5) ? null : rd.GetString(5)
    };

    private static object NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
}
