namespace ConstructionMS.Forms;

partial class PayrollForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Left panel — period selector ──────────────────────────────────────────
    private Panel         pnlLeft;
    private Label         lblFormTitle;
    private Label         lblFrom;
    private DateTimePicker dtpFrom;
    private Label         lblTo;
    private DateTimePicker dtpTo;
    private Button        btnGenerate;
    private Label         lblSummaryHeading;
    private Label         lblWorkerCount;
    private Label         lblTotalPay;
    private Button        btnExportPayroll;

    // ── Right panel — report grid ─────────────────────────────────────────────
    private Panel         pnlRight;
    private Label         lblReportTitle;
    private DataGridView  dgvPayroll;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlLeft          = new Panel();
        lblFormTitle     = new Label();
        lblFrom          = new Label();
        dtpFrom          = new DateTimePicker();
        lblTo            = new Label();
        dtpTo            = new DateTimePicker();
        btnGenerate      = new Button();
        lblSummaryHeading = new Label();
        lblWorkerCount   = new Label();
        lblTotalPay      = new Label();
        btnExportPayroll = new Button();
        pnlRight         = new Panel();
        lblReportTitle   = new Label();
        dgvPayroll       = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)dgvPayroll).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // LEFT PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlLeft.Dock       = DockStyle.Left;
        pnlLeft.Width      = 280;
        pnlLeft.BackColor  = Color.FromArgb(248, 249, 250);
        pnlLeft.AutoScroll = true;

        const int lx = 16, fw = 244;
        int y = 14;

        lblFormTitle.Text      = "Generate Payroll";
        lblFormTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblFormTitle.Location  = new Point(lx, y);
        lblFormTitle.Size      = new Size(fw, 24);
        y += 36;

        lblFrom.Text      = "Period From  *";
        lblFrom.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblFrom.ForeColor = Color.FromArgb(60, 60, 60);
        lblFrom.Location  = new Point(lx, y);
        lblFrom.Size      = new Size(fw, 18);
        y += 20;

        dtpFrom.Format   = DateTimePickerFormat.Short;
        dtpFrom.Font     = new Font("Segoe UI", 9.5F);
        dtpFrom.Location = new Point(lx, y);
        dtpFrom.Size     = new Size(fw, 26);
        y += 34;

        lblTo.Text      = "Period To  *";
        lblTo.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTo.ForeColor = Color.FromArgb(60, 60, 60);
        lblTo.Location  = new Point(lx, y);
        lblTo.Size      = new Size(fw, 18);
        y += 20;

        dtpTo.Format   = DateTimePickerFormat.Short;
        dtpTo.Font     = new Font("Segoe UI", 9.5F);
        dtpTo.Location = new Point(lx, y);
        dtpTo.Size     = new Size(fw, 26);
        y += 42;

        btnGenerate.Text                      = "Generate Report";
        btnGenerate.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnGenerate.BackColor                 = Color.FromArgb(30, 90, 160);
        btnGenerate.ForeColor                 = Color.White;
        btnGenerate.FlatStyle                 = FlatStyle.Flat;
        btnGenerate.FlatAppearance.BorderSize = 0;
        btnGenerate.Location                  = new Point(lx, y);
        btnGenerate.Size                      = new Size(fw, 34);
        btnGenerate.Cursor                    = Cursors.Hand;
        btnGenerate.Click                    += BtnGenerate_Click;
        y += 50;

        // Divider line via Label with border
        var divider = new Label
        {
            BorderStyle = BorderStyle.Fixed3D,
            Location    = new Point(lx, y),
            Size        = new Size(fw, 2)
        };
        y += 14;

        lblSummaryHeading.Text      = "Summary";
        lblSummaryHeading.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSummaryHeading.ForeColor = Color.FromArgb(30, 90, 160);
        lblSummaryHeading.Location  = new Point(lx, y);
        lblSummaryHeading.Size      = new Size(fw, 18);
        y += 24;

        lblWorkerCount.Text      = "Workers:     —";
        lblWorkerCount.Font      = new Font("Segoe UI", 9F);
        lblWorkerCount.ForeColor = Color.FromArgb(60, 60, 60);
        lblWorkerCount.Location  = new Point(lx, y);
        lblWorkerCount.Size      = new Size(fw, 20);
        y += 26;

        lblTotalPay.Text      = "Gross Pay:   —";
        lblTotalPay.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblTotalPay.ForeColor = Color.FromArgb(40, 167, 69);
        lblTotalPay.Location  = new Point(lx, y);
        lblTotalPay.Size      = new Size(fw, 20);
        y += 34;

        btnExportPayroll.Text                      = "Export CSV";
        btnExportPayroll.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnExportPayroll.BackColor                 = Color.FromArgb(40, 167, 69);
        btnExportPayroll.ForeColor                 = Color.White;
        btnExportPayroll.FlatStyle                 = FlatStyle.Flat;
        btnExportPayroll.FlatAppearance.BorderSize = 0;
        btnExportPayroll.Location                  = new Point(lx, y);
        btnExportPayroll.Size                      = new Size(fw, 32);
        btnExportPayroll.Cursor                    = Cursors.Hand;
        btnExportPayroll.Click                    += BtnExportPayroll_Click;

        pnlLeft.Controls.AddRange(new Control[] {
            lblFormTitle,
            lblFrom, dtpFrom,
            lblTo,   dtpTo,
            btnGenerate,
            divider,
            lblSummaryHeading, lblWorkerCount, lblTotalPay,
            btnExportPayroll
        });

        // ══════════════════════════════════════════════════════════════════════
        // RIGHT PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlRight.Dock      = DockStyle.Fill;
        pnlRight.BackColor = Color.White;
        pnlRight.Padding   = new Padding(12, 12, 12, 12);

        lblReportTitle.Text      = "Payroll Report";
        lblReportTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblReportTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblReportTitle.Dock      = DockStyle.Top;
        lblReportTitle.Height    = 28;

        dgvPayroll.Dock                  = DockStyle.Fill;
        dgvPayroll.ReadOnly              = true;
        dgvPayroll.AllowUserToAddRows    = false;
        dgvPayroll.AllowUserToDeleteRows = false;
        dgvPayroll.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPayroll.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
        dgvPayroll.RowHeadersVisible     = false;
        dgvPayroll.BackgroundColor       = Color.White;
        dgvPayroll.BorderStyle           = BorderStyle.None;
        dgvPayroll.Font                  = new Font("Segoe UI", 9F);
        dgvPayroll.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvPayroll.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 90, 160);
        dgvPayroll.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvPayroll.EnableHeadersVisualStyles   = false;
        dgvPayroll.RowTemplate.Height          = 26;
        dgvPayroll.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

        pnlRight.Controls.Add(dgvPayroll);
        pnlRight.Controls.Add(lblReportTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(900, 500);
        Text          = "Payroll Management";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(720, 400);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);

        ((System.ComponentModel.ISupportInitialize)dgvPayroll).EndInit();
        ResumeLayout(false);
    }
}
