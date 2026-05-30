namespace ConstructionMS.Forms;

partial class TaskForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Entry panel ───────────────────────────────────────────────────────────
    private Panel          pnlEntry;
    private Label          lblFormTitle;
    private Label          lblTaskName;
    private TextBox        txtTaskName;
    private Label          lblStartDate;
    private DateTimePicker dtpStartDate;
    private Label          lblEndDate;
    private DateTimePicker dtpEndDate;
    private Label          lblAssignee;
    private ComboBox       cmbAssignee;
    private Button         btnSaveTask;
    private Label          lblTaskStatus;

    // ── Grid panel ────────────────────────────────────────────────────────────
    private Panel           pnlGrid;
    private Label           lblGridTitle;
    private DataGridView    dgvTasks;
    private ContextMenuStrip ctxMenu;
    private ToolStripMenuItem menuMarkComplete;
    private ToolStripMenuItem menuDelete;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlEntry         = new Panel();
        lblFormTitle     = new Label();
        lblTaskName      = new Label();
        txtTaskName      = new TextBox();
        lblStartDate     = new Label();
        dtpStartDate     = new DateTimePicker();
        lblEndDate       = new Label();
        dtpEndDate       = new DateTimePicker();
        lblAssignee      = new Label();
        cmbAssignee      = new ComboBox();
        btnSaveTask      = new Button();
        lblTaskStatus    = new Label();
        pnlGrid          = new Panel();
        lblGridTitle     = new Label();
        dgvTasks         = new DataGridView();
        ctxMenu          = new ContextMenuStrip();
        menuMarkComplete = new ToolStripMenuItem();
        menuDelete       = new ToolStripMenuItem();

        ((System.ComponentModel.ISupportInitialize)dgvTasks).BeginInit();
        SuspendLayout();

        // ── Entry panel ───────────────────────────────────────────────────────
        pnlEntry.Dock      = DockStyle.Top;
        pnlEntry.Height    = 230;
        pnlEntry.BackColor = Color.FromArgb(248, 249, 250);
        pnlEntry.Padding   = new Padding(14, 10, 14, 8);

        const int lx = 14, fw = 420;
        int y = 10;

        lblFormTitle.Text      = "Add New Task";
        lblFormTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFormTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblFormTitle.Location  = new Point(lx, y);
        lblFormTitle.Size      = new Size(fw, 24);
        y += 30;

        lblTaskName.Text      = "Task Name  *";
        lblTaskName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTaskName.ForeColor = Color.FromArgb(60, 60, 60);
        lblTaskName.Location  = new Point(lx, y);
        lblTaskName.Size      = new Size(fw, 18);
        y += 20;

        txtTaskName.Font     = new Font("Segoe UI", 9.5F);
        txtTaskName.Location = new Point(lx, y);
        txtTaskName.Size     = new Size(fw, 26);
        y += 32;

        // Start + End dates side by side
        lblStartDate.Text      = "Start Date";
        lblStartDate.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStartDate.ForeColor = Color.FromArgb(60, 60, 60);
        lblStartDate.Location  = new Point(lx, y);
        lblStartDate.Size      = new Size(200, 18);

        lblEndDate.Text      = "End Date";
        lblEndDate.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEndDate.ForeColor = Color.FromArgb(60, 60, 60);
        lblEndDate.Location  = new Point(lx + 220, y);
        lblEndDate.Size      = new Size(200, 18);
        y += 20;

        dtpStartDate.Format   = DateTimePickerFormat.Short;
        dtpStartDate.Value    = DateTime.Today;
        dtpStartDate.Font     = new Font("Segoe UI", 9.5F);
        dtpStartDate.Location = new Point(lx, y);
        dtpStartDate.Size     = new Size(200, 26);

        dtpEndDate.Format   = DateTimePickerFormat.Short;
        dtpEndDate.Value    = DateTime.Today.AddDays(7);
        dtpEndDate.Font     = new Font("Segoe UI", 9.5F);
        dtpEndDate.Location = new Point(lx + 220, y);
        dtpEndDate.Size     = new Size(200, 26);
        y += 32;

        lblAssignee.Text      = "Assign To (optional)";
        lblAssignee.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAssignee.ForeColor = Color.FromArgb(60, 60, 60);
        lblAssignee.Location  = new Point(lx, y);
        lblAssignee.Size      = new Size(fw, 18);
        y += 20;

        cmbAssignee.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAssignee.Font          = new Font("Segoe UI", 9.5F);
        cmbAssignee.Location      = new Point(lx, y);
        cmbAssignee.Size          = new Size(fw, 26);
        y += 32;

        btnSaveTask.Text                      = "Add Task";
        btnSaveTask.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveTask.BackColor                 = Color.FromArgb(40, 167, 69);
        btnSaveTask.ForeColor                 = Color.White;
        btnSaveTask.FlatStyle                 = FlatStyle.Flat;
        btnSaveTask.FlatAppearance.BorderSize = 0;
        btnSaveTask.Location                  = new Point(lx, y);
        btnSaveTask.Size                      = new Size(140, 32);
        btnSaveTask.Cursor                    = Cursors.Hand;
        btnSaveTask.Click                    += BtnSaveTask_Click;

        lblTaskStatus.Text      = "";
        lblTaskStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTaskStatus.Location  = new Point(lx + 150, y + 6);
        lblTaskStatus.Size      = new Size(fw - 150, 22);
        lblTaskStatus.Visible   = false;

        pnlEntry.Controls.AddRange(new Control[] {
            lblFormTitle,
            lblTaskName, txtTaskName,
            lblStartDate, dtpStartDate,
            lblEndDate,   dtpEndDate,
            lblAssignee,  cmbAssignee,
            btnSaveTask,  lblTaskStatus
        });

        // ── Grid panel ────────────────────────────────────────────────────────
        pnlGrid.Dock      = DockStyle.Fill;
        pnlGrid.BackColor = Color.White;
        pnlGrid.Padding   = new Padding(14, 8, 14, 14);

        lblGridTitle.Text      = "Task List";
        lblGridTitle.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblGridTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblGridTitle.Dock      = DockStyle.Top;
        lblGridTitle.Height    = 26;

        // Context menu
        menuMarkComplete.Text   = "Mark as Complete";
        menuMarkComplete.Click += MenuMarkComplete_Click;
        menuDelete.Text         = "Delete";
        menuDelete.Click       += MenuDelete_Click;
        ctxMenu.Items.AddRange(new ToolStripItem[] { menuMarkComplete, menuDelete });

        dgvTasks.Dock                        = DockStyle.Fill;
        dgvTasks.ReadOnly                    = true;
        dgvTasks.AllowUserToAddRows          = false;
        dgvTasks.AllowUserToDeleteRows       = false;
        dgvTasks.AutoSizeColumnsMode         = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTasks.SelectionMode               = DataGridViewSelectionMode.FullRowSelect;
        dgvTasks.RowHeadersVisible           = false;
        dgvTasks.BackgroundColor             = Color.White;
        dgvTasks.BorderStyle                 = BorderStyle.None;
        dgvTasks.Font                        = new Font("Segoe UI", 9F);
        dgvTasks.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvTasks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 90, 160);
        dgvTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvTasks.EnableHeadersVisualStyles   = false;
        dgvTasks.RowTemplate.Height          = 26;
        dgvTasks.ContextMenuStrip            = ctxMenu;
        dgvTasks.CellFormatting             += DgvTasks_CellFormatting;

        pnlGrid.Controls.Add(dgvTasks);
        pnlGrid.Controls.Add(lblGridTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(860, 580);
        Text          = "Task Scheduling";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(700, 480);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlGrid);
        Controls.Add(pnlEntry);

        ((System.ComponentModel.ISupportInitialize)dgvTasks).EndInit();
        ResumeLayout(false);
    }
}
