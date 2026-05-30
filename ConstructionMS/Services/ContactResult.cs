namespace ConstructionMS.Services;

/// <summary>
/// Represents the outcome of a contact operation.
/// Use <see cref="Ok"/> for success and <see cref="Fail"/> for validation errors.
/// </summary>
public class ContactResult
{
    /// <summary>Gets whether the operation succeeded.</summary>
    public bool   Success { get; }

    /// <summary>Gets the error message, or empty string on success.</summary>
    public string Message { get; }

    private ContactResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>Returns a successful result.</summary>
    public static ContactResult Ok()             => new(true,  "");

    /// <summary>Returns a failed result with the supplied reason.</summary>
    public static ContactResult Fail(string msg) => new(false, msg);
}
