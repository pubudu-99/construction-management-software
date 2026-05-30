namespace ConstructionMS.Forms;

partial class ProjectForm
{
    private System.ComponentModel.IContainer components = null;

    private Panel          pnlEntry;
    private Label          lblTitle;
    private Label          lblProjectName;
    private TextBox        txtProjectName;
    private Label          lblBudget;
    private NumericUpDown  numBudget;
    private Label          lblSpentCaption;
    private Label          lblSpent;
    private Label          lblStart;
    private DateTimePicker dtpStart;
    private Label          lblEnd;
    private DateTimePicker dtpEnd;
    private Button         btnSave;
    private Label          lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlEntry        = new Panel();
        lblTitle        = new Label();
        lblProjectName  = new Label();
        txtProjectName  = new TextBox();
        lblBudget       = new Label();
        numBudget       = new NumericUpDown();
        lblSpentCaption = new Label();
        lblSpent        = new Label();
        lblStart        = new Label();
        dtpStart        = new DateTimePicker();
        lblEnd          = new Label();
        dtpEnd          = new DateTimePicker();
        btnSave         = new Button();
        lblStatus       = new Label();

        ((System.ComponentModel.ISupportInitialize)numBudget).BeginInit();
        SuspendLayout();

        // ── Entry panel ─────────────────────────────────────────────────────────
        pnlEntry.Dock      = DockStyle.Fill;
        pnlEntry.BackColor = Color.White;
        pnlEntry.Padding   = new Padding(24, 20, 24, 20);

        const int lx = 24;
        const int fw = 360;
        int y = 20;

        lblTitle.Text      = "Project Setup";
        lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(lx, y);
        lblTitle.Size      = new Size(fw, 28);
        y += 40;

        // Project name
        lblProjectName.Text      = "Project Name  *";
        lblProjectName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblProjectName.ForeColor = Color.FromArgb(60, 60, 60);
        lblProjectName.Location  = new Point(lx, y);
        lblProjectName.Size      = new Size(fw, 18);
        y += 20;

        txtProjectName.Font     = new Font("Segoe UI", 9.5F);
        txtProjectName.Location = new Point(lx, y);
        txtProjectName.Size     = new Size(fw, 26);
        y += 38;

        // Budget
        lblBudget.Text      = "Budget (LKR)  *";
        lblBudget.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBudget.ForeColor = Color.FromArgb(60, 60, 60);
        lblBudget.Location  = new Point(lx, y);
        lblBudget.Size      = new Size(fw, 18);
        y += 20;

        numBudget.Font          = new Font("Segoe UI", 9.5F);
        numBudget.Location      = new Point(lx, y);
        numBudget.Size          = new Size(fw, 26);
        numBudget.DecimalPlaces = 2;
        numBudget.Maximum       = 999999999M;
        numBudget.Minimum       = 0M;
        numBudget.ThousandsSeparator = true;
        y += 38;

        // Spent (read-only)
        lblSpentCaption.Text      = "Spent so far  (read-only)";
        lblSpentCaption.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSpentCaption.ForeColor = Color.FromArgb(60, 60, 60);
        lblSpentCaption.Location  = new Point(lx, y);
        lblSpentCaption.Size      = new Size(fw, 18);
        y += 20;

        lblSpent.Text      = "LKR 0.00";
        lblSpent.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSpent.ForeColor = Color.FromArgb(120, 120, 120);
        lblSpent.Location  = new Point(lx, y);
        lblSpent.Size      = new Size(fw, 22);
        y += 38;

        // Start date
        lblStart.Text      = "Start Date";
        lblStart.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStart.ForeColor = Color.FromArgb(60, 60, 60);
        lblStart.Location  = new Point(lx, y);
        lblStart.Size      = new Size(fw, 18);
        y += 20;

        dtpStart.Format   = DateTimePickerFormat.Short;
        dtpStart.Font     = new Font("Segoe UI", 9.5F);
        dtpStart.Location = new Point(lx, y);
        dtpStart.Size     = new Size(180, 26);
        y += 38;

        // End date
        lblEnd.Text      = "End Date";
        lblEnd.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEnd.ForeColor = Color.FromArgb(60, 60, 60);
        lblEnd.Location  = new Point(lx, y);
        lblEnd.Size      = new Size(fw, 18);
        y += 20;

        dtpEnd.Format   = DateTimePickerFormat.Short;
        dtpEnd.Font     = new Font("Segoe UI", 9.5F);
        dtpEnd.Location = new Point(lx, y);
        dtpEnd.Size     = new Size(180, 26);
        y += 46;

        // Save button
        btnSave.Text                      = "Save Changes";
        btnSave.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSave.BackColor                 = Color.FromArgb(40, 167, 69);
        btnSave.ForeColor                 = Color.White;
        btnSave.FlatStyle                 = FlatStyle.Flat;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Location                  = new Point(lx, y);
        btnSave.Size                      = new Size(180, 36);
        btnSave.Cursor                    = Cursors.Hand;
        btnSave.Click                    += BtnSave_Click;
        y += 44;

        // Status label
        lblStatus.Text      = "";
        lblStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location  = new Point(lx, y);
        lblStatus.Size      = new Size(fw, 40);
        lblStatus.Visible   = false;

        pnlEntry.Controls.AddRange(new Control[] {
            lblTitle,
            lblProjectName,  txtProjectName,
            lblBudget,       numBudget,
            lblSpentCaption, lblSpent,
            lblStart,        dtpStart,
            lblEnd,          dtpEnd,
            btnSave,         lblStatus
        });

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode   = AutoScaleMode.Font;
        ClientSize      = new Size(420, 520);
        Text            = "Project Setup";
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        Font            = new Font("Segoe UI", 9F);
        BackColor       = Color.White;

        Controls.Add(pnlEntry);

        ((System.ComponentModel.ISupportInitialize)numBudget).EndInit();
        ResumeLayout(false);
    }
}
