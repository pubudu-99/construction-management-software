using System.Text;
using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Module 6 — Reports form.
/// Three tabs: Financial Summary, Payroll Summary, Stock Status.
/// </summary>
public partial class ReportForm : Form
{
    private readonly DbConnectionFactory     _factory;
    private readonly ProjectRepository       _projectRepo;
    private readonly ReportRepository        _reportRepo;
    private readonly WorkerRepository        _workerRepo;
    private readonly AttendanceRepository    _attendanceRepo;
    private readonly MaterialRepository      _matRepo;
    private readonly StockMovementRepository _movRepo;
    private readonly InventoryService        _inventoryService;
    private readonly PayrollCalculator       _calculator;
    private readonly ActivityLogRepository   _activityRepo;

    // Cache last-run data for CSV export.
    private List<PaymentSummaryLine>          _paymentSummary  = new();
    private List<PayrollLine>                 _payrollLines    = new();
    private List<StockStatusLine>             _stockLines      = new();
    private List<Models.ActivityLogEntry>     _activityEntries = new();

    /// <summary>Initialises the form with database access.</summary>
    public ReportForm(DbConnectionFactory factory)
    {
        _factory          = factory;
        _projectRepo      = new ProjectRepository(factory);
        _reportRepo       = new ReportRepository(factory);
        _workerRepo       = new WorkerRepository(factory);
        _attendanceRepo   = new AttendanceRepository(factory);
        _matRepo          = new MaterialRepository(factory);
        _movRepo          = new StockMovementRepository(factory);
        _inventoryService = new InventoryService(_matRepo, _movRepo);
        _calculator       = new PayrollCalculator();
        _activityRepo     = new ActivityLogRepository(factory);

        InitializeComponent();

        GridStyle.Apply(dgvPaymentSummary);
        GridStyle.Apply(dgvPaySummary);
        GridStyle.Apply(dgvStockStatus);
        GridStyle.Apply(dgvActivityLog);

        // Default payroll period: first of this month → today
        dtpPaySumFrom.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtpPaySumTo.Value   = DateTime.Today;

        Load += ReportForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Loads all three tabs when the form first opens.</summary>
    private void ReportForm_Load(object? sender, EventArgs e)
    {
        LoadFinancialTab();
        LoadStockTab();
        // Payroll tab is generated on demand (user must click Generate).
    }

    // ── Tab 1 — Financial Summary ─────────────────────────────────────────────

    /// <summary>Loads project budget figures and payment summary grid.</summary>
    private void LoadFinancialTab()
    {
        var project = _projectRepo.GetFirst();

        if (project is null)
        {
            lblProjectTitle.Text = "Project:  (none found)";
            lblBudget.Text       = "Budget:       LKR 0.00";
            lblSpent.Text        = "Spent:         LKR 0.00";
            lblRemaining.Text    = "Remaining:  LKR 0.00";
            lblPctUsed.Text      = "% Used:       0.0%";
            dgvPaymentSummary.DataSource = null;
            return;
        }

        decimal remaining = project.Budget - project.Spent;
        decimal pct       = project.Budget > 0 ? project.Spent / project.Budget * 100m : 0m;

        lblProjectTitle.Text = $"Project:  {project.Name}";
        lblBudget.Text       = $"Budget:       LKR {project.Budget:N2}";
        lblSpent.Text        = $"Spent:         LKR {project.Spent:N2}";
        lblRemaining.Text    = $"Remaining:  LKR {remaining:N2}";
        lblPctUsed.Text      = $"% Used:       {pct:F1}%";

        // Colour coding
        lblSpent.ForeColor     = project.Spent > project.Budget * 0.8m
            ? Color.FromArgb(220, 53, 69)
            : Color.FromArgb(40, 40, 40);

        lblRemaining.ForeColor = remaining > project.Budget * 0.2m
            ? Color.FromArgb(40, 167, 69)
            : Color.FromArgb(220, 53, 69);

        // Payment summary grid
        _paymentSummary = _reportRepo.GetPaymentSummaryByType(project.ProjectId);

        var rows = _paymentSummary
            .Select(p => new
            {
                Payee_Type        = p.PayeeType,
                Count             = p.Count,
                Total_Amount_LKR  = p.Total
            })
            .ToList();

        dgvPaymentSummary.DataSource = rows;

        if (dgvPaymentSummary.Columns.Count > 0)
        {
            SetFinColHeader("Payee_Type",       "Payee Type");
            SetFinColHeader("Count",             "Count");
            SetFinColHeader("Total_Amount_LKR",  "Total Amount (LKR)");

            SetFinColWeight("Payee_Type",       200);
            SetFinColWeight("Count",             100);
            SetFinColWeight("Total_Amount_LKR",  200);

            if (dgvPaymentSummary.Columns["Total_Amount_LKR"] is { } c)
            {
                c.DefaultCellStyle.Format    = "N2";
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dgvPaymentSummary.Columns["Count"] is { } cc)
                cc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
    }

    /// <summary>Applies italic style to the Total row if added, no-op here.</summary>
    private void DgvPaymentSummary_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e) { }

    // ── Tab 2 — Payroll Summary ───────────────────────────────────────────────

    /// <summary>Generates the payroll report for the selected date range.</summary>
    private void BtnRunPaySummary_Click(object sender, EventArgs e)
    {
        var from = dtpPaySumFrom.Value.Date;
        var to   = dtpPaySumTo.Value.Date;

        if (from > to)
        {
            MessageBox.Show("'From' date must not be later than 'To' date.",
                            "Invalid Period", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var workers = _workerRepo.GetActive();
        if (workers.Count == 0)
        {
            dgvPaySummary.DataSource    = null;
            lblPaySummaryTotal.Text     = "Grand Total:  LKR 0.00  (no active workers)";
            return;
        }

        _payrollLines = _calculator.GenerateReport(workers, _attendanceRepo, from, to);

        var rows = _payrollLines
            .OrderBy(l => l.WorkerName)
            .Select(l => new
            {
                Worker         = l.WorkerName,
                Regular_Hours  = l.RegularHours,
                Overtime_Hours = l.OvertimeHours,
                Total_Pay_LKR  = l.TotalPay
            })
            .ToList();

        dgvPaySummary.DataSource = rows;

        if (dgvPaySummary.Columns.Count > 0)
        {
            foreach (DataGridViewColumn col in dgvPaySummary.Columns)
            {
                if (col.Index > 0)
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dgvPaySummary.Columns["Total_Pay_LKR"] is { } pc)
                pc.DefaultCellStyle.Format = "N2";
            foreach (var key in new[] { "Regular_Hours", "Overtime_Hours" })
            {
                if (dgvPaySummary.Columns[key] is { } hc)
                    hc.DefaultCellStyle.Format = "0.#";
            }
        }

        decimal grand = _payrollLines.Sum(l => l.TotalPay);
        lblPaySummaryTotal.Text = $"Grand Total:  LKR {grand:N2}   ({workers.Count} workers)";
    }

    // ── Tab 3 — Stock Status ──────────────────────────────────────────────────

    /// <summary>Loads (or reloads) the stock status grid.</summary>
    private void LoadStockTab()
    {
        _stockLines = _inventoryService.GetStockStatusReport();

        var rows = _stockLines
            .Select(s => new
            {
                Material      = s.MaterialName,
                s.Unit,
                Current_Stock = s.CurrentStock,
                Reorder_Point = s.ReorderPoint,
                s.Status
            })
            .ToList();

        dgvStockStatus.DataSource = rows;

        if (dgvStockStatus.Columns.Count > 0)
        {
            SetStockColHeader("Material",      "Material");
            SetStockColHeader("Unit",          "Unit");
            SetStockColHeader("Current_Stock", "Current Stock");
            SetStockColHeader("Reorder_Point", "Reorder Point");
            SetStockColHeader("Status",        "Status");

            SetStockColWeight("Material",      220);
            SetStockColWeight("Unit",          80);
            SetStockColWeight("Current_Stock", 130);
            SetStockColWeight("Reorder_Point", 130);
            SetStockColWeight("Status",        120);

            foreach (var key in new[] { "Current_Stock", "Reorder_Point" })
            {
                if (dgvStockStatus.Columns[key] is { } c)
                {
                    c.DefaultCellStyle.Format    = "0.##";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
        }
    }

    /// <summary>Applies status colours to the Status column in the stock grid.</summary>
    private void DgvStockStatus_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvStockStatus.Rows[e.RowIndex].DataBoundItem is null) return;
        if (e.CellStyle is not { } cs) return;
        if (dgvStockStatus.Columns[e.ColumnIndex]?.Name != "Status") return;

        string status = dgvStockStatus.Rows[e.RowIndex].Cells["Status"]?.Value?.ToString() ?? "";

        switch (status)
        {
            case "Out of Stock":
            case "Low Stock":
                cs.BackColor          = GridStyle.StatusRed;
                cs.ForeColor          = GridStyle.StatusWhite;
                cs.SelectionBackColor = GridStyle.StatusRedSel;
                cs.SelectionForeColor = GridStyle.StatusWhite;
                break;

            case "Getting Low":
                cs.BackColor          = GridStyle.StatusAmber;
                cs.ForeColor          = GridStyle.StatusBlack;
                cs.SelectionBackColor = GridStyle.StatusAmberSel;
                cs.SelectionForeColor = GridStyle.StatusBlack;
                break;

            case "OK":
                cs.BackColor          = GridStyle.StatusGreen;
                cs.ForeColor          = GridStyle.StatusWhite;
                cs.SelectionBackColor = GridStyle.StatusGreenSel;
                cs.SelectionForeColor = GridStyle.StatusWhite;
                break;
        }
    }

    /// <summary>Reloads the stock grid from the database.</summary>
    private void BtnRefreshStock_Click(object sender, EventArgs e) => LoadStockTab();

    /// <summary>Reloads the relevant tab when the user switches to it.</summary>
    private void TabReports_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (tabReports.SelectedTab == tabStock)
            LoadStockTab();
        else if (tabReports.SelectedTab == tabActivity)
            LoadActivityTab();
    }

    // ── Tab 4 — Activity Log ──────────────────────────────────────────────────

    /// <summary>Loads the most recent 200 audit-trail entries into the grid.</summary>
    private void LoadActivityTab()
    {
        _activityEntries = _activityRepo.GetRecent(200);

        var rows = _activityEntries
            .Select(a => new
            {
                Timestamp = a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                a.Username,
                a.Action,
                a.Details
            })
            .ToList();

        dgvActivityLog.DataSource = rows;

        if (dgvActivityLog.Columns.Count > 0)
        {
            if (dgvActivityLog.Columns["Timestamp"] is { } ts) ts.FillWeight = 160;
            if (dgvActivityLog.Columns["Username"]  is { } un) un.FillWeight = 110;
            if (dgvActivityLog.Columns["Action"]    is { } ac) ac.FillWeight = 150;
            if (dgvActivityLog.Columns["Details"]   is { } de) de.FillWeight = 320;
        }
    }

    /// <summary>Reloads the activity log from the database.</summary>
    private void BtnRefreshLog_Click(object sender, EventArgs e) => LoadActivityTab();

    /// <summary>Exports the activity log to a CSV file chosen via SaveFileDialog.</summary>
    private void BtnExportLog_Click(object sender, EventArgs e)
    {
        if (_activityEntries.Count == 0)
        {
            MessageBox.Show("No activity to export.", "Export", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,Username,Action,Details");
        foreach (var a in _activityEntries)
            sb.AppendLine(
                $"{a.Timestamp:yyyy-MM-dd HH:mm:ss},{CsvCell(a.Username)},{CsvCell(a.Action)},{CsvCell(a.Details)}");

        SaveCsv($"activity-log-{DateTime.Today:yyyy-MM-dd}.csv", sb.ToString());
    }

    // ── CSV Export ────────────────────────────────────────────────────────────

    /// <summary>Exports the payment summary to a CSV file chosen via SaveFileDialog.</summary>
    private void BtnExportFinancial_Click(object sender, EventArgs e)
    {
        if (_paymentSummary.Count == 0)
        {
            MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Payee Type,Count,Total Amount (LKR)");
        foreach (var r in _paymentSummary)
            sb.AppendLine($"{CsvCell(r.PayeeType)},{r.Count},{r.Total:F2}");

        SaveCsv($"financial-summary-{DateTime.Today:yyyy-MM-dd}.csv", sb.ToString());
    }

    /// <summary>Exports the payroll summary to a CSV file chosen via SaveFileDialog.</summary>
    private void BtnExportPayroll_Click(object sender, EventArgs e)
    {
        if (_payrollLines.Count == 0)
        {
            MessageBox.Show("Run the payroll report first.", "Export",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Worker,Regular Hours,Overtime Hours,Total Pay (LKR)");
        foreach (var l in _payrollLines.OrderBy(x => x.WorkerName))
            sb.AppendLine($"{CsvCell(l.WorkerName)},{l.RegularHours:F2},{l.OvertimeHours:F2},{l.TotalPay:F2}");

        SaveCsv($"payroll-{DateTime.Today:yyyy-MM-dd}.csv", sb.ToString());
    }

    /// <summary>Exports the stock status to a CSV file chosen via SaveFileDialog.</summary>
    private void BtnExportStock_Click(object sender, EventArgs e)
    {
        if (_stockLines.Count == 0)
        {
            MessageBox.Show("No stock data to export.", "Export", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("Material,Unit,Current Stock,Reorder Point,Status");
        foreach (var s in _stockLines)
            sb.AppendLine($"{CsvCell(s.MaterialName)},{CsvCell(s.Unit)},{s.CurrentStock:F2},{s.ReorderPoint:F2},{CsvCell(s.Status)}");

        SaveCsv($"stock-status-{DateTime.Today:yyyy-MM-dd}.csv", sb.ToString());
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Shows a SaveFileDialog and writes <paramref name="content"/> to the chosen path.</summary>
    private void SaveCsv(string defaultName, string content)
    {
        using var dlg = new SaveFileDialog
        {
            Title    = "Export CSV",
            Filter   = "CSV Files (*.csv)|*.csv",
            FileName = defaultName
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        File.WriteAllText(dlg.FileName, content, Encoding.UTF8);
        MessageBox.Show($"Exported to:\n{dlg.FileName}", "Export Complete",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>Wraps a CSV cell value in quotes if it contains a comma or quote.</summary>
    private static string CsvCell(string value)
    {
        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    // Null-safe column helpers — Financial tab
    private void SetFinColHeader(string n, string h)  { if (dgvPaymentSummary.Columns[n] is { } c) c.HeaderText = h; }
    private void SetFinColWeight(string n, int w)     { if (dgvPaymentSummary.Columns[n] is { } c) c.FillWeight = w; }

    // Null-safe column helpers — Stock tab
    private void SetStockColHeader(string n, string h) { if (dgvStockStatus.Columns[n] is { } c) c.HeaderText = h; }
    private void SetStockColWeight(string n, int w)    { if (dgvStockStatus.Columns[n] is { } c) c.FillWeight = w; }
}
