namespace ConstructionMS.Services;

/// <summary>
/// Carries the outcome of a <see cref="ScheduleService.CreateTask"/> call.
/// Use the static factory methods <see cref="Ok"/> and <see cref="Fail"/>
/// to create instances.
/// </summary>
public class TaskValidationResult
{
    /// <summary>Gets whether the operation succeeded.</summary>
    public bool   Success { get; }

    /// <summary>Gets a human-readable message describing the outcome.</summary>
    public string Message { get; }

    private TaskValidationResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>Creates a successful result.</summary>
    public static TaskValidationResult Ok() => new(true, "Task created.");

    /// <summary>Creates a failed result with the supplied reason.</summary>
    public static TaskValidationResult Fail(string msg) => new(false, msg);
}
