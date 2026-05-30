namespace ConstructionMS.Models;

/// <summary>
/// Represents a single payment made against a project (e.g. supplier, labour).
/// </summary>
public class Payment
{
    public int       PaymentId   { get; set; }
    public int       ProjectId   { get; set; }
    public string    PayeeType   { get; set; } = "";
    public string    Reference   { get; set; } = "";
    public decimal   Amount      { get; set; }
    public DateTime  PaymentDate { get; set; }
    public string?   Note        { get; set; }
}
