using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// The application's sign-in form.
/// Validates inputs and delegates credential checking to <see cref="AuthService"/>.
/// On success opens <see cref="DashboardForm"/> and returns here on sign-out.
/// </summary>
public partial class LoginForm : Form
{
    private readonly DbConnectionFactory _factory;
    private readonly AuthService         _auth;

    /// <summary>
    /// Initialises the LoginForm and wires up the authentication service.
    /// </summary>
    /// <param name="factory">
    /// The database connection factory passed in from <c>Program.cs</c>.
    /// </param>
    public LoginForm(DbConnectionFactory factory)
    {
        _factory = factory;
        InitializeComponent();
        _auth = new AuthService(new UserRepository(factory));
    }

    // ── Event handlers ───────────────────────────────────────────────────────

    /// <summary>
    /// Handles the Sign In button click.
    /// Validates inputs, then calls <see cref="AuthService.Login"/>.
    /// On success opens the Dashboard; on failure shows an inline error.
    /// </summary>
    private void BtnSignIn_Click(object sender, EventArgs e)
    {
        lblError.Visible = false;

        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        // ── Empty-field validation ───────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(username))
        {
            ShowError("Please enter your username.");
            txtUsername.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Please enter your password.");
            txtPassword.Focus();
            return;
        }

        // ── Delegate to AuthService (BCrypt verify happens inside) ───────────
        var user = _auth.Login(username, password);

        if (user is null)
        {
            ShowError("Invalid username or password.");
            txtPassword.Clear();
            txtPassword.Focus();
            return;
        }

        // ── Establish the session, then force a password change if needed ───
        // Setting Session.Current first means AuthService.ChangePassword updates
        // this same user object, keeping the in-memory hash current.
        Session.Current = user;

        bool isDefault    = BCrypt.Net.BCrypt.Verify("ChangeMe123", user.PasswordHash);
        bool mustChange   = user.MustChangePassword || isDefault;
        if (mustChange)
        {
            MessageBox.Show(
                "You must set a new password before continuing.",
                "Password Change Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            using var pwForm = new ChangePasswordForm(user, _factory);
            if (pwForm.ShowDialog() != DialogResult.OK)
            {
                Session.SignOut();   // user cancelled — block access
                return;
            }
        }

        // ── Success — open Dashboard ─────────────────────────────────────────
        txtPassword.Clear();

        Hide();
        using (var dash = new DashboardForm(_factory))
            dash.ShowDialog();

        // After the dashboard closes, decide what to do:
        //   • Sign Out clears Session.Current → return to the login screen.
        //   • Closing via the window X leaves the session set → exit the app.
        if (Session.Current is null)
            Show();
        else
            Close();   // LoginForm is the main form, so this exits the application
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Displays an error message in the red label below the Sign In button.
    /// </summary>
    /// <param name="message">The message to display.</param>
    private void ShowError(string message)
    {
        lblError.Text    = message;
        lblError.Visible = true;
    }
}
