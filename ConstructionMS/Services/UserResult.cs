namespace ConstructionMS.Services;

/// <summary>
/// Outcome of a user-management operation (create/update).
/// Mirrors the <see cref="PaymentResult"/> pattern.
/// </summary>
public class UserResult
{
    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool   Success { get; }

    /// <summary>Gets the failure reason; empty string on success.</summary>
    public string Message { get; }

    private UserResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>Creates a successful result.</summary>
    public static UserResult Ok() => new(true, "");

    /// <summary>Creates a failure result with the supplied reason.</summary>
    public static UserResult Fail(string message) => new(false, message);
}
