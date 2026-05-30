using System.Configuration;

namespace ConstructionMS.Services;

/// <summary>
/// Reads application settings from App.config (appSettings section).
/// Each property falls back to a hard-coded default if the key is absent
/// or cannot be parsed, so the application always starts successfully.
/// </summary>
public static class AppConfig
{
    /// <summary>
    /// Maximum regular hours per working day before overtime applies.
    /// App.config key: <c>StandardDailyHours</c>. Default: 8.
    /// </summary>
    public static decimal StandardDailyHours =>
        TryGetDecimal("StandardDailyHours", 8.0m);

    /// <summary>
    /// Rate multiplier applied to overtime hours (e.g. 1.5 = time-and-a-half).
    /// App.config key: <c>OvertimeMultiplier</c>. Default: 1.5.
    /// </summary>
    public static decimal OvertimeMultiplier =>
        TryGetDecimal("OvertimeMultiplier", 1.5m);

    /// <summary>
    /// Number of days before scheduled maintenance to start showing the amber alert.
    /// App.config key: <c>MaintenanceWarningDays</c>. Default: 7.
    /// </summary>
    public static int MaintenanceWarningDays =>
        TryGetInt("MaintenanceWarningDays", 7);

    /// <summary>
    /// Multiplier applied to the reorder point to define the "Getting Low" amber zone.
    /// Stock &lt;= ReorderPoint × buffer triggers a "Getting Low" warning.
    /// App.config key: <c>LowStockWarningBuffer</c>. Default: 1.5.
    /// </summary>
    public static decimal LowStockWarningBuffer =>
        TryGetDecimal("LowStockWarningBuffer", 1.5m);

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>Reads a key as decimal; returns <paramref name="fallback"/> on any failure.</summary>
    private static decimal TryGetDecimal(string key, decimal fallback)
    {
        string? raw = ConfigurationManager.AppSettings[key];
        return decimal.TryParse(raw, out decimal val) ? val : fallback;
    }

    /// <summary>Reads a key as int; returns <paramref name="fallback"/> on any failure.</summary>
    private static int TryGetInt(string key, int fallback)
    {
        string? raw = ConfigurationManager.AppSettings[key];
        return int.TryParse(raw, out int val) ? val : fallback;
    }
}
