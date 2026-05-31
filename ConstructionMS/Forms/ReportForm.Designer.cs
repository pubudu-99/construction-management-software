namespace ConstructionMS.Forms;

partial class ReportForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Shared ────────────────────────────────────────────────────────────────
    private TabControl   tabReports;
    private TabPage      tabFinancial;
    private TabPage      tabPayroll;
    private TabPage      tabStock;
    private TabPage      tabActivity;

    // ── Tab 1 — Financial Summary ─────────────────────────────────────────────
    private Panel        pnlFinHeader;
    private Label        lblProjectTitle;
    private Label        lblBudget;
    private Label        lblSpent;
    private Label        lblRemaining;
    private Label        lblPctUsed;
    private Button       btnExportFinancial;
    private DataGridView dgvPaymentSummary;

    // ── Tab 2 — Payroll Summary ───────────────────────────────────────────────
    private Panel        pnlPayHeader;
    private Label        lblPayFrom;
    private DateTimePicker dtpPaySumFrom;
    private Label        lblPayTo;
    private DateTimePicker dtpPaySumTo;
    private Button       btnRunPaySummary;
    private Label        lblPaySummaryTotal;
    private Button       btnExportPayroll;
    private DataGridView dgvPaySummary;

    // ── Tab 3 — Stock Status ──────────────────────────────────────────────────
    private Panel        pnlStockButtons;
    private Button       btnRefreshStock;
    private Button       btnExportStock;
    private DataGridView dgvStockStatus;

    // ── Tab 4 — Activity Log ──────────────────────────────────────────────────
    private Panel        pnlActivityHeader;
    private Label        lblActivityHeading;
    private Button       btnRefreshLog;
    private Button       btnExportLog;
    private DataGridView dgvActivityLog;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tabReports      = new TabControl();
        tabFinancial    = new TabPage();
        tabPayroll      = new TabPage();
        tabStock        = new TabPage();
        tabActivity     = new TabPage();

        // Financial tab controls
        pnlFinHeader       = new Panel();
        lblProjectTitle    = new Label();
        lblBudget          = new Label();
        lblSpent           = new Label();
        lblRemaining       = new Label();
        lblPctUsed         = new Label();
        btnExportFinancial = new Button();
        dgvPaymentSummary  = new DataGridView();

        // Payroll tab controls
        pnlPayHeader       = new Panel();
        lblPayFrom         = new Label();
        dtpPaySumFrom      = new DateTimePicker();
        lblPayTo           = new Label();
        dtpPaySumTo        = new DateTimePicker();
        btnRunPaySummary   = new Button();
        lblPaySummaryTotal = new Label();
        btnExportPayroll   = new Button();
        dgvPaySummary      = new DataGridView();

        // Stock tab controls
        pnlStockButtons    = new Panel();
        btnRefreshStock    = new Button();
        btnExportStock     = new Button();
        dgvStockStatus     = new DataGridView();

        // Activity Log tab controls
        pnlActivityHeader  = new Panel();
        lblActivityHeading = new Label();
        btnRefreshLog      = new Button();
        btnExportLog       = new Button();
        dgvActivityLog     = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)dgvPaymentSummary).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvPaySummary).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvStockStatus).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvActivityLog).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // TAB 1 — Financial Summary
        // ══════════════════════════════════════════════════════════════════════
        pnlFinHeader.Dock      = DockStyle.Top;
        pnlFinHeader.Height    = 200;
        pnlFinHeader.BackColor = Color.FromArgb(248, 249, 250);
        pnlFinHeader.Padding   = new Padding(16, 12, 16, 8);

        const int lx = 16;
        int fy = 12;

        lblProjectTitle.Text      = "Project:  —";
        lblProjectTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblProjectTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblProjectTitle.Location  = new Point(lx, fy);
        lblProjectTitle.Size      = new Size(680, 24);
        fy += 32;

        lblBudget.Text      = "Budget:       LKR —";
        lblBudget.Font      = new Font("Segoe UI", 10F);
        lblBudget.ForeColor = Color.FromArgb(40, 40, 40);
        lblBudget.Location  = new Point(lx, fy);
        lblBudget.Size      = new Size(400, 22);
        fy += 26;

        lblSpent.Text      = "Spent:         LKR —";
        lblSpent.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSpent.ForeColor = Color.FromArgb(40, 40, 40);
        lblSpent.Location  = new Point(lx, fy);
        lblSpent.Size      = new Size(400, 22);
        fy += 26;

        lblRemaining.Text      = "Remaining:  LKR —";
        lblRemaining.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblRemaining.ForeColor = Color.FromArgb(40, 167, 69);
        lblRemaining.Location  = new Point(lx, fy);
        lblRemaining.Size      = new Size(400, 22);
        fy += 26;

        lblPctUsed.Text      = "% Used:       —";
        lblPctUsed.Font      = new Font("Segoe UI", 10F);
        lblPctUsed.ForeColor = Color.FromArgb(40, 40, 40);
        lblPctUsed.Location  = new Point(lx, fy);
        lblPctUsed.Size      = new Size(400, 22);
        fy += 32;

        btnExportFinancial.Text                      = "Export CSV";
        btnExportFinancial.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportFinancial.BackColor                 = Color.FromArgb(30, 90, 160);
        btnExportFinancial.ForeColor                 = Color.White;
        btnExportFinancial.FlatStyle                 = FlatStyle.Flat;
        btnExportFinancial.FlatAppearance.BorderSize = 0;
        btnExportFinancial.Location                  = new Point(lx, fy);
        btnExportFinancial.Size                      = new Size(120, 30);
        btnExportFinancial.Cursor                    = Cursors.Hand;
        btnExportFinancial.Click                    += BtnExportFinancial_Click;

        pnlFinHeader.Controls.AddRange(new Control[] {
            lblProjectTitle, lblBudget, lblSpent, lblRemaining, lblPctUsed,
            btnExportFinancial
        });

        dgvPaymentSummary.Dock        = DockStyle.Fill;
        dgvPaymentSummary.CellFormatting += DgvPaymentSummary_CellFormatting;

        tabFinancial.Text    = "Financial Summary";
        tabFinancial.Padding = new Padding(0);
        tabFinancial.Controls.Add(dgvPaymentSummary);
        tabFinancial.Controls.Add(pnlFinHeader);

        // ══════════════════════════════════════════════════════════════════════
        // TAB 2 — Payroll Summary
        // ══════════════════════════════════════════════════════════════════════
        pnlPayHeader.Dock      = DockStyle.Top;
        pnlPayHeader.Height    = 92;
        pnlPayHeader.BackColor = Color.FromArgb(248, 249, 250);

        int py = 14;

        lblPayFrom.Text      = "From";
        lblPayFrom.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPayFrom.ForeColor = Color.FromArgb(60, 60, 60);
        lblPayFrom.AutoSize  = true;
        lblPayFrom.Location  = new Point(16, py + 2);

        dtpPaySumFrom.Format   = DateTimePickerFormat.Short;
        dtpPaySumFrom.Font     = new Font("Segoe UI", 9.5F);
        dtpPaySumFrom.Location = new Point(60, py - 2);
        dtpPaySumFrom.Size     = new Size(130, 26);

        lblPayTo.Text      = "To";
        lblPayTo.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPayTo.ForeColor = Color.FromArgb(60, 60, 60);
        lblPayTo.AutoSize  = true;
        lblPayTo.Location  = new Point(204, py + 2);

        dtpPaySumTo.Format   = DateTimePickerFormat.Short;
        dtpPaySumTo.Font     = new Font("Segoe UI", 9.5F);
        dtpPaySumTo.Location = new Point(230, py - 2);
        dtpPaySumTo.Size     = new Size(130, 26);

        btnRunPaySummary.Text                      = "Generate";
        btnRunPaySummary.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRunPaySummary.BackColor                 = Color.FromArgb(40, 167, 69);
        btnRunPaySummary.ForeColor                 = Color.White;
        btnRunPaySummary.FlatStyle                 = FlatStyle.Flat;
        btnRunPaySummary.FlatAppearance.BorderSize = 0;
        btnRunPaySummary.Location                  = new Point(374, py - 2);
        btnRunPaySummary.Size                      = new Size(100, 26);
        btnRunPaySummary.Cursor                    = Cursors.Hand;
        btnRunPaySummary.Click                    += BtnRunPaySummary_Click;
        py += 36;

        lblPaySummaryTotal.Text      = "Grand Total:  LKR 0.00";
        lblPaySummaryTotal.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblPaySummaryTotal.ForeColor = Color.FromArgb(30, 90, 160);
        lblPaySummaryTotal.Location  = new Point(16, py);
        lblPaySummaryTotal.Size      = new Size(320, 20);

        btnExportPayroll.Text                      = "Export CSV";
        btnExportPayroll.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportPayroll.BackColor                 = Color.FromArgb(30, 90, 160);
        btnExportPayroll.ForeColor                 = Color.White;
        btnExportPayroll.FlatStyle                 = FlatStyle.Flat;
        btnExportPayroll.FlatAppearance.BorderSize = 0;
        btnExportPayroll.Location                  = new Point(374, py - 2);
        btnExportPayroll.Size                      = new Size(100, 26);
        btnExportPayroll.Cursor                    = Cursors.Hand;
        btnExportPayroll.Click                    += BtnExportPayroll_Click;

        pnlPayHeader.Controls.AddRange(new Control[] {
            lblPayFrom, dtpPaySumFrom, lblPayTo, dtpPaySumTo,
            btnRunPaySummary, lblPaySummaryTotal, btnExportPayroll
        });

        dgvPaySummary.Dock = DockStyle.Fill;

        tabPayroll.Text    = "Payroll Summary";
        tabPayroll.Padding = new Padding(0);
        tabPayroll.Controls.Add(dgvPaySummary);
        tabPayroll.Controls.Add(pnlPayHeader);

        // ══════════════════════════════════════════════════════════════════════
        // TAB 3 — Stock Status
        // ══════════════════════════════════════════════════════════════════════
        pnlStockButtons.Dock      = DockStyle.Top;
        pnlStockButtons.Height    = 48;
        pnlStockButtons.BackColor = Color.FromArgb(248, 249, 250);

        btnRefreshStock.Text                      = "Refresh";
        btnRefreshStock.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRefreshStock.BackColor                 = Color.FromArgb(108, 117, 125);
        btnRefreshStock.ForeColor                 = Color.White;
        btnRefreshStock.FlatStyle                 = FlatStyle.Flat;
        btnRefreshStock.FlatAppearance.BorderSize = 0;
        btnRefreshStock.Location                  = new Point(16, 10);
        btnRefreshStock.Size                      = new Size(100, 28);
        btnRefreshStock.Cursor                    = Cursors.Hand;
        btnRefreshStock.Click                    += BtnRefreshStock_Click;

        btnExportStock.Text                      = "Export CSV";
        btnExportStock.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportStock.BackColor                 = Color.FromArgb(30, 90, 160);
        btnExportStock.ForeColor                 = Color.White;
        btnExportStock.FlatStyle                 = FlatStyle.Flat;
        btnExportStock.FlatAppearance.BorderSize = 0;
        btnExportStock.Location                  = new Point(128, 10);
        btnExportStock.Size                      = new Size(100, 28);
        btnExportStock.Cursor                    = Cursors.Hand;
        btnExportStock.Click                    += BtnExportStock_Click;

        pnlStockButtons.Controls.AddRange(new Control[] { btnRefreshStock, btnExportStock });

        dgvStockStatus.Dock            = DockStyle.Fill;
        dgvStockStatus.CellFormatting += DgvStockStatus_CellFormatting;

        tabStock.Text    = "Stock Status";
        tabStock.Padding = new Padding(0);
        tabStock.Controls.Add(dgvStockStatus);
        tabStock.Controls.Add(pnlStockButtons);

        // ══════════════════════════════════════════════════════════════════════
        // TAB 4 — Activity Log
        // ══════════════════════════════════════════════════════════════════════
        pnlActivityHeader.Dock      = DockStyle.Top;
        pnlActivityHeader.Height    = 48;
        pnlActivityHeader.BackColor = Color.FromArgb(248, 249, 250);

        btnRefreshLog.Text                      = "Refresh";
        btnRefreshLog.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRefreshLog.BackColor                 = Color.FromArgb(108, 117, 125);
        btnRefreshLog.ForeColor                 = Color.White;
        btnRefreshLog.FlatStyle                 = FlatStyle.Flat;
        btnRefreshLog.FlatAppearance.BorderSize = 0;
        btnRefreshLog.Location                  = new Point(16, 10);
        btnRefreshLog.Size                      = new Size(100, 28);
        btnRefreshLog.Cursor                    = Cursors.Hand;
        btnRefreshLog.Click                    += BtnRefreshLog_Click;

        btnExportLog.Text                      = "Export CSV";
        btnExportLog.Font                      = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnExportLog.BackColor                 = Color.FromArgb(30, 90, 160);
        btnExportLog.ForeColor                 = Color.White;
        btnExportLog.FlatStyle                 = FlatStyle.Flat;
        btnExportLog.FlatAppearance.BorderSize = 0;
        btnExportLog.Location                  = new Point(128, 10);
        btnExportLog.Size                      = new Size(100, 28);
        btnExportLog.Cursor                    = Cursors.Hand;
        btnExportLog.Click                    += BtnExportLog_Click;

        lblActivityHeading.Text      = "Recent Activity (last 200 entries)";
        lblActivityHeading.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblActivityHeading.ForeColor = Color.FromArgb(30, 90, 160);
        lblActivityHeading.AutoSize  = true;
        lblActivityHeading.Location  = new Point(248, 16);

        pnlActivityHeader.Controls.AddRange(new Control[] {
            btnRefreshLog, btnExportLog, lblActivityHeading
        });

        dgvActivityLog.Dock = DockStyle.Fill;

        tabActivity.Text    = "Activity Log";
        tabActivity.Padding = new Padding(0);
        tabActivity.Controls.Add(dgvActivityLog);
        tabActivity.Controls.Add(pnlActivityHeader);

        // ══════════════════════════════════════════════════════════════════════
        // TAB CONTROL
        // ══════════════════════════════════════════════════════════════════════
        tabReports.Dock     = DockStyle.Fill;
        tabReports.Font     = new Font("Segoe UI", 9.5F);
        tabReports.Controls.AddRange(new TabPage[] { tabFinancial, tabPayroll, tabStock, tabActivity });
        tabReports.SelectedIndexChanged += TabReports_SelectedIndexChanged;

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(1000, 700);
        Text          = "Reports";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(900, 650);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(tabReports);

        ((System.ComponentModel.ISupportInitialize)dgvPaymentSummary).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvPaySummary).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvStockStatus).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvActivityLog).EndInit();
        ResumeLayout(false);
    }
}
