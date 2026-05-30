namespace ConstructionMS.Forms;

partial class AttendanceForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Left panel — entry ────────────────────────────────────────────────────
    private Panel         pnlLeft;
    private Label         lblFormTitle;
    private Label         lblWorker;
    private ComboBox      cmbWorker;
    private Button        btnAddWorker;
    private Label         lblAttDate;
    private DateTimePicker dtpAttDate;
    private Label         lblHours;
    private NumericUpDown numHours;
    private Button        btnSaveAttendance;
    private Label         lblAttStatus;

    // ── Right panel — history ─────────────────────────────────────────────────
    private Panel         pnlRight;
    private Label         lblHistoryTitle;
    private Panel         pnlHistFilter;
    private Label         lblHistFrom;
    private DateTimePicker dtpHistFrom;
    private Label         lblHistTo;
    private DateTimePicker dtpHistTo;
    private Button        btnRefreshHist;
    private DataGridView  dgvAttendance;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlLeft           = new Panel();
        lblFormTitle      = new Label();
        lblWorker         = new Label();
        cmbWorker         = new ComboBox();
        btnAddWorker      = new Button();
        lblAttDate        = new Label();
        dtpAttDate        = new DateTimePicker();
        lblHours          = new Label();
        numHours          = new NumericUpDown();
        btnSaveAttendance = new Button();
        lblAttStatus      = new Label();
        pnlRight          = new Panel();
        lblHistoryTitle   = new Label();
        pnlHistFilter     = new Panel();
        lblHistFrom       = new Label();
        dtpHistFrom       = new DateTimePicker();
        lblHistTo         = new Label();
        dtpHistTo         = new DateTimePicker();
        btnRefreshHist    = new Button();
        dgvAttendance     = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)numHours).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvAttendance).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // LEFT PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlLeft.Dock      = DockStyle.Left;
        pnlLeft.Width     = 360;
        pnlLeft.BackColor = Color.FromArgb(248, 249, 250);
        pnlLeft.AutoScroll = true;

        const int lx = 16, fw = 320;
        int y = 14;

        lblFormTitle.Text      = "Record Attendance";
        lblFormTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblFormTitle.Location  = new Point(lx, y);
        lblFormTitle.Size      = new Size(fw, 24);
        y += 34;

        lblWorker.Text      = "Worker  *";
        lblWorker.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblWorker.ForeColor = Color.FromArgb(60, 60, 60);
        lblWorker.Location  = new Point(lx, y);
        lblWorker.Size      = new Size(fw, 18);
        y += 20;

        cmbWorker.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbWorker.Font          = new Font("Segoe UI", 9.5F);
        cmbWorker.Location      = new Point(lx, y);
        cmbWorker.Size          = new Size(fw - 110, 26);

        btnAddWorker.Text                      = "+ Add Worker";
        btnAddWorker.Font                      = new Font("Segoe UI", 8.5F);
        btnAddWorker.FlatStyle                 = FlatStyle.Flat;
        btnAddWorker.FlatAppearance.BorderColor = Color.SteelBlue;
        btnAddWorker.ForeColor                 = Color.SteelBlue;
        btnAddWorker.BackColor                 = Color.White;
        btnAddWorker.Location                  = new Point(lx + fw - 104, y);
        btnAddWorker.Size                      = new Size(104, 26);
        btnAddWorker.Cursor                    = Cursors.Hand;
        btnAddWorker.Click                    += BtnAddWorker_Click;
        y += 34;

        lblAttDate.Text      = "Date  *";
        lblAttDate.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAttDate.ForeColor = Color.FromArgb(60, 60, 60);
        lblAttDate.Location  = new Point(lx, y);
        lblAttDate.Size      = new Size(fw, 18);
        y += 20;

        dtpAttDate.Format   = DateTimePickerFormat.Short;
        dtpAttDate.Value    = DateTime.Today;
        dtpAttDate.Font     = new Font("Segoe UI", 9.5F);
        dtpAttDate.Location = new Point(lx, y);
        dtpAttDate.Size     = new Size(fw, 26);
        y += 34;

        lblHours.Text      = "Hours Worked  *";
        lblHours.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblHours.ForeColor = Color.FromArgb(60, 60, 60);
        lblHours.Location  = new Point(lx, y);
        lblHours.Size      = new Size(fw, 18);
        y += 20;

        numHours.Minimum       = new decimal(new int[] { 5, 0, 0, 65536 }); // 0.5
        numHours.Maximum       = 24;
        numHours.DecimalPlaces = 1;
        numHours.Increment     = new decimal(new int[] { 5, 0, 0, 65536 }); // 0.5
        numHours.Value         = 8;
        numHours.Font          = new Font("Segoe UI", 9.5F);
        numHours.Location      = new Point(lx, y);
        numHours.Size          = new Size(fw, 26);
        y += 34;

        btnSaveAttendance.Text                      = "Record Attendance";
        btnSaveAttendance.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveAttendance.BackColor                 = Color.FromArgb(40, 167, 69);
        btnSaveAttendance.ForeColor                 = Color.White;
        btnSaveAttendance.FlatStyle                 = FlatStyle.Flat;
        btnSaveAttendance.FlatAppearance.BorderSize = 0;
        btnSaveAttendance.Location                  = new Point(lx, y);
        btnSaveAttendance.Size                      = new Size(fw, 34);
        btnSaveAttendance.Cursor                    = Cursors.Hand;
        btnSaveAttendance.Click                    += BtnSaveAttendance_Click;
        y += 42;

        lblAttStatus.Text      = "";
        lblAttStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAttStatus.Location  = new Point(lx, y);
        lblAttStatus.Size      = new Size(fw, 40);
        lblAttStatus.Visible   = false;
        lblAttStatus.TextAlign = ContentAlignment.MiddleCenter;

        pnlLeft.Controls.AddRange(new Control[] {
            lblFormTitle,
            lblWorker, cmbWorker, btnAddWorker,
            lblAttDate, dtpAttDate,
            lblHours, numHours,
            btnSaveAttendance, lblAttStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // RIGHT PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlRight.Dock      = DockStyle.Fill;
        pnlRight.BackColor = Color.White;
        pnlRight.Padding   = new Padding(12, 12, 12, 12);

        lblHistoryTitle.Text      = "Attendance History";
        lblHistoryTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblHistoryTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblHistoryTitle.Dock      = DockStyle.Top;
        lblHistoryTitle.Height    = 28;

        pnlHistFilter.Dock      = DockStyle.Top;
        pnlHistFilter.Height    = 38;
        pnlHistFilter.BackColor = Color.White;

        lblHistFrom.Text      = "From:";
        lblHistFrom.Font      = new Font("Segoe UI", 9F);
        lblHistFrom.Location  = new Point(0, 9);
        lblHistFrom.Size      = new Size(42, 20);
        lblHistFrom.TextAlign = ContentAlignment.MiddleLeft;

        dtpHistFrom.Format   = DateTimePickerFormat.Short;
        dtpHistFrom.Value    = DateTime.Today.AddDays(-7);
        dtpHistFrom.Font     = new Font("Segoe UI", 9F);
        dtpHistFrom.Location = new Point(46, 8);
        dtpHistFrom.Size     = new Size(120, 24);

        lblHistTo.Text      = "To:";
        lblHistTo.Font      = new Font("Segoe UI", 9F);
        lblHistTo.Location  = new Point(176, 9);
        lblHistTo.Size      = new Size(28, 20);
        lblHistTo.TextAlign = ContentAlignment.MiddleLeft;

        dtpHistTo.Format   = DateTimePickerFormat.Short;
        dtpHistTo.Value    = DateTime.Today;
        dtpHistTo.Font     = new Font("Segoe UI", 9F);
        dtpHistTo.Location = new Point(208, 8);
        dtpHistTo.Size     = new Size(120, 24);

        btnRefreshHist.Text                       = "Refresh";
        btnRefreshHist.Font                       = new Font("Segoe UI", 9F);
        btnRefreshHist.FlatStyle                  = FlatStyle.Flat;
        btnRefreshHist.FlatAppearance.BorderColor = Color.LightGray;
        btnRefreshHist.BackColor                  = Color.WhiteSmoke;
        btnRefreshHist.Location                   = new Point(338, 6);
        btnRefreshHist.Size                       = new Size(80, 26);
        btnRefreshHist.Click                     += BtnRefreshHist_Click;

        pnlHistFilter.Controls.AddRange(new Control[] {
            lblHistFrom, dtpHistFrom, lblHistTo, dtpHistTo, btnRefreshHist
        });

        dgvAttendance.Dock                        = DockStyle.Fill;
        dgvAttendance.ReadOnly                    = true;
        dgvAttendance.AllowUserToAddRows          = false;
        dgvAttendance.AllowUserToDeleteRows       = false;
        dgvAttendance.AutoSizeColumnsMode         = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAttendance.SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
        dgvAttendance.RowHeadersVisible           = false;
        dgvAttendance.BackgroundColor             = Color.White;
        dgvAttendance.BorderStyle                 = BorderStyle.None;
        dgvAttendance.Font                        = new Font("Segoe UI", 9F);
        dgvAttendance.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvAttendance.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 90, 160);
        dgvAttendance.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvAttendance.EnableHeadersVisualStyles   = false;
        dgvAttendance.RowTemplate.Height          = 26;
        dgvAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

        pnlRight.Controls.Add(dgvAttendance);
        pnlRight.Controls.Add(pnlHistFilter);
        pnlRight.Controls.Add(lblHistoryTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(900, 500);
        Text          = "Attendance Management";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(780, 420);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlRight);
        Controls.Add(pnlLeft);

        ((System.ComponentModel.ISupportInitialize)numHours).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvAttendance).EndInit();
        ResumeLayout(false);
    }
}
