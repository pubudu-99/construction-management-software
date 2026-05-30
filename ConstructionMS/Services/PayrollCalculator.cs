using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// FR4 — Pure payroll calculation logic.
/// No database calls — receives data from the caller and returns results.
/// Standard hours and overtime multiplier are read from App.config via
/// <see cref="AppConfig"/>; the constants below serve as fallback defaults.
/// </summary>
public class PayrollCalculator
{
    /// <summary>Hard-coded fallback: maximum regular hours per working day.</summary>
    public const decimal StandardDailyHours = 8.0m;

    /// <summary>Hard-coded fallback: overtime rate multiplier.</summary>
    public const decimal OvertimeMultiplier = 1.5m;

    /// <summary>
    /// Calculates regular hours, overtime hours, and total gross pay
    /// for a single worker over the supplied attendance records.
    /// Standard hours and overtime multiplier are read from App.config
    /// (keys <c>StandardDailyHours</c> and <c>OvertimeMultiplier</c>)
    /// and fall back to the class constants when the keys are absent.
    /// </summary>
    /// <param name="worker">The worker whose rate will be used.</param>
    /// <param name="records">Attendance records for the period.</param>
    /// <returns>A <see cref="PayrollLine"/> with all calculated figures.</returns>
    public PayrollLine CalculateForWorker(Worker worker, List<Attendance> records)
    {
        decimal stdHours  = AppConfig.StandardDailyHours;
        decimal otMult    = AppConfig.OvertimeMultiplier;

        decimal regular  = 0m;
        decimal overtime = 0m;

        foreach (var record in records)
        {
            if (record.Hours <= stdHours)
            {
                regular += record.Hours;
            }
            else
            {
                regular  += stdHours;
                overtime += record.Hours - stdHours;
            }
        }

        decimal pay = (regular  * worker.HourlyRate)
                    + (overtime * worker.HourlyRate * otMult);

        return new PayrollLine(worker.WorkerId, worker.Name, regular, overtime, pay);
    }

    /// <summary>
    /// Generates a full payroll report for all supplied workers
    /// using attendance data from the given date range.
    /// </summary>
    /// <param name="workers">Active workers to include in the report.</param>
    /// <param name="attendanceRepo">Repository used to fetch attendance records.</param>
    /// <param name="from">Start of the pay period (inclusive).</param>
    /// <param name="to">End of the pay period (inclusive).</param>
    /// <returns>A list of <see cref="PayrollLine"/> objects, one per worker.</returns>
    public List<PayrollLine> GenerateReport(List<Worker> workers,
                                             AttendanceRepository attendanceRepo,
                                             DateTime from, DateTime to)
    {
        var report = new List<PayrollLine>();
        foreach (var worker in workers)
        {
            var records = attendanceRepo.GetForWorker(worker.WorkerId, from, to);
            report.Add(CalculateForWorker(worker, records));
        }
        return report;
    }
}
