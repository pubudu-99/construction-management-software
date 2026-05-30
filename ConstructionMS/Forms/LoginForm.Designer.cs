namespace ConstructionMS.Forms;

partial class LoginForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // ── Controls ────────────────────────────────────────────────────────────
    private Label lblTitle;
    private Label lblUsername;
    private TextBox txtUsername;
    private Label lblPassword;
    private TextBox txtPassword;
    private Button btnSignIn;
    private Label lblError;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>
    /// Required method for Designer support – do not modify the contents
    /// of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        lblTitle    = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        btnSignIn   = new Button();
        lblError    = new Label();

        SuspendLayout();

        // ── lblTitle ────────────────────────────────────────────────────────
        lblTitle.Text      = "Construction Management";
        lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(20, 22);
        lblTitle.Size      = new Size(360, 30);
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;

        // ── lblUsername ─────────────────────────────────────────────────────
        lblUsername.Text     = "Username";
        lblUsername.Font     = new Font("Segoe UI", 9F);
        lblUsername.Location = new Point(30, 78);
        lblUsername.Size     = new Size(340, 18);

        // ── txtUsername ─────────────────────────────────────────────────────
        txtUsername.Font     = new Font("Segoe UI", 9.5F);
        txtUsername.Location = new Point(30, 98);
        txtUsername.Size     = new Size(340, 26);
        txtUsername.TabIndex = 0;

        // ── lblPassword ─────────────────────────────────────────────────────
        lblPassword.Text     = "Password";
        lblPassword.Font     = new Font("Segoe UI", 9F);
        lblPassword.Location = new Point(30, 138);
        lblPassword.Size     = new Size(340, 18);

        // ── txtPassword ─────────────────────────────────────────────────────
        txtPassword.Font                  = new Font("Segoe UI", 9.5F);
        txtPassword.Location              = new Point(30, 158);
        txtPassword.Size                  = new Size(340, 26);
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.TabIndex              = 1;

        // ── btnSignIn ───────────────────────────────────────────────────────
        btnSignIn.Text                         = "Sign In";
        btnSignIn.Font                         = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSignIn.BackColor                    = Color.FromArgb(30, 90, 160);
        btnSignIn.ForeColor                    = Color.White;
        btnSignIn.FlatStyle                    = FlatStyle.Flat;
        btnSignIn.FlatAppearance.BorderSize    = 0;
        btnSignIn.Location                     = new Point(145, 205);
        btnSignIn.Size                         = new Size(110, 36);
        btnSignIn.TabIndex                     = 2;
        btnSignIn.Click                       += new EventHandler(BtnSignIn_Click);

        // ── lblError ────────────────────────────────────────────────────────
        lblError.Text      = string.Empty;
        lblError.Font      = new Font("Segoe UI", 8.5F);
        lblError.ForeColor = Color.Crimson;
        lblError.Location  = new Point(30, 252);
        lblError.Size      = new Size(340, 36);
        lblError.TextAlign = ContentAlignment.MiddleCenter;
        lblError.Visible   = false;

        // ── Form ────────────────────────────────────────────────────────────
        AutoScaleMode   = AutoScaleMode.Font;
        ClientSize      = new Size(400, 300);
        Text            = "Construction Management - Sign In";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = Color.White;
        Font            = new Font("Segoe UI", 9F);

        Controls.Add(lblTitle);
        Controls.Add(lblUsername);
        Controls.Add(txtUsername);
        Controls.Add(lblPassword);
        Controls.Add(txtPassword);
        Controls.Add(btnSignIn);
        Controls.Add(lblError);

        ResumeLayout(false);
        PerformLayout();
    }
}
