namespace ConstructionMS.Services;

/// <summary>
/// Holds the calculated payroll figures for one worker over a date range.
/// Produced by <see cref="PayrollCalculator.CalculateForWorker"/>.
/// </summary>
public class PayrollLine
{
    /// <summary>Gets the worker's database ID.</summary>
    public int     WorkerId      { get; }

    /// <summary>Gets the worker's full name.</summary>
    public string  WorkerName    { get; }

    /// <summary>Gets the total regular (non-overtime) hours worked.</summary>
    public decimal RegularHours  { get; }

    /// <summary>Gets the total overtime hours worked (above 8 hrs/day).</summary>
    public decimal OvertimeHours { get; }

    /// <summary>Gets the total gross pay for the period.</summary>
    public decimal TotalPay      { get; }

    /// <summary>Initialises a payroll line with all calculated values.</summary>
    public PayrollLine(int workerId, string workerName,
                       decimal regularHours, decimal overtimeHours, decimal totalPay)
    {
        WorkerId      = workerId;
        WorkerName    = workerName;
        RegularHours  = regularHours;
        OvertimeHours = overtimeHours;
        TotalPay      = totalPay;
    }
}
