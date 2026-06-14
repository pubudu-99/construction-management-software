using ConstructionMS.Models;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="ActivityLogEntry"/>
/// audit trail. Records are append-only; the most recent are read back for display.
/// </summary>
public class ActivityLogRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    /// <param name="factory">The factory used to open SQLite connections.</param>
    public ActivityLogRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Inserts a new audit-trail entry. The timestamp is stamped now,
    /// formatted as yyyy-MM-dd HH:mm:ss.
    /// </summary>
    /// <param name="username">The user who performed the action ("system" if none).</param>
    /// <param name="action">A short action label (e.g. "Sign In").</param>
    /// <param name="details">Optional free-text details.</param>
    public void Insert(string username, string action, string details)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO ActivityLog (Timestamp, Username, Action, Details)
            VALUES ($ts, $user, $action, $details);
        ";
        cmd.Parameters.AddWithValue("$ts",      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("$user",    username);
        cmd.Parameters.AddWithValue("$action",  action);
        cmd.Parameters.AddWithValue("$details", details ?? "");
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns the most recent audit-trail entries, newest first.
    /// </summary>
    /// <param name="maxRows">The maximum number of rows to return (default 200).</param>
    /// <returns>A list of <see cref="ActivityLogEntry"/> objects.</returns>
    public List<ActivityLogEntry> GetRecent(int maxRows = 200)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT LogId, Timestamp, Username, Action, Details
            FROM   ActivityLog
            ORDER  BY Timestamp DESC, LogId DESC
            LIMIT  $max;
        ";
        cmd.Parameters.AddWithValue("$max", maxRows);

        var list = new List<ActivityLogEntry>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(new ActivityLogEntry
            {
                LogId     = rd.GetInt32(0),
                Timestamp = DateTime.Parse(rd.GetString(1)),
                Username  = rd.GetString(2),
                Action    = rd.GetString(3),
                Details   = rd.IsDBNull(4) ? "" : rd.GetString(4)
            });
        return list;
    }
}
