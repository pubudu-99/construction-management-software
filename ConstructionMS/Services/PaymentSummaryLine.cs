namespace ConstructionMS.Services;

/// <summary>
/// Holds one row of the payment-summary-by-type report.
/// Produced by <see cref="Data.Repositories.ReportRepository.GetPaymentSummaryByType"/>.
/// </summary>
public class PaymentSummaryLine
{
    /// <summary>Gets or sets the payee category (e.g. "Supplier", "Labour").</summary>
    public string  PayeeType { get; set; } = "";

    /// <summary>Gets or sets the number of payment records in this category.</summary>
    public int     Count     { get; set; }

    /// <summary>Gets or sets the summed amount for this category.</summary>
    public decimal Total     { get; set; }
}
