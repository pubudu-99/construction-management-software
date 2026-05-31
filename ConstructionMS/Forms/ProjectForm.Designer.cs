namespace ConstructionMS.Forms;

partial class ProjectForm
{
    private System.ComponentModel.IContainer components = null;

    private Panel          pnlEntry;
    private Label          lblTitle;
    private Label          lblStatusBanner;
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
    private Panel          pnlDivider;
    private Label          lblLifecycle;
    private Button         btnMarkComplete;
    private Button         btnCreateNew;
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
        lblStatusBanner = new Label();
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
        pnlDivider      = new Panel();
        lblLifecycle    = new Label();
        btnMarkComplete = new Button();
        btnCreateNew    = new Button();
        lblStatus       = new Label();

        ((System.ComponentModel.ISupportInitialize)numBudget).BeginInit();
        SuspendLayout();

        // ── Entry panel ─────────────────────────────────────────────────────────
        pnlEntry.Dock      = DockStyle.Fill;
        pnlEntry.BackColor = Color.White;

        const int lx = 28;
        const int fw = 584;
        int y = 24;

        lblTitle.Text      = "Project Setup";
        lblTitle.Font      = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(lx, y);
        lblTitle.Size      = new Size(fw, 30);
        y += 38;

        // Status banner (wraps so long project names are not clipped)
        lblStatusBanner.Text      = "Current Project: —   |   Status: —";
        lblStatusBanner.Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblStatusBanner.ForeColor = Color.FromArgb(40, 167, 69);
        lblStatusBanner.Location  = new Point(lx, y);
        lblStatusBanner.Size      = new Size(fw, 36);
        y += 46;

        // Project name
        lblProjectName.Text      = "Project Name  *";
        lblProjectName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblProjectName.ForeColor = Color.FromArgb(33, 37, 41);
        lblProjectName.Location  = new Point(lx, y);
        lblProjectName.Size      = new Size(fw, 18);
        y += 22;

        txtProjectName.Font     = new Font("Segoe UI", 9.5F);
        txtProjectName.Location = new Point(lx, y);
        txtProjectName.Size     = new Size(fw, 26);
        y += 40;

        // Budget
        lblBudget.Text      = "Budget (LKR)  *";
        lblBudget.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBudget.ForeColor = Color.FromArgb(33, 37, 41);
        lblBudget.Location  = new Point(lx, y);
        lblBudget.Size      = new Size(fw, 18);
        y += 22;

        numBudget.Font          = new Font("Segoe UI", 9.5F);
        numBudget.Location      = new Point(lx, y);
        numBudget.Size          = new Size(fw, 26);
        numBudget.DecimalPlaces = 2;
        numBudget.Maximum       = 999999999M;
        numBudget.Minimum       = 0M;
        numBudget.ThousandsSeparator = true;
        y += 40;

        // Spent (read-only)
        lblSpentCaption.Text      = "Spent so far  (read-only)";
        lblSpentCaption.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblSpentCaption.ForeColor = Color.FromArgb(33, 37, 41);
        lblSpentCaption.Location  = new Point(lx, y);
        lblSpentCaption.Size      = new Size(fw, 18);
        y += 22;

        lblSpent.Text      = "LKR 0.00";
        lblSpent.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSpent.ForeColor = Color.FromArgb(108, 117, 125);
        lblSpent.Location  = new Point(lx, y);
        lblSpent.Size      = new Size(fw, 22);
        y += 38;

        // Dates — Start and End side by side on one row, each half width.
        const int dGap = 16;
        const int dW   = (fw - dGap) / 2;
        int       dX2  = lx + dW + dGap;

        lblStart.Text      = "Start Date";
        lblStart.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStart.ForeColor = Color.FromArgb(33, 37, 41);
        lblStart.Location  = new Point(lx, y);
        lblStart.Size      = new Size(dW, 18);

        lblEnd.Text      = "End Date";
        lblEnd.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEnd.ForeColor = Color.FromArgb(33, 37, 41);
        lblEnd.Location  = new Point(dX2, y);
        lblEnd.Size      = new Size(dW, 18);
        y += 22;

        dtpStart.Format   = DateTimePickerFormat.Short;
        dtpStart.Font     = new Font("Segoe UI", 9.5F);
        dtpStart.Location = new Point(lx, y);
        dtpStart.Size     = new Size(dW, 26);

        dtpEnd.Format   = DateTimePickerFormat.Short;
        dtpEnd.Font     = new Font("Segoe UI", 9.5F);
        dtpEnd.Location = new Point(dX2, y);
        dtpEnd.Size     = new Size(dW, 26);
        y += 48;

        // Save button (primary action — full width)
        btnSave.Text      = "Save Changes";
        btnSave.Location  = new Point(lx, y);
        btnSave.Size      = new Size(fw, 38);
        btnSave.Click    += BtnSave_Click;
        y += 54;

        // Divider before the lifecycle section
        pnlDivider.BackColor = Color.FromArgb(222, 226, 232);
        pnlDivider.Location  = new Point(lx, y);
        pnlDivider.Size      = new Size(fw, 1);
        y += 16;

        lblLifecycle.Text      = "Project Lifecycle";
        lblLifecycle.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblLifecycle.ForeColor = Color.FromArgb(108, 117, 125);
        lblLifecycle.Location  = new Point(lx, y);
        lblLifecycle.Size      = new Size(fw, 20);
        y += 28;

        // Lifecycle buttons (side by side)
        const int half = (fw - 12) / 2;
        btnMarkComplete.Text      = "Mark Project Complete";
        btnMarkComplete.Location  = new Point(lx, y);
        btnMarkComplete.Size      = new Size(half, 38);
        btnMarkComplete.Click    += BtnMarkComplete_Click;

        btnCreateNew.Text      = "Create New Project";
        btnCreateNew.Location  = new Point(lx + half + 12, y);
        btnCreateNew.Size      = new Size(half, 38);
        btnCreateNew.Click    += BtnCreateNew_Click;
        y += 50;

        // Status label
        lblStatus.Text      = "";
        lblStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.Location  = new Point(lx, y);
        lblStatus.Size      = new Size(fw, 40);
        lblStatus.Visible   = false;

        pnlEntry.Controls.AddRange(new Control[] {
            lblTitle,        lblStatusBanner,
            lblProjectName,  txtProjectName,
            lblBudget,       numBudget,
            lblSpentCaption, lblSpent,
            lblStart,        dtpStart,
            lblEnd,          dtpEnd,
            btnSave,         pnlDivider, lblLifecycle,
            btnMarkComplete, btnCreateNew,
            lblStatus
        });

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode   = AutoScaleMode.Font;
        ClientSize      = new Size(640, 620);
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
