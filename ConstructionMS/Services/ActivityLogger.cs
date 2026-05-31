using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;

namespace ConstructionMS.Services;

/// <summary>
/// Thin helper that records an audit-trail entry for an important action.
/// Failures are swallowed — logging must never crash the application.
/// </summary>
public static class ActivityLogger
{
    /// <summary>
    /// Records an action against the current session user (or "system" when
    /// no one is signed in). Any error during logging is silently ignored.
    /// </summary>
    /// <param name="factory">The database connection factory.</param>
    /// <param name="action">A short action label (e.g. "Payment Recorded").</param>
    /// <param name="details">Optional free-text details.</param>
    public static void Log(DbConnectionFactory factory, string action, string details = "")
    {
        try
        {
            string user = Session.Current?.Username ?? "system";
            new ActivityLogRepository(factory).Insert(user, action, details);
        }
        catch
        {
            // Logging is best-effort: never let an audit failure break the app.
        }
    }
}
