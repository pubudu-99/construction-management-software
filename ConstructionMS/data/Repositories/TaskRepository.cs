using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="ProjectTask"/> entity.
/// </summary>
public class TaskRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public TaskRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Inserts a new task. Dates are stored as yyyy-MM-dd strings.
    /// AssigneeId is stored as NULL when not set.
    /// </summary>
    public void Insert(ProjectTask t)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Tasks (ProjectId, Name, StartDate, EndDate, AssigneeId, Status)
            VALUES ($pid, $name, $start, $end, $aid, $status);
        ";
        cmd.Parameters.AddWithValue("$pid",    t.ProjectId);
        cmd.Parameters.AddWithValue("$name",   t.Name);
        cmd.Parameters.AddWithValue("$start",  t.StartDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$end",    t.EndDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$aid",    t.AssigneeId.HasValue ? (object)t.AssigneeId.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("$status", t.Status);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns all tasks for a project ordered by end date ascending.
    /// </summary>
    public List<ProjectTask> GetByProject(int projectId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT TaskId, ProjectId, Name, StartDate, EndDate, AssigneeId, Status
            FROM   Tasks
            WHERE  ProjectId = $pid
            ORDER  BY EndDate ASC;
        ";
        cmd.Parameters.AddWithValue("$pid", projectId);
        return ReadList(cmd);
    }

    /// <summary>
    /// Returns all tasks with Status = 'Open', ordered by end date ascending.
    /// Spans every project; callers that need a single project's open tasks
    /// should use <see cref="GetOpenTasksForProject"/> instead.
    /// </summary>
    public List<ProjectTask> GetOpenTasks()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT TaskId, ProjectId, Name, StartDate, EndDate, AssigneeId, Status
            FROM   Tasks
            WHERE  Status = 'Open'
            ORDER  BY EndDate ASC;
        ";
        return ReadList(cmd);
    }

    /// <summary>
    /// Returns open tasks for a single project, ordered by end date ascending.
    /// Used by the dashboard deadline alert loader so alerts and KPIs stay
    /// scoped to the active project.
    /// </summary>
    /// <param name="projectId">The project whose open tasks are wanted.</param>
    public List<ProjectTask> GetOpenTasksForProject(int projectId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT TaskId, ProjectId, Name, StartDate, EndDate, AssigneeId, Status
            FROM   Tasks
            WHERE  Status = 'Open' AND ProjectId = $pid
            ORDER  BY EndDate ASC;
        ";
        cmd.Parameters.AddWithValue("$pid", projectId);
        return ReadList(cmd);
    }

    /// <summary>
    /// Updates the Status column for a single task.
    /// </summary>
    public void UpdateStatus(int taskId, string status)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "UPDATE Tasks SET Status=$s WHERE TaskId=$id;";
        cmd.Parameters.AddWithValue("$s",  status);
        cmd.Parameters.AddWithValue("$id", taskId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Loads a single task by its primary key.
    /// </summary>
    /// <param name="taskId">The task ID to find.</param>
    /// <returns>The matching <see cref="ProjectTask"/>, or <c>null</c> if not found.</returns>
    public ProjectTask? GetById(int taskId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT TaskId, ProjectId, Name, StartDate, EndDate, AssigneeId, Status
            FROM   Tasks
            WHERE  TaskId = $id
            LIMIT  1;
        ";
        cmd.Parameters.AddWithValue("$id", taskId);

        using var rd = cmd.ExecuteReader();
        return rd.Read() ? MapRow(rd) : null;
    }

    /// <summary>
    /// Permanently removes a task row.
    /// </summary>
    /// <param name="taskId">The task to delete.</param>
    public void Delete(int taskId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Tasks WHERE TaskId = $id;";
        cmd.Parameters.AddWithValue("$id", taskId);
        cmd.ExecuteNonQuery();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static List<ProjectTask> ReadList(SqliteCommand cmd)
    {
        var list = new List<ProjectTask>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(MapRow(rd));
        return list;
    }

    private static ProjectTask MapRow(SqliteDataReader rd) => new()
    {
        TaskId     = rd.GetInt32(0),
        ProjectId  = rd.GetInt32(1),
        Name       = rd.GetString(2),
        StartDate  = DateTime.Parse(rd.GetString(3)),
        EndDate    = DateTime.Parse(rd.GetString(4)),
        AssigneeId = rd.IsDBNull(5) ? (int?)null : rd.GetInt32(5),
        Status     = rd.GetString(6)
    };
}
