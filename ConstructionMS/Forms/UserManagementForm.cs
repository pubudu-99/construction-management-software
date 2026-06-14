using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR1.1 — User Registration / FR1.3 — Role-Based Access.
/// Managers only. Allows creating new users, editing display details,
/// and toggling active status. Username and password cannot be changed
/// from this form once the user exists.
/// </summary>
public partial class UserManagementForm : Form
{
    private readonly UserService _service;

    /// <summary>0 means "Add new"; > 0 means editing that user.</summary>
    private int _editingUserId;

    /// <summary>Initialises the form with database access.</summary>
    public UserManagementForm(DbConnectionFactory factory)
    {
        _service = new UserService(factory, new UserRepository(factory));
        InitializeComponent();
        GridStyle.Apply(dgvUsers);
        Theme.Apply(this);
        Load += UserManagementForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Enforces Manager-only access then loads the user list.</summary>
    private void UserManagementForm_Load(object? sender, EventArgs e)
    {
        if (!Session.IsManager)
        {
            MessageBox.Show(
                "Access denied. Only Managers can open User Management.",
                "Access Denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            BeginInvoke((Action)Close);
            return;
        }

        LoadUsers();
    }

    // ── Data loading ──────────────────────────────────────────────────────────

    /// <summary>Loads all users from the database into the grid.</summary>
    private void LoadUsers()
    {
        var rows = _service.GetAll()
            .Select(u => new
            {
                u.UserId,
                u.Username,
                FullName = u.FullName,
                u.Role,
                Active   = u.IsActive ? "Yes" : "No"
            })
            .ToList();

        dgvUsers.DataSource = rows;

        if (dgvUsers.Columns.Count == 0) return;

        SetColVisible("UserId",  false);
        SetColHeader("Username", "Username");
        SetColHeader("FullName", "Full Name");
        SetColHeader("Role",     "Role");
        SetColHeader("Active",   "Active");

        SetColWeight("Username", 160);
        SetColWeight("FullName", 240);
        SetColWeight("Role",     130);
        SetColWeight("Active",   80);
    }

    /// <summary>Greys out inactive users in the grid.</summary>
    private void DgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvUsers.Rows[e.RowIndex].DataBoundItem is null) return;
        if (e.CellStyle is not { } cs) return;

        var active = dgvUsers.Rows[e.RowIndex].Cells["Active"]?.Value?.ToString();
        if (active == "No")
        {
            cs.ForeColor          = Color.FromArgb(140, 140, 140);
            cs.SelectionForeColor = Color.FromArgb(70, 70, 70);
        }
    }

    // ── Entry panel events ────────────────────────────────────────────────────

    /// <summary>Creates a new user, or updates the user currently being edited.</summary>
    private void BtnCreateUser_Click(object sender, EventArgs e)
    {
        if (_editingUserId == 0)
        {
            // ── Create mode ──
            var result = _service.CreateUser(
                username:  txtNewUsername.Text.Trim(),
                password:  txtNewPassword.Text,
                fullName:  txtNewFullName.Text.Trim(),
                role:      cmbNewRole.SelectedItem?.ToString() ?? "Supervisor");

            if (!result.Success)
            {
                ShowStatus(result.Message, success: false);
                return;
            }

            ShowStatus("User created successfully.", success: true);
            ClearEntry();
            LoadUsers();
        }
        else
        {
            // ── Edit mode (FullName + Role + active status only) ──
            var existing = _service.GetAll().FirstOrDefault(x => x.UserId == _editingUserId);
            if (existing is null)
            {
                ShowStatus("User no longer exists.", success: false);
                ClearEntry();
                LoadUsers();
                return;
            }

            existing.FullName = txtNewFullName.Text.Trim();
            existing.Role     = cmbNewRole.SelectedItem?.ToString() ?? existing.Role;

            var result = _service.UpdateUser(existing);
            if (!result.Success)
            {
                ShowStatus(result.Message, success: false);
                return;
            }

            ShowStatus("User updated successfully.", success: true);
            ClearEntry();
            LoadUsers();
        }
    }

    /// <summary>Cancels edit mode and returns to "Add New User" state.</summary>
    private void BtnCancelEdit_Click(object sender, EventArgs e) => ClearEntry();

    // ── Grid events ───────────────────────────────────────────────────────────

    /// <summary>Switches the form into edit mode when a row is double-clicked.</summary>
    private void DgvUsers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvUsers.Rows[e.RowIndex].Cells["UserId"]?.Value is not int id) return;

        var user = _service.GetAll().FirstOrDefault(u => u.UserId == id);
        if (user is null) return;

        _editingUserId = user.UserId;

        // Populate the editable fields.
        txtNewFullName.Text = user.FullName;
        for (int i = 0; i < cmbNewRole.Items.Count; i++)
        {
            if (cmbNewRole.Items[i]?.ToString() == user.Role)
            {
                cmbNewRole.SelectedIndex = i;
                break;
            }
        }

        // Hide username and password fields — they cannot be changed here.
        lblNewUsername.Visible = false;
        txtNewUsername.Visible = false;
        lblNewPassword.Visible = false;
        txtNewPassword.Visible = false;

        lblEntryTitle.Text   = $"Edit User: {user.Username}";
        btnCreateUser.Text   = "Save Changes";
        btnCancelEdit.Visible = true;
        txtNewFullName.Focus();
    }

    /// <summary>
    /// Updates the context menu's Toggle Active label based on the row state.
    /// </summary>
    private void CtxUserMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (dgvUsers.CurrentRow?.Cells["Active"]?.Value is not string active)
        {
            e.Cancel = true;
            return;
        }
        mnuToggleActive.Text = active == "Yes" ? "Deactivate User" : "Activate User";
    }

    /// <summary>Toggles the IsActive flag on the selected user.</summary>
    private void MnuToggleActive_Click(object sender, EventArgs e)
    {
        if (dgvUsers.CurrentRow?.Cells["UserId"]?.Value is not int id) return;
        if (dgvUsers.CurrentRow.Cells["Active"]?.Value is not string active) return;

        // Safety: prevent deactivating yourself.
        if (Session.Current is not null && Session.Current.UserId == id && active == "Yes")
        {
            MessageBox.Show(
                "You cannot deactivate your own account while signed in.",
                "Action Blocked",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        bool makeActive = active == "No";
        var result = _service.SetActive(id, makeActive);
        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        LoadUsers();
    }

    /// <summary>
    /// Resets the selected user's password to a new temporary value and
    /// forces them to change it at next login. The Manager never sees the
    /// user's own password — this only issues a fresh temporary one.
    /// </summary>
    private void MnuResetPassword_Click(object sender, EventArgs e)
    {
        if (dgvUsers.CurrentRow?.Cells["UserId"]?.Value is not int id) return;
        string username = dgvUsers.CurrentRow.Cells["Username"]?.Value?.ToString() ?? "user";

        string? temp = PromptTempPassword(username);
        if (temp is null) return;   // cancelled

        var result = _service.ResetPassword(id, temp);
        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Reset Password",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(
            $"Temporary password set for '{username}'.\n\n" +
            $"Give them this password:\n    {temp}\n\n" +
            "They will be required to change it at next login.",
            "Password Reset",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        ShowStatus($"Password reset for {username}.", success: true);
    }

    /// <summary>
    /// Shows a small modal dialog asking for a new temporary password.
    /// Returns the entered value, or <c>null</c> if the Manager cancelled.
    /// </summary>
    private string? PromptTempPassword(string username)
    {
        using var dlg = new Form
        {
            Text            = "Reset Password",
            ClientSize      = new Size(340, 150),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition   = FormStartPosition.CenterParent,
            MaximizeBox     = false,
            MinimizeBox     = false,
            Font            = new Font("Segoe UI", 9F)
        };

        var lbl = new Label
        {
            Text     = $"New temporary password for '{username}':",
            Location = new Point(16, 16),
            Size     = new Size(308, 18)
        };

        var txt = new TextBox
        {
            Location        = new Point(16, 40),
            Size            = new Size(308, 26),
            Font            = new Font("Segoe UI", 9.5F),
            PlaceholderText = "At least 6 characters"
        };

        var btnOk = new Button
        {
            Text         = "Reset",
            BackColor    = Color.FromArgb(40, 167, 69),
            ForeColor    = Color.White,
            FlatStyle    = FlatStyle.Flat,
            Location     = new Point(16, 100),
            Size         = new Size(120, 32),
            DialogResult = DialogResult.OK,
            Cursor       = Cursors.Hand
        };
        btnOk.FlatAppearance.BorderSize = 0;

        var btnCancel = new Button
        {
            Text         = "Cancel",
            Location     = new Point(150, 100),
            Size         = new Size(90, 32),
            DialogResult = DialogResult.Cancel,
            Cursor       = Cursors.Hand
        };

        dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
        dlg.AcceptButton = btnOk;
        dlg.CancelButton = btnCancel;

        return dlg.ShowDialog(this) == DialogResult.OK ? txt.Text : null;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Resets the entry panel to "Add New User" mode.</summary>
    private void ClearEntry()
    {
        _editingUserId = 0;

        txtNewUsername.Clear();
        txtNewPassword.Clear();
        txtNewFullName.Clear();
        cmbNewRole.SelectedIndex = 1; // default Supervisor

        // Restore visibility for create mode.
        lblNewUsername.Visible = true;
        txtNewUsername.Visible = true;
        lblNewPassword.Visible = true;
        txtNewPassword.Visible = true;

        lblEntryTitle.Text    = "Add New User";
        btnCreateUser.Text    = "Create User";
        btnCancelEdit.Visible = false;
    }

    /// <summary>Shows a coloured status message that auto-hides after 3 seconds.</summary>
    private void ShowStatus(string message, bool success)
    {
        lblUserStatus.Text      = message;
        lblUserStatus.ForeColor = success
            ? Color.FromArgb(40, 167, 69)
            : Color.FromArgb(220, 53, 69);
        lblUserStatus.Visible   = true;

        var timer = new System.Windows.Forms.Timer { Interval = 3000 };
        timer.Tick += (_, _) =>
        {
            lblUserStatus.Visible = false;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    // Null-safe column helpers
    private void SetColVisible(string n, bool v) { if (dgvUsers.Columns[n] is { } c) c.Visible    = v; }
    private void SetColHeader(string n, string h) { if (dgvUsers.Columns[n] is { } c) c.HeaderText = h; }
    private void SetColWeight(string n, int w)    { if (dgvUsers.Columns[n] is { } c) c.FillWeight = w; }
}
