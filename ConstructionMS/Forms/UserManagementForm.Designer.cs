namespace ConstructionMS.Forms;

partial class UserManagementForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Entry panel ───────────────────────────────────────────────────────────
    private Panel    pnlEntry;
    private Label    lblEntryTitle;
    private Label    lblNewUsername;
    private TextBox  txtNewUsername;
    private Label    lblNewPassword;
    private TextBox  txtNewPassword;
    private Label    lblNewFullName;
    private TextBox  txtNewFullName;
    private Label    lblNewRole;
    private ComboBox cmbNewRole;
    private Button   btnCreateUser;
    private Button   btnCancelEdit;
    private Label    lblUserStatus;

    // ── Grid panel ────────────────────────────────────────────────────────────
    private Panel             pnlGrid;
    private Label             lblGridTitle;
    private DataGridView      dgvUsers;
    private ContextMenuStrip  ctxUserMenu;
    private ToolStripMenuItem mnuToggleActive;
    private ToolStripMenuItem mnuResetPassword;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlEntry        = new Panel();
        lblEntryTitle   = new Label();
        lblNewUsername  = new Label();
        txtNewUsername  = new TextBox();
        lblNewPassword  = new Label();
        txtNewPassword  = new TextBox();
        lblNewFullName  = new Label();
        txtNewFullName  = new TextBox();
        lblNewRole      = new Label();
        cmbNewRole      = new ComboBox();
        btnCreateUser   = new Button();
        btnCancelEdit   = new Button();
        lblUserStatus   = new Label();

        pnlGrid         = new Panel();
        lblGridTitle    = new Label();
        dgvUsers         = new DataGridView();
        ctxUserMenu      = new ContextMenuStrip();
        mnuToggleActive  = new ToolStripMenuItem();
        mnuResetPassword = new ToolStripMenuItem();

        ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // ENTRY PANEL — Add / Edit User
        // ══════════════════════════════════════════════════════════════════════
        pnlEntry.Dock      = DockStyle.Left;
        pnlEntry.Width     = 340;
        pnlEntry.BackColor = Color.FromArgb(248, 249, 250);
        pnlEntry.Padding   = new Padding(16, 12, 16, 12);

        const int lx = 16;
        const int fw = 296;
        int y = 14;

        lblEntryTitle.Text      = "Add New User";
        lblEntryTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblEntryTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblEntryTitle.Location  = new Point(lx, y);
        lblEntryTitle.Size      = new Size(fw, 24);
        y += 32;

        // ── Username ───────────────────────────────────────────────────────────
        lblNewUsername.Text      = "Username  *";
        lblNewUsername.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNewUsername.ForeColor = Color.FromArgb(60, 60, 60);
        lblNewUsername.Location  = new Point(lx, y);
        lblNewUsername.Size      = new Size(fw, 18);
        y += 20;

        txtNewUsername.Font     = new Font("Segoe UI", 9.5F);
        txtNewUsername.Location = new Point(lx, y);
        txtNewUsername.Size     = new Size(fw, 26);
        y += 34;

        // ── Password ───────────────────────────────────────────────────────────
        lblNewPassword.Text      = "Password  *";
        lblNewPassword.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNewPassword.ForeColor = Color.FromArgb(60, 60, 60);
        lblNewPassword.Location  = new Point(lx, y);
        lblNewPassword.Size      = new Size(fw, 18);
        y += 20;

        txtNewPassword.Font     = new Font("Segoe UI", 9.5F);
        txtNewPassword.Location = new Point(lx, y);
        txtNewPassword.Size     = new Size(fw, 26);
        txtNewPassword.UseSystemPasswordChar = true;
        y += 34;

        // ── Full Name ──────────────────────────────────────────────────────────
        lblNewFullName.Text      = "Full Name  *";
        lblNewFullName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNewFullName.ForeColor = Color.FromArgb(60, 60, 60);
        lblNewFullName.Location  = new Point(lx, y);
        lblNewFullName.Size      = new Size(fw, 18);
        y += 20;

        txtNewFullName.Font     = new Font("Segoe UI", 9.5F);
        txtNewFullName.Location = new Point(lx, y);
        txtNewFullName.Size     = new Size(fw, 26);
        y += 34;

        // ── Role ───────────────────────────────────────────────────────────────
        lblNewRole.Text      = "Role  *";
        lblNewRole.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNewRole.ForeColor = Color.FromArgb(60, 60, 60);
        lblNewRole.Location  = new Point(lx, y);
        lblNewRole.Size      = new Size(fw, 18);
        y += 20;

        cmbNewRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbNewRole.Items.AddRange(new object[] { "Manager", "Supervisor" });
        cmbNewRole.SelectedIndex = 1;
        cmbNewRole.Font     = new Font("Segoe UI", 9.5F);
        cmbNewRole.Location = new Point(lx, y);
        cmbNewRole.Size     = new Size(fw, 26);
        y += 42;

        // ── Buttons ────────────────────────────────────────────────────────────
        btnCreateUser.Text                      = "Create User";
        btnCreateUser.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnCreateUser.BackColor                 = Color.FromArgb(40, 167, 69);
        btnCreateUser.ForeColor                 = Color.White;
        btnCreateUser.FlatStyle                 = FlatStyle.Flat;
        btnCreateUser.FlatAppearance.BorderSize = 0;
        btnCreateUser.Location                  = new Point(lx, y);
        btnCreateUser.Size                      = new Size(180, 34);
        btnCreateUser.Cursor                    = Cursors.Hand;
        btnCreateUser.Click                    += BtnCreateUser_Click;

        btnCancelEdit.Text                      = "Cancel";
        btnCancelEdit.Font                      = new Font("Segoe UI", 9.5F);
        btnCancelEdit.FlatStyle                 = FlatStyle.Flat;
        btnCancelEdit.FlatAppearance.BorderColor = Color.LightGray;
        btnCancelEdit.BackColor                 = Color.White;
        btnCancelEdit.Location                  = new Point(lx + 190, y);
        btnCancelEdit.Size                      = new Size(106, 34);
        btnCancelEdit.Cursor                    = Cursors.Hand;
        btnCancelEdit.Visible                   = false;
        btnCancelEdit.Click                    += BtnCancelEdit_Click;
        y += 42;

        lblUserStatus.Text      = "";
        lblUserStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUserStatus.Location  = new Point(lx, y);
        lblUserStatus.Size      = new Size(fw, 22);
        lblUserStatus.Visible   = false;

        pnlEntry.Controls.AddRange(new Control[] {
            lblEntryTitle,
            lblNewUsername, txtNewUsername,
            lblNewPassword, txtNewPassword,
            lblNewFullName, txtNewFullName,
            lblNewRole,     cmbNewRole,
            btnCreateUser,  btnCancelEdit,
            lblUserStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // GRID PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlGrid.Dock      = DockStyle.Fill;
        pnlGrid.BackColor = Color.White;
        pnlGrid.Padding   = new Padding(12, 12, 12, 12);

        lblGridTitle.Text      = "Existing Users";
        lblGridTitle.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblGridTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblGridTitle.Dock      = DockStyle.Top;
        lblGridTitle.Height    = 24;

        dgvUsers.Dock = DockStyle.Fill;
        dgvUsers.CellDoubleClick += DgvUsers_CellDoubleClick;
        dgvUsers.CellFormatting  += DgvUsers_CellFormatting;

        // Context menu
        mnuToggleActive.Text   = "Toggle Active";
        mnuToggleActive.Click += MnuToggleActive_Click;

        mnuResetPassword.Text   = "Reset Password...";
        mnuResetPassword.Click += MnuResetPassword_Click;

        ctxUserMenu.Items.AddRange(new ToolStripItem[] { mnuToggleActive, mnuResetPassword });
        ctxUserMenu.Opening   += CtxUserMenu_Opening;
        dgvUsers.ContextMenuStrip = ctxUserMenu;

        pnlGrid.Controls.Add(dgvUsers);
        pnlGrid.Controls.Add(lblGridTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(1000, 580);
        Text          = "User Management";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(880, 520);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlGrid);
        Controls.Add(pnlEntry);

        ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
        ResumeLayout(false);
    }
}
