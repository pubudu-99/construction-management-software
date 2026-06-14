namespace ConstructionMS.Forms;

partial class ChangePasswordForm
{
    private System.ComponentModel.IContainer components = null;

    private Label    lblTitle;
    private Label    lblSubtitle;
    private Label    lblCurrent;
    private TextBox  txtCurrent;
    private Label    lblNew;
    private TextBox  txtNew;
    private Label    lblConfirm;
    private TextBox  txtConfirm;
    private Label    lblStrength;
    private Button   btnSave;
    private Button   btnCancel;
    private Label    lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle    = new Label();
        lblSubtitle = new Label();
        lblCurrent  = new Label();
        txtCurrent  = new TextBox();
        lblNew      = new Label();
        txtNew      = new TextBox();
        lblConfirm  = new Label();
        txtConfirm  = new TextBox();
        lblStrength = new Label();
        btnSave     = new Button();
        btnCancel   = new Button();
        lblStatus   = new Label();

        SuspendLayout();

        const int lx = 30;
        const int fw = 320;
        int y = 24;

        // ── Title ─────────────────────────────────────────────────────────────
        lblTitle.Text      = "Change Password";
        lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(lx, y);
        lblTitle.Size      = new Size(fw, 28);
        y += 32;

        // Text is set by the constructors (from Session or the forced-login user)
        // so the Designer never reads session state.
        lblSubtitle.Text      = "";
        lblSubtitle.Font      = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblSubtitle.ForeColor = Color.Gray;
        lblSubtitle.Location  = new Point(lx, y);
        lblSubtitle.Size      = new Size(fw, 18);
        y += 28;

        // ── Current password ──────────────────────────────────────────────────
        lblCurrent.Text      = "Current Password";
        lblCurrent.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCurrent.ForeColor = Color.FromArgb(60, 60, 60);
        lblCurrent.Location  = new Point(lx, y);
        lblCurrent.Size      = new Size(fw, 18);
        y += 20;

        txtCurrent.UseSystemPasswordChar = true;
        txtCurrent.Font                  = new Font("Segoe UI", 9.5F);
        txtCurrent.Location              = new Point(lx, y);
        txtCurrent.Size                  = new Size(fw, 26);
        txtCurrent.TabIndex              = 0;
        y += 34;

        // ── New password ──────────────────────────────────────────────────────
        lblNew.Text      = "New Password";
        lblNew.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNew.ForeColor = Color.FromArgb(60, 60, 60);
        lblNew.Location  = new Point(lx, y);
        lblNew.Size      = new Size(fw, 18);
        y += 20;

        txtNew.UseSystemPasswordChar = true;
        txtNew.Font                  = new Font("Segoe UI", 9.5F);
        txtNew.Location              = new Point(lx, y);
        txtNew.Size                  = new Size(fw, 26);
        txtNew.TabIndex              = 1;
        txtNew.TextChanged          += TxtNew_TextChanged;
        y += 32;

        // Strength indicator
        lblStrength.Text      = "";
        lblStrength.Font      = new Font("Segoe UI", 8F, FontStyle.Bold);
        lblStrength.Location  = new Point(lx, y);
        lblStrength.Size      = new Size(fw, 16);
        y += 22;

        // ── Confirm password ──────────────────────────────────────────────────
        lblConfirm.Text      = "Confirm New Password";
        lblConfirm.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblConfirm.ForeColor = Color.FromArgb(60, 60, 60);
        lblConfirm.Location  = new Point(lx, y);
        lblConfirm.Size      = new Size(fw, 18);
        y += 20;

        txtConfirm.UseSystemPasswordChar = true;
        txtConfirm.Font                  = new Font("Segoe UI", 9.5F);
        txtConfirm.Location              = new Point(lx, y);
        txtConfirm.Size                  = new Size(fw, 26);
        txtConfirm.TabIndex              = 2;
        y += 38;

        // ── Status label ──────────────────────────────────────────────────────
        lblStatus.Text      = "";
        lblStatus.Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblStatus.Location  = new Point(lx, y);
        lblStatus.Size      = new Size(fw, 36);
        lblStatus.Visible   = false;
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        y += 42;

        // ── Buttons ───────────────────────────────────────────────────────────
        btnSave.Text                      = "Save New Password";
        btnSave.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSave.BackColor                 = Color.FromArgb(30, 90, 160);
        btnSave.ForeColor                 = Color.White;
        btnSave.FlatStyle                 = FlatStyle.Flat;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.Location                  = new Point(lx, y);
        btnSave.Size                      = new Size(fw, 36);
        btnSave.TabIndex                  = 3;
        btnSave.Cursor                    = Cursors.Hand;
        btnSave.Click                    += BtnSave_Click;
        y += 44;

        btnCancel.Text                         = "Cancel";
        btnCancel.Font                         = new Font("Segoe UI", 9F);
        btnCancel.FlatStyle                    = FlatStyle.Flat;
        btnCancel.FlatAppearance.BorderColor   = Color.LightGray;
        btnCancel.BackColor                    = Color.WhiteSmoke;
        btnCancel.Location                     = new Point(lx, y);
        btnCancel.Size                         = new Size(fw, 30);
        btnCancel.TabIndex                     = 4;
        btnCancel.Cursor                       = Cursors.Hand;
        btnCancel.Click                       += (_, _) => Close();

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode   = AutoScaleMode.Font;
        ClientSize      = new Size(382, y + 50);
        Text            = "Change Password";
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        MinimizeBox     = false;
        Font            = new Font("Segoe UI", 9F);
        BackColor       = Color.White;

        Controls.AddRange(new Control[] {
            lblTitle, lblSubtitle,
            lblCurrent, txtCurrent,
            lblNew,     txtNew,     lblStrength,
            lblConfirm, txtConfirm,
            lblStatus,
            btnSave,    btnCancel
        });

        ResumeLayout(false);
    }
}
