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
            SELECT WorkerId, Name, HourlyRate, IsActive
            FROM   Workers
            WHERE  IsActive = 1
            ORDER  BY Name;
        ";
        var list = new List<Worker>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(MapRow(rd));
        return list;
    }

    /// <summary>Returns a single worker by ID, or null if not found.</summary>
    public Worker? GetById(int id)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT WorkerId, Name, HourlyRate, IsActive
            FROM   Workers WHERE WorkerId = $id LIMIT 1;
        ";
        cmd.Parameters.AddWithValue("$id", id);
        using var rd = cmd.ExecuteReader();
        return rd.Read() ? MapRow(rd) : null;
    }

    /// <summary>
    /// Inserts a new worker and returns the auto-generated WorkerId.
    /// </summary>
    public int Insert(Worker w)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Workers (Name, HourlyRate, IsActive)
            VALUES ($name, $rate, 1);
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("$name", w.Name);
        cmd.Parameters.AddWithValue("$rate", (double)w.HourlyRate);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static Worker MapRow(SqliteDataReader rd) => new()
    {
        WorkerId   = rd.GetInt32(0),
        Name       = rd.GetString(1),
        HourlyRate = (decimal)rd.GetDouble(2),
        IsActive   = rd.GetInt32(3) == 1
    };
}
