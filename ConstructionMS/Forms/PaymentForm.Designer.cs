namespace ConstructionMS.Forms;

partial class PaymentForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Layout panels ─────────────────────────────────────────────────────────
    private Panel pnlLeft;
    private Panel pnlRight;
    private Panel pnlRightFilter;
    private Panel pnlBudgetInfo;      // styled budget summary box

    // ── Left panel — entry controls ───────────────────────────────────────────
    private Label          lblFormTitle;
    private Label          lblPayeeType;
    private ComboBox       cmbPayeeType;
    private Label          lblReference;
    private TextBox        txtReference;
    private Label          lblAmount;
    private NumericUpDown  numAmount;
    private Label          lblDate;
    private DateTimePicker dtpDate;
    private Label          lblNote;
    private TextBox        txtNote;

    // Budget info — four individual labels inside pnlBudgetInfo
    private Label lblBudgetProject;
    private Label lblBudgetBudget;
    private Label lblBudgetSpent;
    private Label lblBudgetRemaining;

    private Button btnSave;
    private Label  lblStatus;

    // ── Right panel — history controls ────────────────────────────────────────
    private Label          lblHistoryTitle;
    private Label          lblFrom;
    private DateTimePicker dtpFrom;
    private Label          lblTo;
    private DateTimePicker dtpTo;
    private Button         btnRefresh;
    private DataGridView   dgvPayments;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlLeft          = new Panel();
        pnlRight         = new Panel();
        pnlRightFilter   = new Panel();
        pnlBudgetInfo    = new Panel();
        lblFormTitle     = new Label();
        lblPayeeType     = new Label();
        cmbPayeeType     = new ComboBox();
        lblReference     = new Label();
        txtReference     = new TextBox();
        lblAmount        = new Label();
        numAmount        = new NumericUpDown();
        lblDate          = new Label();
        dtpDate          = new DateTimePicker();
        lblNote          = new Label();
        txtNote          = new TextBox();
        lblBudgetProject   = new Label();
        lblBudgetBudget    = new Label();
        lblBudgetSpent     = new Label();
        lblBudgetRemaining = new Label();
        btnSave          = new Button();
        lblStatus        = new Label();
        lblHistoryTitle  = new Label();
        lblFrom          = new Label();
        dtpFrom          = new DateTimePicker();
        lblTo            = new Label();
        dtpTo            = new DateTimePicker();
        btnRefresh       = new Button();
        dgvPayments      = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)numAmount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvPayments).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // LEFT PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlLeft.Dock        = DockStyle.Left;
        pnlLeft.Width       = 380;
        pnlLeft.BackColor   = Color.FromArgb(248, 249, 250);
        pnlLeft.AutoScroll  = true;

        const int lx = 16;
        const int fw = 336;
        int y = 14;

        // Section title
        lblFormTitle.Text      = "Record New Payment";
        lblFormTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblFormTitle.Location  = new Point(lx, y);
        lblFormTitle.Size      = new Size(fw, 26);
        y += 34;

        // ── Payee Type ────────────────────────────────────────────────────────
        lblPayeeType.Text      = "Payee Type";
        lblPayeeType.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPayeeType.ForeColor = Color.FromArgb(60, 60, 60);
        lblPayeeType.Location  = new Point(lx, y);
        lblPayeeType.Size      = new Size(fw, 18);
        y += 20;

        cmbPayeeType.Items.AddRange(new object[] { "Supplier", "Contractor", "Worker" });
        cmbPayeeType.SelectedIndex = 0;
        cmbPayeeType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPayeeType.Font          = new Font("Segoe UI", 9.5F);
        cmbPayeeType.Location      = new Point(lx, y);
        cmbPayeeType.Size          = new Size(fw, 26);
        y += 34;

        // ── Reference ─────────────────────────────────────────────────────────
        lblReference.Text      = "Reference  *";
        lblReference.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblReference.ForeColor = Color.FromArgb(60, 60, 60);
        lblReference.Location  = new Point(lx, y);
        lblReference.Size      = new Size(fw, 18);
        y += 20;

        txtReference.Font     = new Font("Segoe UI", 9.5F);
        txtReference.Location = new Point(lx, y);
        txtReference.Size     = new Size(fw, 26);
        txtReference.TabIndex = 1;
        y += 34;

        // ── Amount ────────────────────────────────────────────────────────────
        lblAmount.Text      = "Amount (LKR)  *";
        lblAmount.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAmount.ForeColor = Color.FromArgb(60, 60, 60);
        lblAmount.Location  = new Point(lx, y);
        lblAmount.Size      = new Size(fw, 18);
        y += 20;

        numAmount.Minimum            = 0;
        numAmount.Maximum            = 100_000_000;
        numAmount.DecimalPlaces      = 2;
        numAmount.ThousandsSeparator = true;
        numAmount.Font               = new Font("Segoe UI", 9.5F);
        numAmount.Location           = new Point(lx, y);
        numAmount.Size               = new Size(fw, 26);
        numAmount.TabIndex           = 2;
        y += 34;

        // ── Payment Date ──────────────────────────────────────────────────────
        lblDate.Text      = "Payment Date";
        lblDate.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblDate.ForeColor = Color.FromArgb(60, 60, 60);
        lblDate.Location  = new Point(lx, y);
        lblDate.Size      = new Size(fw, 18);
        y += 20;

        dtpDate.Format   = DateTimePickerFormat.Short;
        dtpDate.Value    = DateTime.Today;
        dtpDate.Font     = new Font("Segoe UI", 9.5F);
        dtpDate.Location = new Point(lx, y);
        dtpDate.Size     = new Size(fw, 26);
        dtpDate.TabIndex = 3;
        y += 34;

        // ── Note ──────────────────────────────────────────────────────────────
        lblNote.Text      = "Note (optional)";
        lblNote.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNote.ForeColor = Color.FromArgb(60, 60, 60);
        lblNote.Location  = new Point(lx, y);
        lblNote.Size      = new Size(fw, 18);
        y += 20;

        txtNote.Multiline    = true;
        txtNote.Font         = new Font("Segoe UI", 9.5F);
        txtNote.Location     = new Point(lx, y);
        txtNote.Size         = new Size(fw, 58);
        txtNote.TabIndex     = 4;
        txtNote.ScrollBars   = ScrollBars.Vertical;
        y += 66;

        // ── Budget Info Box ───────────────────────────────────────────────────
        pnlBudgetInfo.Location  = new Point(lx, y);
        pnlBudgetInfo.Size      = new Size(fw, 90);
        pnlBudgetInfo.BackColor = Color.FromArgb(235, 244, 255);
        pnlBudgetInfo.Padding   = new Padding(8, 6, 8, 6);

        // Four labels stacked inside the budget box
        int by = 6;
        lblBudgetProject.Text      = "";
        lblBudgetProject.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblBudgetProject.ForeColor = Color.FromArgb(30, 90, 160);
        lblBudgetProject.Location  = new Point(8, by);
        lblBudgetProject.Size      = new Size(fw - 16, 18);
        by += 20;

        lblBudgetBudget.Text      = "";
        lblBudgetBudget.Font      = new Font("Segoe UI", 8.5F);
        lblBudgetBudget.ForeColor = Color.FromArgb(50, 50, 50);
        lblBudgetBudget.Location  = new Point(8, by);
        lblBudgetBudget.Size      = new Size(fw - 16, 17);
        by += 18;

        lblBudgetSpent.Text      = "";
        lblBudgetSpent.Font      = new Font("Segoe UI", 8.5F);
        lblBudgetSpent.ForeColor = Color.FromArgb(50, 50, 50);
        lblBudgetSpent.Location  = new Point(8, by);
        lblBudgetSpent.Size      = new Size(fw - 16, 17);
        by += 18;

        lblBudgetRemaining.Text      = "";
        lblBudgetRemaining.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblBudgetRemaining.ForeColor = Color.SeaGreen;
        lblBudgetRemaining.Location  = new Point(8, by);
        lblBudgetRemaining.Size      = new Size(fw - 16, 17);

        pnlBudgetInfo.Controls.AddRange(new Control[] {
            lblBudgetProject, lblBudgetBudget, lblBudgetSpent, lblBudgetRemaining
        });
        y += 98;

        // ── Save button ───────────────────────────────────────────────────────
        btnSave.Text                      = "Save Payment";
        btnSave.Font                      = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnSave.BackColor                 = Color.FromArgb(40, 167, 69);
        btnSave.ForeColor                 = Color.White;
        btnSave.FlatStyle                 = FlatStyle.Flat;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Location                  = new Point(lx, y);
        btnSave.Size                      = new Size(fw, 38);
        btnSave.TabIndex                  = 5;
        btnSave.Cursor                    = Cursors.Hand;
        btnSave.Click                    += BtnSave_Click;
        y += 46;

        // ── Status label ──────────────────────────────────────────────────────
        lblStatus.Text      = "";
        lblStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location  = new Point(lx, y);
        lblStatus.Size      = new Size(fw, 40);
        lblStatus.Visible   = false;
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;

        pnlLeft.Controls.AddRange(new Control[] {
            lblFormTitle,
            lblPayeeType,  cmbPayeeType,
            lblReference,  txtReference,
            lblAmount,     numAmount,
            lblDate,       dtpDate,
            lblNote,       txtNote,
            pnlBudgetInfo,
            btnSave,       lblStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // RIGHT PANEL — history
        // ══════════════════════════════════════════════════════════════════════
        pnlRight.Dock      = DockStyle.Fill;
        pnlRight.BackColor = Color.White;
        pnlRight.Padding   = new Padding(12, 12, 12, 12);

        // Section title
        lblHistoryTitle.Text      = "Payment History";
        lblHistoryTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblHistoryTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblHistoryTitle.Dock      = DockStyle.Top;
        lblHistoryTitle.Height    = 30;

        // Filter toolbar
        pnlRightFilter.Dock      = DockStyle.Top;
        pnlRightFilter.Height    = 40;
        pnlRightFilter.BackColor = Color.White;
        pnlRightFilter.Padding   = new Padding(0, 4, 0, 0);

        lblFrom.Text      = "From:";
        lblFrom.Font      = new Font("Segoe UI", 9F);
        lblFrom.Location  = new Point(0, 9);
        lblFrom.Size      = new Size(42, 22);
        lblFrom.TextAlign = ContentAlignment.MiddleLeft;

        dtpFrom.Format   = DateTimePickerFormat.Short;
        dtpFrom.Value    = DateTime.Today.AddMonths(-1);
        dtpFrom.Font     = new Font("Segoe UI", 9F);
        dtpFrom.Location = new Point(46, 8);
        dtpFrom.Size     = new Size(130, 24);

        lblTo.Text      = "To:";
        lblTo.Font      = new Font("Segoe UI", 9F);
        lblTo.Location  = new Point(186, 9);
        lblTo.Size      = new Size(28, 22);
        lblTo.TextAlign = ContentAlignment.MiddleLeft;

        dtpTo.Format   = DateTimePickerFormat.Short;
        dtpTo.Value    = DateTime.Today;
        dtpTo.Font     = new Font("Segoe UI", 9F);
        dtpTo.Location = new Point(218, 8);
        dtpTo.Size     = new Size(130, 24);

        btnRefresh.Text                         = "Refresh";
        btnRefresh.Font                         = new Font("Segoe UI", 9F);
        btnRefresh.FlatStyle                    = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderColor   = Color.LightGray;
        btnRefresh.BackColor                    = Color.WhiteSmoke;
        btnRefresh.Location                     = new Point(358, 6);
        btnRefresh.Size                         = new Size(100, 28);
        btnRefresh.Cursor                       = Cursors.Hand;
        btnRefresh.Click                       += BtnRefresh_Click;

        pnlRightFilter.Controls.AddRange(new Control[] {
            lblFrom, dtpFrom, lblTo, dtpTo, btnRefresh
        });

        // DataGridView
        dgvPayments.Dock                        = DockStyle.Fill;
        dgvPayments.ReadOnly                    = true;
        dgvPayments.AllowUserToAddRows          = false;
        dgvPayments.AllowUserToDeleteRows       = false;
        dgvPayments.AutoSizeColumnsMode         = DataGridViewAutoSizeColumnsMode.Fill;
        dgvPayments.SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
        dgvPayments.RowHeadersVisible           = false;
        dgvPayments.BackgroundColor             = Color.White;
        dgvPayments.BorderStyle                 = BorderStyle.None;
        dgvPayments.Font                        = new Font("Segoe UI", 9F);
        dgvPayments.ColumnHeadersDefaultCellStyle.Font
                                                = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvPayments.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 90, 160);
        dgvPayments.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvPayments.EnableHeadersVisualStyles   = false;
        dgvPayments.RowTemplate.Height          = 26;
        dgvPayments.GridColor                   = Color.FromArgb(220, 220, 220);
        dgvPayments.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

        // Add to pnlRight — Fill added first, then Top items
        pnlRight.Controls.Add(dgvPayments);
        pnlRight.Controls.Add(pnlRightFilter);
        pnlRight.Controls.Add(lblHistoryTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(980, 590);
        Text          = "Payment Management";
        StartPosition = FormStartPosition.CenterParent;
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;
        MinimumSize   = new Size(860, 520);

        // Fill must be added before Left
        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);

        ((System.ComponentModel.ISupportInitialize)numAmount).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvPayments).EndInit();
        ResumeLayout(false);
    }
}
