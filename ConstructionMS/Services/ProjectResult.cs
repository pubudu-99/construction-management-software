namespace ConstructionMS.Services;

/// <summary>
/// Outcome of a project-management operation (create/update).
/// Mirrors the <see cref="PaymentResult"/> pattern used across the app.
/// </summary>
public class ProjectResult
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool   Success { get; }

    /// <summary>Gets the failure reason; empty string on success.</summary>
    public string Message { get; }

    private ProjectResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>Creates a successful result.</summary>
    public static ProjectResult Ok() => new(true, "");

    /// <summary>Creates a failure result with the supplied reason.</summary>
    public static ProjectResult Fail(string message) => new(false, message);
}
