namespace ConstructionMS.Services;

/// <summary>
/// Carries the outcome of a <see cref="PaymentService.Record"/> call.
/// Use the static factory methods <see cref="Ok"/> and <see cref="Fail"/>
/// to create instances — the private constructor prevents invalid states.
/// </summary>
public class PaymentResult
{
    /// <summary>Gets whether the operation succeeded.</summary>
    public bool   Success { get; }

    /// <summary>Gets a human-readable message describing the outcome.</summary>
    public string Message { get; }

    private PaymentResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    /// <summary>Creates a successful result.</summary>
    public static PaymentResult Ok() => new(true, "Payment recorded.");

    /// <summary>Creates a failed result with the supplied reason.</summary>
    /// <param name="msg">A message explaining why the operation failed.</param>
    public static PaymentResult Fail(string msg) => new(false, msg);
}
