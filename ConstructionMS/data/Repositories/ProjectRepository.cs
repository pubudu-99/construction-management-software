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
            SELECT ProjectId, Name, Budget, Spent, StartDate, EndDate, Status, CompletedDate
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
    /// Returns the single project whose <c>Status</c> is <c>'Active'</c>, or
    /// <c>null</c> if no active project exists (e.g. all projects are completed,
    /// or the database is empty). At most one project is ever active.
    /// </summary>
    /// <returns>The active <see cref="Project"/>, or <c>null</c>.</returns>
    public Project? GetActive()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ProjectId FROM Projects
            WHERE  Status = 'Active'
            ORDER  BY ProjectId
            LIMIT  1;
        ";
        var result = cmd.ExecuteScalar();
        if (result is null) return null;

        int id = Convert.ToInt32(result);
        return GetById(conn, null, id);
    }

    /// <summary>
    /// Backwards-compatible alias for <see cref="GetActive"/>. Existing callers
    /// that asked for "the current project" now resolve the active one.
    /// </summary>
    /// <returns>The active <see cref="Project"/>, or <c>null</c>.</returns>
    public Project? GetFirst() => GetActive();

    /// <summary>
    /// Returns every project, active first then the rest, newest ID first within
    /// each group. Used by the Reports project selector.
    /// </summary>
    /// <returns>All projects, ordered for display.</returns>
    public List<Project> GetAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ProjectId, Name, Budget, Spent, StartDate, EndDate, Status, CompletedDate
            FROM   Projects
            ORDER  BY (CASE Status WHEN 'Active' THEN 0 ELSE 1 END), ProjectId DESC;
        ";

        var list = new List<Project>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(MapRow(rd));
        return list;
    }

    /// <summary>
    /// Returns all completed projects, most recently completed first.
    /// </summary>
    /// <returns>The completed projects.</returns>
    public List<Project> GetCompleted()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ProjectId, Name, Budget, Spent, StartDate, EndDate, Status, CompletedDate
            FROM   Projects
            WHERE  Status = 'Completed'
            ORDER  BY CompletedDate DESC;
        ";

        var list = new List<Project>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(MapRow(rd));
        return list;
    }

    /// <summary>
    /// Marks a project as completed, stamping today's date. After this the
    /// project is read-only and a new active project may be created.
    /// </summary>
    /// <param name="projectId">The project to complete.</param>
    public void MarkComplete(int projectId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Projects
            SET    Status        = 'Completed',
                   CompletedDate = date('now')
            WHERE  ProjectId = $id;
        ";
        cmd.Parameters.AddWithValue("$id", projectId);
        cmd.ExecuteNonQuery();
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
    /// Inserts a new active project with <c>Spent</c> initialised to zero and
    /// returns its generated <c>ProjectId</c>. Only one project may be active at
    /// a time: if an active project already exists this throws and inserts nothing.
    /// </summary>
    /// <param name="p">The project to insert. <c>ProjectId</c>, <c>Spent</c> and <c>Status</c> are ignored.</param>
    /// <returns>The new project's <c>ProjectId</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when another active project already exists.</exception>
    public int Insert(Project p)
    {
        if (GetActive() is not null)
            throw new InvalidOperationException(
                "Cannot create new project while another is active. " +
                "Mark current project complete first.");

        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Projects (Name, Budget, Spent, StartDate, EndDate, Status)
            VALUES ($n, $b, 0, $s, $e, 'Active');
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("$n", p.Name);
        cmd.Parameters.AddWithValue("$b", (double)p.Budget);
        cmd.Parameters.AddWithValue("$s", p.StartDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$e", p.EndDate.ToString("yyyy-MM-dd"));
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>Maps a data reader row to a <see cref="Project"/> instance.</summary>
    private static Project MapRow(SqliteDataReader rd) => new()
    {
        ProjectId     = rd.GetInt32(0),
        Name          = rd.GetString(1),
        Budget        = (decimal)rd.GetDouble(2),
        Spent         = (decimal)rd.GetDouble(3),
        StartDate     = DateTime.Parse(rd.GetString(4)),
        EndDate       = DateTime.Parse(rd.GetString(5)),
        Status        = rd.GetString(6),
        CompletedDate = rd.IsDBNull(7) ? (DateTime?)null : DateTime.Parse(rd.GetString(7))
    };
}
