using System.Text;
using System.Drawing.Printing;
using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR4 — Generates a gross payroll report for all active workers
/// over a user-selected date range.
/// </summary>
public partial class PayrollForm : Form
{
    private readonly WorkerRepository     _workerRepo;
    private readonly AttendanceRepository _attendanceRepo;
    private readonly ProjectRepository    _projectRepo;
    private readonly PayrollCalculator    _calculator;

    /// <summary>Holds the most recently generated report for CSV export.</summary>
    private List<PayrollLine> _lines = new();

    /// <summary>Initialises the form and defaults the period to the current calendar month.</summary>
    public PayrollForm(DbConnectionFactory factory)
    {
        InitializeComponent();
        GridStyle.Apply(dgvPayroll);
        Theme.Apply(this);
        _workerRepo     = new WorkerRepository(factory);
        _attendanceRepo = new AttendanceRepository(factory);
        _projectRepo    = new ProjectRepository(factory);
        _calculator     = new PayrollCalculator();

        // Default period: first day of this month → today
        dtpFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpTo.Value   = DateTime.Today;
    }

    // ── Generate report ───────────────────────────────────────────────────────

    /// <summary>Generates and displays the payroll report for the selected period.</summary>
    private void BtnGenerate_Click(object? sender, EventArgs e)
    {
        var from = dtpFrom.Value.Date;
        var to   = dtpTo.Value.Date;

        if (from > to)
        {
            MessageBox.Show("'Period From' must not be later than 'Period To'.",
                            "Invalid Period", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var workers = _workerRepo.GetActive();
        if (workers.Count == 0)
        {
            dgvPayroll.DataSource    = null;
            lblWorkerCount.Text      = "Workers:     0";
            lblTotalPay.Text         = "Gross Pay:   LKR 0.00";
            lblReportTitle.Text      = "Payroll Report — no active workers";
            _lines = new();
            btnPrintReceipt.Enabled  = false;
            return;
        }

        var lines = _calculator.GenerateReport(workers, _attendanceRepo, from, to);
        _lines = lines;   // cache for CSV export

        // Project to a display-friendly anonymous type
        var rows = lines
            .OrderBy(l => l.WorkerName)
            .Select(l => new
            {
                Worker           = l.WorkerName,
                Regular_Hours    = l.RegularHours,
                Overtime_Hours   = l.OvertimeHours,
                Total_Pay_LKR    = l.TotalPay
            })
            .ToList();

        dgvPayroll.DataSource = rows;

        // Right-align numeric columns
        foreach (DataGridViewColumn col in dgvPayroll.Columns)
        {
            if (col.Index > 0)   // all except Worker name
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        // Format the Total_Pay_LKR column as currency
        if (dgvPayroll.Columns["Total_Pay_LKR"] is DataGridViewColumn payCol)
            payCol.DefaultCellStyle.Format = "N2";

        // Format hour columns to 1 decimal place
        foreach (var key in new[] { "Regular_Hours", "Overtime_Hours" })
        {
            if (dgvPayroll.Columns[key] is DataGridViewColumn hCol)
                hCol.DefaultCellStyle.Format = "0.#";
        }

        decimal grandTotal = lines.Sum(l => l.TotalPay);
        lblWorkerCount.Text = $"Workers:     {lines.Count}";
        lblTotalPay.Text    = $"Gross Pay:   LKR {grandTotal:N2}";
        lblReportTitle.Text =
            $"Payroll Report  ({from:dd MMM yyyy} – {to:dd MMM yyyy})";
    }

    // ── CSV export ──────────────────────────────────────────────────────────────

    /// <summary>Exports the most recently generated payroll report to a CSV file.</summary>
    private void BtnExportPayroll_Click(object? sender, EventArgs e)
    {
        if (_lines.Count == 0)
        {
            MessageBox.Show("Generate a report first, then export.",
                "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Worker,Regular Hours,Overtime Hours,Total Pay (LKR)");
        foreach (var l in _lines.OrderBy(x => x.WorkerName))
            sb.AppendLine(
                $"{CsvCell(l.WorkerName)},{l.RegularHours:F2},{l.OvertimeHours:F2},{l.TotalPay:F2}");

        using var dlg = new SaveFileDialog
        {
            Title    = "Export Payroll CSV",
            Filter   = "CSV Files (*.csv)|*.csv",
            FileName = $"payroll-{DateTime.Today:yyyy-MM-dd}.csv"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
        MessageBox.Show($"Exported to:\n{dlg.FileName}", "Export Complete",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // ── Print receipt ─────────────────────────────────────────────────────────

    /// <summary>Enables the Print Receipt button only when a worker row is selected.</summary>
    private void DgvPayroll_SelectionChanged(object? sender, EventArgs e)
    {
        btnPrintReceipt.Enabled = _lines.Count > 0 && dgvPayroll.SelectedRows.Count > 0;
    }

    /// <summary>
    /// Builds a printable payslip for the selected worker and shows a print
    /// preview, from which the user can print or save as PDF
    /// ("Microsoft Print to PDF").
    /// </summary>
    private void BtnPrintReceipt_Click(object? sender, EventArgs e)
    {
        if (dgvPayroll.SelectedRows.Count == 0) return;

        string workerName = dgvPayroll.SelectedRows[0].Cells["Worker"]?.Value?.ToString() ?? "";
        var line = _lines.FirstOrDefault(l => l.WorkerName == workerName);
        if (line is null) return;

        // PayrollLine carries no rate — resolve it from the worker record.
        var worker = _workerRepo.GetActive().FirstOrDefault(w => w.WorkerId == line.WorkerId);
        decimal rate = worker?.HourlyRate ?? 0m;

        // Identify the receipt with the current construction project,
        // not the application name. Falls back to a generic header if
        // no active project exists.
        string projectName =
            _projectRepo.GetActive()?.Name
            ?? _projectRepo.GetAll().FirstOrDefault()?.Name
            ?? "";

        var printer = new PayrollReceiptPrinter(
            projectName,
            line.WorkerName, dtpFrom.Value.Date, dtpTo.Value.Date,
            line.RegularHours, line.OvertimeHours,
            rate, AppConfig.OvertimeMultiplier, line.TotalPay);

        using var doc     = printer.Build();
        using var preview = new PrintPreviewDialog { Document = doc, Width = 820, Height = 640 };
        preview.ShowDialog(this);
    }

    /// <summary>Wraps a CSV cell in quotes if it contains a comma or quote.</summary>
    private static string CsvCell(string value)
    {
        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
