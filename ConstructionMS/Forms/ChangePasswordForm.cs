using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Allows the currently signed-in user to change their own password.
/// Also used at first login when the account still has the default password.
/// Verifies the current password with BCrypt before saving the new hash.
/// </summary>
public partial class ChangePasswordForm : Form
{
    private readonly AuthService _auth;

    /// <summary>
    /// Non-null when the form is opened from the login flow before
    /// <see cref="Session.Current"/> is set.
    /// </summary>
    private readonly User? _forcedUser;

    // ── Constructors ──────────────────────────────────────────────────────────

    /// <summary>
    /// Standard constructor — used from the Dashboard header button.
    /// Reads the current user from <see cref="Session.Current"/>.
    /// </summary>
    /// <param name="factory">Database connection factory.</param>
    public ChangePasswordForm(DbConnectionFactory factory)
    {
        _auth = new AuthService(factory, new UserRepository(factory));
        InitializeComponent();
        Theme.Apply(this);
    }

    /// <summary>
    /// First-login constructor — used when the user still has the default password.
    /// The <paramref name="user"/> object provides the subtitle and user ID.
    /// Returns <see cref="DialogResult.OK"/> on success,
    /// <see cref="DialogResult.Cancel"/> if the user closes without changing.
    /// </summary>
    /// <param name="user">The authenticated user who must change their password.</param>
    /// <param name="factory">Database connection factory.</param>
    public ChangePasswordForm(User user, DbConnectionFactory factory)
    {
        _auth       = new AuthService(factory, new UserRepository(factory));
        _forcedUser = user;
        InitializeComponent();
        // Override the subtitle set by InitializeComponent (Session.Current may be unset here).
        lblSubtitle.Text = $"Changing password for:  {user.FullName}";
        Theme.Apply(this);
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    /// <summary>
    /// Updates the password strength indicator as the user types.
    /// </summary>
    private void TxtNew_TextChanged(object sender, EventArgs e)
    {
        string pw = txtNew.Text;

        if (pw.Length == 0)
        {
            lblStrength.Text      = "";
            lblStrength.ForeColor = Color.Gray;
            return;
        }

        int score = 0;
        if (pw.Length >= 8)                            score++;
        if (pw.Any(char.IsUpper))                      score++;
        if (pw.Any(char.IsDigit))                      score++;
        if (pw.Any(c => !char.IsLetterOrDigit(c)))     score++;

        (lblStrength.Text, lblStrength.ForeColor) = score switch
        {
            1 => ("Strength: Weak",   Color.Crimson),
            2 => ("Strength: Fair",   Color.DarkOrange),
            3 => ("Strength: Good",   Color.CadetBlue),
            _ => ("Strength: Strong", Color.SeaGreen)
        };
    }

    /// <summary>
    /// Validates all fields then delegates verification and hashing to
    /// <see cref="AuthService.ChangePassword"/>.
    /// Sets <see cref="Form.DialogResult"/> to <see cref="DialogResult.OK"/> on success.
    /// </summary>
    private void BtnSave_Click(object sender, EventArgs e)
    {
        lblStatus.Visible = false;

        // ── Field validation ──────────────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(txtCurrent.Text))
        {
            ShowStatus("Please enter your current password.", success: false);
            txtCurrent.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtNew.Text))
        {
            ShowStatus("Please enter a new password.", success: false);
            txtNew.Focus();
            return;
        }

        if (txtNew.Text.Length < 8)
        {
            ShowStatus("New password must be at least 8 characters.", success: false);
            txtNew.Focus();
            return;
        }

        if (txtNew.Text != txtConfirm.Text)
        {
            ShowStatus("New passwords do not match.", success: false);
            txtConfirm.Clear();
            txtConfirm.Focus();
            return;
        }

        if (txtNew.Text == txtCurrent.Text)
        {
            ShowStatus("New password must differ from the current one.", success: false);
            txtNew.Focus();
            return;
        }

        // ── Verify and save via AuthService ───────────────────────────────────
        int userId = (_forcedUser ?? Session.Current!).UserId;
        bool changed = _auth.ChangePassword(userId, txtCurrent.Text, txtNew.Text);

        if (!changed)
        {
            ShowStatus("Current password is incorrect.", success: false);
            txtCurrent.Clear();
            txtCurrent.Focus();
            return;
        }

        ShowStatus("Password changed successfully.", success: true);
        btnSave.Enabled = false;

        // Auto-close after a short delay so the user sees the success message.
        var timer = new System.Windows.Forms.Timer { Interval = 1500 };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            DialogResult = DialogResult.OK;
            Close();
        };
        timer.Start();
    }

    /// <summary>
    /// Ensures closing the form without saving always returns
    /// <see cref="DialogResult.Cancel"/> to the caller.
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.None)
            DialogResult = DialogResult.Cancel;
        base.OnFormClosing(e);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Shows a coloured status message.</summary>
    private void ShowStatus(string message, bool success)
    {
        lblStatus.Text      = message;
        lblStatus.ForeColor = success ? Color.FromArgb(40, 167, 69) : Color.Crimson;
        lblStatus.Visible   = true;
    }
}
