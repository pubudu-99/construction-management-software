namespace ConstructionMS.Forms;

partial class NewProjectDialog
{
    private System.ComponentModel.IContainer components = null;

    private Label          lblTitle;
    private Label          lblName;
    private TextBox        txtNewProjName;
    private Label          lblBudget;
    private NumericUpDown  numNewBudget;
    private Label          lblStart;
    private DateTimePicker dtpNewStart;
    private Label          lblEnd;
    private DateTimePicker dtpNewEnd;
    private Button         btnCreate;
    private Button         btnCancel;
    private Label          lblNewStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle       = new Label();
        lblName        = new Label();
        txtNewProjName = new TextBox();
        lblBudget      = new Label();
        numNewBudget   = new NumericUpDown();
        lblStart       = new Label();
        dtpNewStart    = new DateTimePicker();
        lblEnd         = new Label();
        dtpNewEnd      = new DateTimePicker();
        btnCreate      = new Button();
        btnCancel      = new Button();
        lblNewStatus   = new Label();

        ((System.ComponentModel.ISupportInitialize)numNewBudget).BeginInit();
        SuspendLayout();

        const int lx = 24;
        const int fw = 320;
        int y = 20;

        lblTitle.Text      = "Create New Project";
        lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(lx, y);
        lblTitle.Size      = new Size(fw, 28);
        y += 44;

        // Name
        lblName.Text      = "New Project Name  *";
        lblName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblName.ForeColor = Color.FromArgb(60, 60, 60);
        lblName.Location  = new Point(lx, y);
        lblName.Size      = new Size(fw, 18);
        y += 20;

        txtNewProjName.Font     = new Font("Segoe UI", 9.5F);
        txtNewProjName.Location = new Point(lx, y);
        txtNewProjName.Size     = new Size(fw, 26);
        y += 38;

        // Budget
        lblBudget.Text      = "Budget (LKR)  *";
        lblBudget.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBudget.ForeColor = Color.FromArgb(60, 60, 60);
        lblBudget.Location  = new Point(lx, y);
        lblBudget.Size      = new Size(fw, 18);
        y += 20;

        numNewBudget.Font              = new Font("Segoe UI", 9.5F);
        numNewBudget.Location          = new Point(lx, y);
        numNewBudget.Size              = new Size(fw, 26);
        numNewBudget.DecimalPlaces     = 2;
        numNewBudget.Minimum           = 1M;
        numNewBudget.Maximum           = 999999999M;
        numNewBudget.ThousandsSeparator = true;
        y += 38;

        // Start date
        lblStart.Text      = "Start Date";
        lblStart.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStart.ForeColor = Color.FromArgb(60, 60, 60);
        lblStart.Location  = new Point(lx, y);
        lblStart.Size      = new Size(fw, 18);
        y += 20;

        dtpNewStart.Format   = DateTimePickerFormat.Short;
        dtpNewStart.Font     = new Font("Segoe UI", 9.5F);
        dtpNewStart.Location = new Point(lx, y);
        dtpNewStart.Size     = new Size(160, 26);
        y += 38;

        // End date
        lblEnd.Text      = "End Date";
        lblEnd.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEnd.ForeColor = Color.FromArgb(60, 60, 60);
        lblEnd.Location  = new Point(lx, y);
        lblEnd.Size      = new Size(fw, 18);
        y += 20;

        dtpNewEnd.Format   = DateTimePickerFormat.Short;
        dtpNewEnd.Font     = new Font("Segoe UI", 9.5F);
        dtpNewEnd.Location = new Point(lx, y);
        dtpNewEnd.Size     = new Size(160, 26);
        y += 46;

        // Buttons
        btnCreate.Text                      = "Create Project";
        btnCreate.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnCreate.BackColor                 = Color.LightGreen;
        btnCreate.ForeColor                 = Color.FromArgb(20, 90, 40);
        btnCreate.FlatStyle                 = FlatStyle.Flat;
        btnCreate.FlatAppearance.BorderSize = 0;
        btnCreate.Location                  = new Point(lx, y);
        btnCreate.Size                      = new Size(170, 36);
        btnCreate.Cursor                    = Cursors.Hand;
        btnCreate.Click                    += BtnCreate_Click;

        btnCancel.Text                      = "Cancel";
        btnCancel.Font                      = new Font("Segoe UI", 9.5F);
        btnCancel.BackColor                 = Color.FromArgb(240, 240, 240);
        btnCancel.ForeColor                 = Color.FromArgb(60, 60, 60);
        btnCancel.FlatStyle                 = FlatStyle.Flat;
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.Location                  = new Point(lx + 180, y);
        btnCancel.Size                      = new Size(140, 36);
        btnCancel.Cursor                    = Cursors.Hand;
        btnCancel.Click                    += BtnCancel_Click;
        y += 46;

        // Inline status (error) label
        lblNewStatus.Text      = "";
        lblNewStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNewStatus.ForeColor = Color.FromArgb(220, 53, 69);
        lblNewStatus.Location  = new Point(lx, y);
        lblNewStatus.Size      = new Size(fw, 40);
        lblNewStatus.Visible   = false;

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode   = AutoScaleMode.Font;
        ClientSize      = new Size(368, y + 56);
        Text            = "Create New Project";
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        AcceptButton    = btnCreate;
        CancelButton    = btnCancel;
        Font            = new Font("Segoe UI", 9F);
        BackColor       = Color.White;

        Controls.AddRange(new Control[] {
            lblTitle,
            lblName,   txtNewProjName,
            lblBudget, numNewBudget,
            lblStart,  dtpNewStart,
            lblEnd,    dtpNewEnd,
            btnCreate, btnCancel,
            lblNewStatus
        });

        ((System.ComponentModel.ISupportInitialize)numNewBudget).EndInit();
        ResumeLayout(false);
    }
}
