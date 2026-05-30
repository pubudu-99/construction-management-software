namespace ConstructionMS.Services;

/// <summary>
/// Represents the outcome of an inventory operation.
/// Use <see cref="Ok"/> for success, <see cref="Fail"/> for validation errors,
/// and <see cref="OkWithWarning"/> when the operation succeeded but stock
/// has fallen to or below the reorder point.
/// </summary>
public class StockResult
{
    /// <summary>Gets whether the operation succeeded (including warnings).</summary>
    public bool   Success   { get; }

    /// <summary>Gets whether the operation succeeded but triggered a low-stock warning.</summary>
    public bool   IsWarning { get; }

    /// <summary>Gets the error or warning message, or empty string on clean success.</summary>
    public string Message   { get; }

    private StockResult(bool success, bool isWarning, string message)
    {
        Success   = success;
        IsWarning = isWarning;
        Message   = message;
    }

    /// <summary>Returns a clean successful result.</summary>
    public static StockResult Ok()                      => new(true,  false, "");

    /// <summary>Returns a failed result with the supplied reason.</summary>
    public static StockResult Fail(string msg)          => new(false, false, msg);

    /// <summary>Returns a successful result that carries a low-stock warning message.</summary>
    public static StockResult OkWithWarning(string msg) => new(true,  true,  msg);
}
