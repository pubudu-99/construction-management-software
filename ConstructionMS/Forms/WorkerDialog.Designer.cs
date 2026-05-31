namespace ConstructionMS.Forms;

partial class WorkerDialog
{
    private System.ComponentModel.IContainer components = null;

    private Label         lblTitle;
    private Label         lblName;
    private TextBox       txtName;
    private Label         lblNic;
    private TextBox       txtNic;
    private Label         lblPhone;
    private TextBox       txtPhone;
    private Label         lblRate;
    private NumericUpDown numRate;
    private CheckBox      chkActive;
    private Button        btnSave;
    private Button        btnCancel;
    private Label         lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle  = new Label();
        lblName   = new Label();
        txtName   = new TextBox();
        lblNic    = new Label();
        txtNic    = new TextBox();
        lblPhone  = new Label();
        txtPhone  = new TextBox();
        lblRate   = new Label();
        numRate   = new NumericUpDown();
        chkActive = new CheckBox();
        btnSave   = new Button();
        btnCancel = new Button();
        lblStatus = new Label();

        ((System.ComponentModel.ISupportInitialize)numRate).BeginInit();
        SuspendLayout();

        const int lx = 24;
        const int fw = 320;
        int y = 20;

        lblTitle.Text      = "Add Worker";
        lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(lx, y);
        lblTitle.Size      = new Size(fw, 28);
        y += 44;

        // Name
        lblName.Text      = "Full Name  *";
        lblName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblName.ForeColor = Color.FromArgb(60, 60, 60);
        lblName.Location  = new Point(lx, y);
        lblName.Size      = new Size(fw, 18);
        y += 20;

        txtName.Font     = new Font("Segoe UI", 9.5F);
        txtName.Location = new Point(lx, y);
        txtName.Size     = new Size(fw, 26);
        y += 38;

        // NIC
        lblNic.Text      = "NIC  (9 digits + V/X, or 12 digits)";
        lblNic.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNic.ForeColor = Color.FromArgb(60, 60, 60);
        lblNic.Location  = new Point(lx, y);
        lblNic.Size      = new Size(fw, 18);
        y += 20;

        txtNic.Font     = new Font("Segoe UI", 9.5F);
        txtNic.Location = new Point(lx, y);
        txtNic.Size     = new Size(fw, 26);
        y += 38;

        // Phone
        lblPhone.Text      = "Phone";
        lblPhone.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPhone.ForeColor = Color.FromArgb(60, 60, 60);
        lblPhone.Location  = new Point(lx, y);
        lblPhone.Size      = new Size(fw, 18);
        y += 20;

        txtPhone.Font     = new Font("Segoe UI", 9.5F);
        txtPhone.Location = new Point(lx, y);
        txtPhone.Size     = new Size(fw, 26);
        y += 38;

        // Hourly rate
        lblRate.Text      = "Hourly Rate (LKR)  *";
        lblRate.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblRate.ForeColor = Color.FromArgb(60, 60, 60);
        lblRate.Location  = new Point(lx, y);
        lblRate.Size      = new Size(fw, 18);
        y += 20;

        numRate.Font          = new Font("Segoe UI", 9.5F);
        numRate.Location      = new Point(lx, y);
        numRate.Size          = new Size(160, 26);
        numRate.DecimalPlaces = 2;
        numRate.Minimum       = 1M;
        numRate.Maximum       = 999999M;
        numRate.Value         = 250M;
        y += 38;

        // Active checkbox
        chkActive.Text     = "Active";
        chkActive.Font     = new Font("Segoe UI", 9.5F);
        chkActive.Location = new Point(lx, y);
        chkActive.Size     = new Size(fw, 24);
        chkActive.Checked  = true;
        y += 38;

        // Buttons
        btnSave.Text                      = "Add Worker";
        btnSave.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSave.BackColor                 = Color.LightGreen;
        btnSave.ForeColor                 = Color.FromArgb(20, 90, 40);
        btnSave.FlatStyle                 = FlatStyle.Flat;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Location                  = new Point(lx, y);
        btnSave.Size                      = new Size(170, 36);
        btnSave.Cursor                    = Cursors.Hand;
        btnSave.Click                    += BtnSave_Click;

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
        lblStatus.Text      = "";
        lblStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.ForeColor = Color.FromArgb(220, 53, 69);
        lblStatus.Location  = new Point(lx, y);
        lblStatus.Size      = new Size(fw, 40);
        lblStatus.Visible   = false;

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode   = AutoScaleMode.Font;
        ClientSize      = new Size(368, y + 56);
        Text            = "Add Worker";
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        AcceptButton    = btnSave;
        CancelButton    = btnCancel;
        Font            = new Font("Segoe UI", 9F);
        BackColor       = Color.White;

        Controls.AddRange(new Control[] {
            lblTitle,
            lblName,  txtName,
            lblNic,   txtNic,
            lblPhone, txtPhone,
            lblRate,  numRate,
            chkActive,
            btnSave,  btnCancel,
            lblStatus
        });

        ((System.ComponentModel.ISupportInitialize)numRate).EndInit();
        ResumeLayout(false);
    }
}
