using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for FR3 — Time Scheduling.
/// Validates task data and generates deadline alerts for the dashboard.
/// </summary>
public class ScheduleService
{
    private readonly DbConnectionFactory _factory;
    private readonly TaskRepository      _tasks;

    /// <summary>Initialises the service with a connection factory and task repository.</summary>
    /// <param name="factory">Connection factory used for audit logging.</param>
    /// <param name="tasks">The repository used to read and write tasks.</param>
    public ScheduleService(DbConnectionFactory factory, TaskRepository tasks)
    {
        _factory = factory;
        _tasks   = tasks;
    }

    /// <summary>
    /// Validates and inserts a new task.
    /// Rules: name not empty, start date before end date, project set.
    /// </summary>
    /// <param name="t">The task to create.</param>
    /// <returns>
    /// <see cref="TaskValidationResult.Ok"/> on success, or
    /// <see cref="TaskValidationResult.Fail"/> with a reason.
    /// </returns>
    public TaskValidationResult CreateTask(ProjectTask t)
    {
        if (string.IsNullOrWhiteSpace(t.Name))
            return TaskValidationResult.Fail("Task name is required.");

        if (t.StartDate.Date > t.EndDate.Date)
            return TaskValidationResult.Fail("Start date must be before end date.");

        if (t.ProjectId == 0)
            return TaskValidationResult.Fail("Project not set.");

        _tasks.Insert(t);
        ActivityLogger.Log(_factory, "Task Created", t.Name);
        return TaskValidationResult.Ok();
    }

    /// <summary>
    /// Returns alerts for all open tasks due within 3 days (or overdue).
    /// Sort order: overdue tasks first (most overdue at top),
    /// then upcoming tasks (least time remaining at top).
    /// </summary>
    /// <returns>A list of <see cref="TaskAlert"/> objects.</returns>
    public List<TaskAlert> GetDeadlineAlerts()
    {
        var now    = DateTime.Now;
        var alerts = new List<TaskAlert>();

        foreach (var task in _tasks.GetOpenTasks())
        {
            int days = (task.EndDate.Date - DateTime.Today).Days;
            if (days <= 3)
            {
                alerts.Add(new TaskAlert
                {
                    TaskName      = task.Name,
                    DueIn         = days,
                    IsOverdue     = days < 0,
                    RemainingText = FormatRemaining(task.EndDate.Date, now)
                });
            }
        }

        // OrderBy(DueIn) ascending puts most-negative (most overdue) first,
        // then 0, 1, 2, 3 (least time remaining first among upcoming).
        return alerts.OrderBy(a => a.DueIn).ToList();
    }

    /// <summary>
    /// Returns a concise countdown to the end of <paramref name="endDate"/>.
    /// Overdue values are prefixed with "-".
    /// Examples: "3d 4h", "5h 20m", "45m", "-2d 1h".
    /// </summary>
    private static string FormatRemaining(DateTime endDate, DateTime now)
    {
        var deadline  = endDate.Date.AddDays(1);
        var remaining = deadline - now;

        if (remaining.TotalSeconds <= 0)
        {
            var over = now - deadline;
            if (over.TotalDays >= 1)
                return $"-{(int)over.TotalDays}d {over.Hours}h";
            if (over.TotalHours >= 1)
                return $"-{(int)over.TotalHours}h {over.Minutes}m";
            return $"-{over.Minutes}m";
        }

        if (remaining.TotalDays >= 1)
            return $"{(int)remaining.TotalDays}d {remaining.Hours}h";
        if (remaining.TotalHours >= 1)
            return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
        return $"{remaining.Minutes}m";
    }
}
