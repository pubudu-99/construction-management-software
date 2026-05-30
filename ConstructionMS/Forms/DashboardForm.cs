using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Main application window shown after sign-in.
/// Displays deadline, maintenance, and low-stock alert cards and provides
/// navigation to all modules.
/// </summary>
public partial class DashboardForm : Form
{
    private readonly DbConnectionFactory _factory;

    /// <summary>
    /// Initialises the Dashboard and registers the load handler.
    /// </summary>
    /// <param name="factory">Passed through to child forms that need DB access.</param>
    public DashboardForm(DbConnectionFactory factory)
    {
        _factory = factory;
        InitializeComponent();
        Load += DashboardForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Sets welcome label, hides Users button for non-Managers,
    /// and loads all alert cards in the background.</summary>
    private void DashboardForm_Load(object? sender, EventArgs e)
    {
        lblWelcome.Text       = $"Welcome, {Session.Current!.FullName}   |   {Session.Current.Role}";
        btnNavUsers.Visible   = Session.IsManager;
        btnNavProject.Visible = Session.IsManager;

        LoadDeadlineAlerts();
        LoadMaintenanceAlerts();
        LoadLowStockAlerts();
    }

    // ── Alert loaders ─────────────────────────────────────────────────────────

    /// <summary>
    /// Loads upcoming task deadlines on a background thread and populates
    /// the Upcoming Deadlines card. Output is split into two sections:
    /// OVERDUE (red) and DUE SOON (amber). Shows "All clear" in green when both empty.
    /// </summary>
    private void LoadDeadlineAlerts()
    {
        System.Threading.Tasks.Task
            .Run(() => new ScheduleService(new TaskRepository(_factory)).GetDeadlineAlerts())
            .ContinueWith(t =>
            {
                var alerts = t.Result;
                rtbDeadlines.Clear();

                var overdue  = alerts.Where(a => a.IsOverdue).ToList();
                var dueSoon  = alerts.Where(a => !a.IsOverdue && a.DueIn <= 3).ToList();

                if (overdue.Count == 0 && dueSoon.Count == 0)
                {
                    rtbDeadlines.SelectionColor = Color.SeaGreen;
                    rtbDeadlines.AppendText("All clear");
                    return;
                }

                if (overdue.Count > 0)
                {
                    rtbDeadlines.SelectionColor = Color.Crimson;
                    rtbDeadlines.AppendText("OVERDUE:" + Environment.NewLine);
                    foreach (var a in overdue)
                    {
                        rtbDeadlines.SelectionColor = Color.Crimson;
                        int daysOver = -a.DueIn;
                        string label = daysOver == 1 ? "1 day overdue" : $"{daysOver} days overdue";
                        rtbDeadlines.AppendText($"   {a.TaskName} — {label}{Environment.NewLine}");
                    }
                }

                if (dueSoon.Count > 0)
                {
                    if (overdue.Count > 0)
                        rtbDeadlines.AppendText(Environment.NewLine);

                    rtbDeadlines.SelectionColor = Color.DarkOrange;
                    rtbDeadlines.AppendText("DUE SOON:" + Environment.NewLine);
                    foreach (var a in dueSoon)
                    {
                        rtbDeadlines.SelectionColor = Color.DarkOrange;
                        string label = a.DueIn == 0
                            ? "due today"
                            : (a.DueIn == 1 ? "due in 1 day" : $"due in {a.DueIn} days");
                        rtbDeadlines.AppendText($"   {a.TaskName} — {label}{Environment.NewLine}");
                    }
                }
            },
            System.Threading.CancellationToken.None,
            System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion,
            System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>
    /// Loads equipment maintenance alerts on a background thread and populates
    /// the Maintenance Due card (warning window read from App.config).
    /// </summary>
    private void LoadMaintenanceAlerts()
    {
        System.Threading.Tasks.Task
            .Run(() => new EquipmentService(new EquipmentRepository(_factory))
                .GetMaintenanceAlerts())
            .ContinueWith(t =>
            {
                var alerts = t.Result;
                rtbMaintenance.Clear();

                if (alerts.Count == 0)
                {
                    rtbMaintenance.SelectionColor = Color.SeaGreen;
                    rtbMaintenance.AppendText("All clear");
                    return;
                }

                foreach (var a in alerts)
                {
                    rtbMaintenance.SelectionColor = a.DaysUntilDue <= 0
                        ? Color.Crimson
                        : Color.DarkOrange;

                    string prefix = a.DaysUntilDue < 0 ? "OVERDUE  " : "";
                    string when   = a.DaysUntilDue <= 0
                        ? $"{-a.DaysUntilDue}d overdue"
                        : $"in {a.DaysUntilDue}d";
                    rtbMaintenance.AppendText(
                        $"{prefix}{a.EquipmentName}  —  {when}{Environment.NewLine}");
                }
            },
            System.Threading.CancellationToken.None,
            System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion,
            System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>
    /// Loads low-stock alerts on a background thread and populates
    /// the Low Stock card.
    /// </summary>
    private void LoadLowStockAlerts()
    {
        System.Threading.Tasks.Task
            .Run(() => new InventoryService(
                    new MaterialRepository(_factory),
                    new StockMovementRepository(_factory))
                .GetLowStockAlerts())
            .ContinueWith(t =>
            {
                var alerts = t.Result;
                rtbLowStock.Clear();

                if (alerts.Count == 0)
                {
                    rtbLowStock.SelectionColor = Color.SeaGreen;
                    rtbLowStock.AppendText("All clear");
                    return;
                }

                foreach (var a in alerts)
                {
                    rtbLowStock.SelectionColor = Color.DarkOrange;
                    rtbLowStock.AppendText(
                        $"{a.MaterialName}  —  {a.CurrentStock:0.##} {a.Unit}{Environment.NewLine}");
                }
            },
            System.Threading.CancellationToken.None,
            System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion,
            System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
    }

    // ── Header buttons ────────────────────────────────────────────────────────

    /// <summary>Opens the Change Password dialog.</summary>
    private void BtnChangePassword_Click(object sender, EventArgs e)
    {
        using var form = new ChangePasswordForm(_factory);
        form.ShowDialog();
    }

    /// <summary>Signs the user out and closes the dashboard (header button).</summary>
    private void BtnSignOut_Click(object sender, EventArgs e)
    {
        Session.SignOut();
        Close();
    }

    // ── Menu handlers ─────────────────────────────────────────────────────────

    /// <summary>
    /// Copies the database file to a user-chosen location as a backup.
    /// </summary>
    private void MnuFileBackup_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Title    = "Save Database Backup",
            Filter   = "SQLite Database (*.db)|*.db",
            FileName = $"construction-backup-{DateTime.Now:yyyy-MM-dd}.db"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            File.Copy(_factory.DatabasePath, dlg.FileName, overwrite: true);
            MessageBox.Show(
                "Backup saved successfully to:\n" + dlg.FileName,
                "Backup Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Backup failed:\n{ex.Message}",
                "Backup Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>Signs the user out via the File menu.</summary>
    private void MnuFileSignOut_Click(object sender, EventArgs e)
    {
        Session.SignOut();
        Close();
    }

    /// <summary>Shows the About dialog.</summary>
    private void MnuHelpAbout_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            "Construction Management Software v1.0\n\n" +
            "Developed by: Gunawardhana L.P.A.P.\n" +
            "Student ID:   E2421061\n" +
            "Programme:    BIT External Degree\n" +
            "University:   University of Moratuwa\n" +
            "Year:         2026\n\n" +
            "Module: ITE 1943 - ICT Project",
            "About Construction MS",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    // ── Nav buttons ───────────────────────────────────────────────────────────

    /// <summary>Opens the Project Setup form (Manager only).</summary>
    private void BtnNavProject_Click(object sender, EventArgs e)
    {
        using var form = new ProjectForm(_factory);
        form.ShowDialog();
    }

    /// <summary>Opens the Payments form (FR2).</summary>
    private void BtnNavPayments_Click(object sender, EventArgs e)
    {
        using var form = new PaymentForm(_factory);
        form.ShowDialog();
    }

    /// <summary>Opens the Task Scheduling form (FR3).</summary>
    private void BtnNavTasks_Click(object sender, EventArgs e)
    {
        using var form = new TaskForm(_factory);
        form.ShowDialog();
        LoadDeadlineAlerts();
    }

    /// <summary>Opens the Attendance recording form (FR4).</summary>
    private void BtnNavAttendance_Click(object sender, EventArgs e)
    {
        using var form = new AttendanceForm(_factory);
        form.ShowDialog();
    }

    /// <summary>Opens the Payroll Report form (FR4).</summary>
    private void BtnNavPayroll_Click(object sender, EventArgs e)
    {
        using var form = new PayrollForm(_factory);
        form.ShowDialog();
    }

    /// <summary>Opens the Equipment Management form (FR5) and reloads maintenance alerts after close.</summary>
    private void BtnNavEquipment_Click(object sender, EventArgs e)
    {
        using var form = new EquipmentForm(new EquipmentRepository(_factory));
        form.ShowDialog();
        LoadMaintenanceAlerts();
    }

    /// <summary>Opens the Materials and Stock form (FR6) and reloads low-stock alerts after close.</summary>
    private void BtnNavMaterials_Click(object sender, EventArgs e)
    {
        using var form = new StockForm(
            new MaterialRepository(_factory),
            new StockMovementRepository(_factory));
        form.ShowDialog();
        LoadLowStockAlerts();
    }

    /// <summary>Opens the Contacts form (FR7).</summary>
    private void BtnNavContacts_Click(object sender, EventArgs e)
    {
        using var form = new ContactForm(new ContactRepository(_factory));
        form.ShowDialog();
    }

    /// <summary>Opens the Reports form (Module 6).</summary>
    private void BtnNavReports_Click(object sender, EventArgs e)
    {
        using var form = new ReportForm(_factory);
        form.ShowDialog();
    }

    /// <summary>Opens the User Management form (FR1.1 / FR1.3). Managers only.</summary>
    private void BtnNavUsers_Click(object sender, EventArgs e)
    {
        using var form = new UserManagementForm(_factory);
        form.ShowDialog();
    }
}
