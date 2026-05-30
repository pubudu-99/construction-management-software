using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Project"/> entity.
/// Provides both standalone-connection and shared-connection overloads so
/// callers can participate in an existing transaction when needed.
/// </summary>
public class ProjectRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>
    /// Initialises the repository with a connection factory.
    /// </summary>
    /// <param name="factory">The factory used to open SQLite connections.</param>
    public ProjectRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Loads a project by its primary key, opening its own connection.
    /// Use this overload for simple read-only lookups.
    /// </summary>
    /// <param name="id">The project ID to find.</param>
    /// <returns>The matching <see cref="Project"/>, or <c>null</c> if not found.</returns>
    public Project? GetById(int id)
    {
        using var conn = _factory.Open();
        return GetById(conn, null, id);
    }

    /// <summary>
    /// Loads a project by its primary key using a supplied connection and
    /// optional transaction. Use this overload when inside a multi-step transaction.
    /// </summary>
    /// <param name="conn">An already-open SQLite connection.</param>
    /// <param name="tx">An active transaction, or <c>null</c>.</param>
    /// <param name="id">The project ID to find.</param>
    /// <returns>The matching <see cref="Project"/>, or <c>null</c> if not found.</returns>
    public Project? GetById(SqliteConnection conn, SqliteTransaction? tx, int id)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = @"
            SELECT ProjectId, Name, Budget, Spent, StartDate, EndDate
            FROM   Projects
            WHERE  ProjectId = $id
            LIMIT  1;
        ";
        cmd.Parameters.AddWithValue("$id", id);

        using var rd = cmd.ExecuteReader();
        if (!rd.Read()) return null;
        return MapRow(rd);
    }

    /// <summary>
    /// Increments the <c>Spent</c> column of a project within a transaction.
    /// Must be called inside an open transaction — the caller commits or rolls back.
    /// </summary>
    /// <param name="conn">An already-open SQLite connection.</param>
    /// <param name="tx">The active transaction.</param>
    /// <param name="projectId">The project to update.</param>
    /// <param name="amount">The amount to add to <c>Spent</c>.</param>
    public void IncrementSpent(SqliteConnection conn, SqliteTransaction tx,
                               int projectId, decimal amount)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = @"
            UPDATE Projects
            SET    Spent = Spent + $a
            WHERE  ProjectId = $id;
        ";
        cmd.Parameters.AddWithValue("$a",   (double)amount);
        cmd.Parameters.AddWithValue("$id",  projectId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns the project with the lowest <c>ProjectId</c>, or <c>null</c> if
    /// no projects exist. Used by forms that need a default project to work with.
    /// </summary>
    /// <returns>The first <see cref="Project"/>, or <c>null</c>.</returns>
    public Project? GetFirst()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ProjectId FROM Projects
            ORDER  BY ProjectId
            LIMIT  1;
        ";
        var result = cmd.ExecuteScalar();
        if (result is null) return null;

        int id = Convert.ToInt32(result);
        return GetById(conn, null, id);
    }

    /// <summary>
    /// Updates the editable fields of a project (name, budget, dates).
    /// <c>Spent</c> is intentionally NOT updated here — it is controlled only
    /// by the payment transaction via <see cref="IncrementSpent"/>.
    /// </summary>
    /// <param name="p">The project carrying the new values; <c>ProjectId</c> selects the row.</param>
    public void Update(Project p)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Projects
            SET    Name      = $n,
                   Budget    = $b,
                   StartDate = $s,
                   EndDate   = $e
            WHERE  ProjectId = $id;
        ";
        cmd.Parameters.AddWithValue("$n",  p.Name);
        cmd.Parameters.AddWithValue("$b",  (double)p.Budget);
        cmd.Parameters.AddWithValue("$s",  p.StartDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$e",  p.EndDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$id", p.ProjectId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Inserts a new project with <c>Spent</c> initialised to zero.
    /// </summary>
    /// <param name="p">The project to insert. <c>ProjectId</c> and <c>Spent</c> are ignored.</param>
    public void Insert(Project p)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Projects (Name, Budget, Spent, StartDate, EndDate)
            VALUES ($n, $b, 0, $s, $e);
        ";
        cmd.Parameters.AddWithValue("$n", p.Name);
        cmd.Parameters.AddWithValue("$b", (double)p.Budget);
        cmd.Parameters.AddWithValue("$s", p.StartDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$e", p.EndDate.ToString("yyyy-MM-dd"));
        cmd.ExecuteNonQuery();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>Maps a data reader row to a <see cref="Project"/> instance.</summary>
    private static Project MapRow(SqliteDataReader rd) => new()
    {
        ProjectId = rd.GetInt32(0),
        Name      = rd.GetString(1),
        Budget    = (decimal)rd.GetDouble(2),
        Spent     = (decimal)rd.GetDouble(3),
        StartDate = DateTime.Parse(rd.GetString(4)),
        EndDate   = DateTime.Parse(rd.GetString(5))
    };
}
