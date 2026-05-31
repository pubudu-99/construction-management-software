using System.Drawing.Printing;

namespace ConstructionMS.Services;

/// <summary>
/// Builds a single-page printable payroll receipt (payslip) for one worker
/// using <see cref="PrintDocument"/>. Requires no extra NuGet package — the
/// user can save the result as PDF via the built-in "Microsoft Print to PDF".
/// </summary>
public class PayrollReceiptPrinter
{
    private readonly string   _projectName;
    private readonly string   _workerName;
    private readonly DateTime _from;
    private readonly DateTime _to;
    private readonly decimal  _regularHours;
    private readonly decimal  _overtimeHours;
    private readonly decimal  _hourlyRate;
    private readonly decimal  _overtimeMultiplier;
    private readonly decimal  _totalPay;

    /// <summary>Creates a printer holding all figures for one worker's payslip.</summary>
    /// <param name="projectName">
    /// Name of the construction project this payslip is issued for —
    /// printed as the document header so the receipt is identified with
    /// the project, not the application name.
    /// </param>
    /// <param name="workerName">Worker's full name.</param>
    /// <param name="from">Start of the pay period (inclusive).</param>
    /// <param name="to">End of the pay period (inclusive).</param>
    /// <param name="regularHours">Total regular (non-overtime) hours.</param>
    /// <param name="overtimeHours">Total overtime hours.</param>
    /// <param name="hourlyRate">Worker's hourly rate (LKR).</param>
    /// <param name="overtimeMultiplier">Overtime rate multiplier (e.g. 1.5).</param>
    /// <param name="totalPay">Gross pay for the period (LKR).</param>
    public PayrollReceiptPrinter(string projectName,
                                 string workerName, DateTime from, DateTime to,
                                 decimal regularHours, decimal overtimeHours,
                                 decimal hourlyRate, decimal overtimeMultiplier,
                                 decimal totalPay)
    {
        _projectName        = projectName;
        _workerName         = workerName;
        _from               = from;
        _to                 = to;
        _regularHours       = regularHours;
        _overtimeHours      = overtimeHours;
        _hourlyRate         = hourlyRate;
        _overtimeMultiplier = overtimeMultiplier;
        _totalPay           = totalPay;
    }

    /// <summary>
    /// Builds a <see cref="PrintDocument"/> whose single page renders the payslip.
    /// The caller is responsible for disposing the returned document.
    /// </summary>
    /// <returns>A ready-to-preview/print document.</returns>
    public PrintDocument Build()
    {
        // The document name shows up in the print queue and as the default
        // file name when saving via "Microsoft Print to PDF". Include the
        // project so multiple project receipts are easy to tell apart.
        var doc = new PrintDocument
        {
            DocumentName = $"{_projectName} - Payroll Receipt - {_workerName}"
        };
        doc.PrintPage += DrawPage;
        return doc;
    }

    /// <summary>Renders the full payslip onto a single page.</summary>
    private void DrawPage(object? sender, PrintPageEventArgs e)
    {
        var g = e.Graphics!;
        float left  = e.MarginBounds.Left;
        float right = e.MarginBounds.Right;
        float width = e.MarginBounds.Width;
        float y     = e.MarginBounds.Top;

        using var headerFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        using var subFont    = new Font("Segoe UI", 13F, FontStyle.Bold);
        using var bodyFont   = new Font("Segoe UI", 11F, FontStyle.Regular);
        using var totalFont  = new Font("Segoe UI", 12F, FontStyle.Bold);
        using var footFont   = new Font("Segoe UI", 8F, FontStyle.Italic);
        using var center     = new StringFormat { Alignment = StringAlignment.Center };

        // ── Document header (project name on top, then "Payroll Receipt") ──
        // The project this payslip is issued for, rather than the application's
        // own name — that's what construction staff actually need to see on
        // a printed/saved receipt.
        string header = string.IsNullOrWhiteSpace(_projectName)
            ? "PAYROLL RECEIPT"
            : _projectName.ToUpperInvariant();

        g.DrawString(header, headerFont, Brushes.Black,
                     new RectangleF(left, y, width, headerFont.GetHeight(g) + 4), center);
        y += headerFont.GetHeight(g) + 6;
        g.DrawString("Payroll Receipt", subFont, Brushes.Black,
                     new RectangleF(left, y, width, subFont.GetHeight(g) + 4), center);
        y += subFont.GetHeight(g) + 18;

        g.DrawLine(Pens.Black, left, y, right, y);
        y += 14;

        decimal otRate      = _hourlyRate * _overtimeMultiplier;
        decimal regularPay  = _regularHours  * _hourlyRate;
        decimal overtimePay = _overtimeHours * otRate;

        // ── Details (left-aligned label / value) ──
        y = DrawRow(g, bodyFont, left, y, "Worker Name:",   _workerName);
        y = DrawRow(g, bodyFont, left, y, "Pay Period:",    $"{_from:yyyy-MM-dd}  to  {_to:yyyy-MM-dd}");
        y += 10;
        y = DrawRow(g, bodyFont, left, y, "Regular Hours:",  $"{_regularHours:0.##}");
        y = DrawRow(g, bodyFont, left, y, "Overtime Hours:", $"{_overtimeHours:0.##}");
        y = DrawRow(g, bodyFont, left, y, "Hourly Rate:",    $"LKR {_hourlyRate:N2}");
        y = DrawRow(g, bodyFont, left, y, "Overtime Rate:",  $"LKR {otRate:N2}");
        y += 10;
        y = DrawRow(g, bodyFont, left, y, "Regular Pay:",    $"LKR {regularPay:N2}");
        y = DrawRow(g, bodyFont, left, y, "Overtime Pay:",   $"LKR {overtimePay:N2}");
        y += 6;
        g.DrawLine(Pens.Black, left, y, right, y);
        y += 8;
        y = DrawRow(g, totalFont, left, y, "TOTAL PAY:",     $"LKR {_totalPay:N2}");

        // ── Footer (small, italic) ──
        y += 40;
        g.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", footFont, Brushes.Black, left, y);
        y += footFont.GetHeight(g) + 24;
        g.DrawString("Worker Signature: ____________________", footFont, Brushes.Black, left, y);

        e.HasMorePages = false;
    }

    /// <summary>Draws one label/value row and returns the next y position.</summary>
    private static float DrawRow(Graphics g, Font font, float left, float y,
                                 string label, string value)
    {
        const float labelWidth = 170F;
        g.DrawString(label, font, Brushes.Black, left, y);
        g.DrawString(value, font, Brushes.Black, left + labelWidth, y);
        return y + font.GetHeight(g) + 6;
    }
}
