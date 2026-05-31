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

        // Sign Out reads as a light button on the brand-coloured header.
        Theme.StyleButton(btnSignOut, Theme.ButtonRole.Secondary);

        // Keep KPI row heights correct if the window moves to a different-DPI monitor.
        DpiChanged += (_, _) => SizeKpiRows();

        Load += DashboardForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Sets welcome label, hides Users button for non-Managers,
    /// and loads all alert cards in the background.</summary>
    private void DashboardForm_Load(object? sender, EventArgs e)
    {
        lblWelcome.Text           = $"Welcome, {Session.Current!.FullName}   ·   {Session.Current.Role}";
        btnNavUsers.Visible       = Session.IsManager;
        btnNavProject.Visible     = Session.IsManager;
        btnNavWorkers.Visible     = Session.IsManager;
        mnuFileSampleData.Visible = Session.IsManager;

        // Re-flow the nav so hidden Manager-only items leave no gaps and the
        // empty ADMIN section caption disappears for Supervisors.
        CollapseNav();

        SizeKpiRows();
        LoadKpis();
        LoadDeadlineAlerts();
        LoadMaintenanceAlerts();
        LoadLowStockAlerts();
    }

    /// <summary>
    /// Repositions the nav-panel captions and buttons top-down, skipping any
    /// hidden buttons so there are no gaps, and hides a section caption when
    /// all of its buttons are hidden (e.g. ADMIN for non-Managers).
    /// Spacing matches AddNavSection (24) and SetupNavButton (40).
    /// </summary>
    private void CollapseNav()
    {
        var sections = new (Label Heading, Button[] Buttons)[]
        {
            (lblNavOperations, new[] { btnNavProject, btnNavPayments, btnNavTasks, btnNavReports }),
            (lblNavResources,  new[] { btnNavEquipment, btnNavMaterials }),
            (lblNavPeople,     new[] { btnNavAttendance, btnNavPayroll, btnNavContacts, btnNavWorkers }),
            (lblNavAdmin,      new[] { btnNavUsers }),
        };

        int y = 12;
        foreach (var (heading, buttons) in sections)
        {
            var visible = buttons.Where(b => b.Visible).ToList();
            if (visible.Count == 0)
            {
                heading.Visible = false;
                continue;
            }

            heading.Visible  = true;
            heading.Location = new Point(16, y + 6);
            y += 24;

            foreach (var b in visible)
            {
                b.Location = new Point(10, y);
                y += 40;
            }
        }
    }

    // ── KPI cards ───────────────────────────────────────────────────────────────

    /// <summary>Snapshot of dashboard metrics gathered on a background thread.</summary>
    private sealed record KpiData(
        bool HasProject, string ProjectName, decimal Budget, decimal Spent,
        int ActiveWorkers, int OpenTasks, int OverdueTasks, int LowStock, int MaintenanceDue);

    /// <summary>
    /// Gathers KPI figures off the UI thread, then renders the stat cards.
    /// Safe when no active project exists (cards show a friendly empty state).
    /// </summary>
    private void LoadKpis()
    {
        System.Threading.Tasks.Task
            .Run(() =>
            {
                var project = new ProjectRepository(_factory).GetActive();
                int workers = new WorkerRepository(_factory).GetActive().Count;
                // Open-task count is scoped to the active project (0 when none),
                // so a completed project's tasks never inflate the dashboard.
                int open    = project is null
                    ? 0
                    : new TaskRepository(_factory).GetOpenTasksForProject(project.ProjectId).Count;
                int overdue = new ScheduleService(_factory, new TaskRepository(_factory))
                    .GetDeadlineAlerts().Count(a => a.IsOverdue);
                int lowStock = new InventoryService(
                        new MaterialRepository(_factory), new StockMovementRepository(_factory))
                    .GetLowStockAlerts().Count;
                int maint = new EquipmentService(_factory, new EquipmentRepository(_factory))
                    .GetMaintenanceAlerts().Count;

                return new KpiData(
                    project is not null,
                    project?.Name ?? "",
                    project?.Budget ?? 0m,
                    project?.Spent ?? 0m,
                    workers, open, overdue, lowStock, maint);
            })
            .ContinueWith(t => RenderKpis(t.Result),
                System.Threading.CancellationToken.None,
                System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion,
                System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>Builds the KPI stat cards into two full-width rows (3 + 4).</summary>
    private void RenderKpis(KpiData k)
    {
        // Reflect the active project's name on the dashboard so renames
        // (or switching to a new project after Mark Complete) are visible
        // without reopening the form.
        Text = k.HasProject
            ? $"Construction Management - {k.ProjectName}"
            : "Construction Management - Dashboard";

        tlpKpiRow1.SuspendLayout();
        tlpKpiRow2.SuspendLayout();
        tlpKpiRow1.Controls.Clear();
        tlpKpiRow2.Controls.Clear();

        // Row 1 — project finances
        if (k.HasProject)
        {
            decimal remaining = k.Budget - k.Spent;
            double  pct       = k.Budget > 0 ? (double)(k.Spent / k.Budget) * 100 : 0;
            Color   pctColor  = pct >= 90 ? Theme.Danger : pct >= 75 ? Theme.Caution : Theme.Success;

            // Budget card title doubles as the project-name label — that's the
            // most prominent place to surface the current project on the dashboard.
            tlpKpiRow1.Controls.Add(MakeBudgetCard(k.ProjectName, k.Budget, pct, pctColor), 0, 0);
            tlpKpiRow1.Controls.Add(MakeStatCard("Spent",     $"LKR {k.Spent:N0}",   $"{pct:0.#}% of budget", pctColor), 1, 0);
            tlpKpiRow1.Controls.Add(MakeStatCard("Remaining", $"LKR {remaining:N0}", "available to spend",
                remaining >= 0 ? Theme.Success : Theme.Danger), 2, 0);
        }
        else
        {
            tlpKpiRow1.Controls.Add(
                MakeStatCard("Project Budget", "No active project", "Create one in Project Setup", Theme.TextMuted), 0, 0);
        }

        // Row 2 — operations
        tlpKpiRow2.Controls.Add(MakeStatCard("Active Workers", k.ActiveWorkers.ToString(), "on the payroll", Theme.TextPrimary), 0, 0);
        tlpKpiRow2.Controls.Add(MakeStatCard("Open Tasks", k.OpenTasks.ToString(),
            k.OverdueTasks > 0 ? $"{k.OverdueTasks} overdue" : "none overdue",
            k.OverdueTasks > 0 ? Theme.Danger : Theme.TextPrimary), 1, 0);
        tlpKpiRow2.Controls.Add(MakeStatCard("Low Stock", k.LowStock.ToString(),
            "items below reorder", k.LowStock > 0 ? Theme.Caution : Theme.TextPrimary), 2, 0);
        tlpKpiRow2.Controls.Add(MakeStatCard("Maintenance Due", k.MaintenanceDue.ToString(),
            "equipment items", k.MaintenanceDue > 0 ? Theme.Caution : Theme.TextPrimary), 3, 0);

        tlpKpiRow1.ResumeLayout();
        tlpKpiRow2.ResumeLayout();
    }

    /// <summary>Sizes the two KPI rows for the current DPI so nothing clips.</summary>
    private void SizeKpiRows()
    {
        int h = (int)Math.Round(128 * (DeviceDpi / 96f));
        tlpKpiRow1.Height = h;
        tlpKpiRow2.Height = h;
    }

    /// <summary>Creates a single stat card (title, big value, sub-caption) that fills its cell.</summary>
    private static CardPanel MakeStatCard(string title, string value, string sub, Color valueColor)
    {
        var card = new CardPanel(title, Theme.Primary)
        {
            Dock   = DockStyle.Fill,
            Margin = new Padding(0, 0, 14, 12),
        };

        // A top-down flow with AutoSize labels never clips at any DPI.
        var flow = new FlowLayoutPanel
        {
            Dock          = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents  = false,
            BackColor     = Theme.Card,
            Padding       = new Padding(0),
        };
        flow.Controls.Add(new Label
        {
            Text = value, Font = Theme.CardValue(), ForeColor = valueColor,
            AutoSize = true, Margin = new Padding(0, 2, 0, 0),
        });
        flow.Controls.Add(new Label
        {
            Text = sub, Font = Theme.Small(), ForeColor = Theme.TextMuted,
            AutoSize = true, Margin = new Padding(0, 4, 0, 0),
        });

        card.Content.Controls.Add(flow);
        return card;
    }

    /// <summary>
    /// Creates the budget card with a responsive coloured usage bar. The card
    /// title shows the active project's name (the most prominent place to surface it).
    /// </summary>
    private static CardPanel MakeBudgetCard(string projectName, decimal budget, double pct, Color pctColor)
    {
        string title = string.IsNullOrWhiteSpace(projectName) ? "Project Budget" : projectName;
        var card = MakeStatCard(title, $"LKR {budget:N0}", $"{pct:0.#}% used", Theme.TextPrimary);
        // Added after the Fill flow, so it docks along the bottom of the card.
        card.Content.Controls.Add(new MiniBar
        {
            Dock      = DockStyle.Bottom,
            Percent   = pct,
            FillColor = pctColor,
            Margin    = new Padding(0),
        });
        return card;
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
            .Run(() =>
            {
                bool hasProject = new ProjectRepository(_factory).GetActive() is not null;
                var list = new ScheduleService(_factory, new TaskRepository(_factory)).GetDeadlineAlerts();
                return (HasProject: hasProject, Alerts: list);
            })
            .ContinueWith(t =>
            {
                var hasProject = t.Result.HasProject;
                var alerts     = t.Result.Alerts;
                rtbDeadlines.Clear();

                // No active project → say so explicitly rather than implying
                // a completed project is "all clear".
                if (!hasProject)
                {
                    cardDeadlines.TitleLabel.Text = "Upcoming Deadlines";
                    rtbDeadlines.SelectionColor   = Color.Gray;
                    rtbDeadlines.AppendText("No active project");
                    return;
                }

                var overdue  = alerts.Where(a => a.IsOverdue).ToList();
                var dueSoon  = alerts.Where(a => !a.IsOverdue && a.DueIn <= 3).ToList();

                cardDeadlines.TitleLabel.Text = (overdue.Count + dueSoon.Count) == 0
                    ? "Upcoming Deadlines"
                    : $"Upcoming Deadlines  •  {overdue.Count} overdue, {dueSoon.Count} soon";

                if (overdue.Count == 0 && dueSoon.Count == 0)
                {
                    rtbDeadlines.SelectionColor = Color.SeaGreen;
                    rtbDeadlines.AppendText("All clear ✓");
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
            .Run(() => new EquipmentService(_factory, new EquipmentRepository(_factory))
                .GetMaintenanceAlerts())
            .ContinueWith(t =>
            {
                var alerts = t.Result;
                rtbMaintenance.Clear();

                cardMaintenance.TitleLabel.Text = alerts.Count == 0
                    ? "Maintenance Due"
                    : $"Maintenance Due  •  {alerts.Count}";

                if (alerts.Count == 0)
                {
                    rtbMaintenance.SelectionColor = Color.SeaGreen;
                    rtbMaintenance.AppendText("All clear ✓");
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

                cardLowStock.TitleLabel.Text = alerts.Count == 0
                    ? "Low Stock"
                    : $"Low Stock  •  {alerts.Count}";

                if (alerts.Count == 0)
                {
                    rtbLowStock.SelectionColor = Color.SeaGreen;
                    rtbLowStock.AppendText("All clear ✓");
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

    /// <summary>Opens the Change Password dialog (File menu).</summary>
    private void MnuChangePassword_Click(object sender, EventArgs e)
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

    /// <summary>
    /// Loads realistic demo data into the database (Manager only).
    /// Idempotent — safe to run multiple times. Refreshes the alert cards
    /// afterwards so newly seeded deadlines/maintenance/stock appear.
    /// </summary>
    private void MnuFileSampleData_Click(object sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Load sample demo data into the database?\n" +
            "This is safe to run multiple times. Continue?",
            "Load Sample Data",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        var seeder = new SampleDataSeeder(
            _factory,
            new WorkerRepository(_factory),
            new AttendanceRepository(_factory),
            new MaterialRepository(_factory),
            new EquipmentRepository(_factory),
            new ContactRepository(_factory),
            new PaymentRepository(_factory),
            new TaskRepository(_factory),
            new ProjectRepository(_factory));

        string result = seeder.LoadSampleData();
        MessageBox.Show(result, "Sample Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Reflect the newly seeded data on the dashboard alert cards and KPIs.
        LoadKpis();
        LoadDeadlineAlerts();
        LoadMaintenanceAlerts();
        LoadLowStockAlerts();
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
        LoadKpis();
    }

    /// <summary>Opens the Payments form (FR2).</summary>
    private void BtnNavPayments_Click(object sender, EventArgs e)
    {
        using var form = new PaymentForm(_factory);
        form.ShowDialog();
        LoadKpis();
    }

    /// <summary>Opens the Task Scheduling form (FR3).</summary>
    private void BtnNavTasks_Click(object sender, EventArgs e)
    {
        using var form = new TaskForm(_factory);
        form.ShowDialog();
        LoadDeadlineAlerts();
        LoadKpis();
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
        using var form = new EquipmentForm(_factory);
        form.ShowDialog();
        LoadMaintenanceAlerts();
        LoadKpis();
    }

    /// <summary>Opens the Materials and Stock form (FR6) and reloads low-stock alerts after close.</summary>
    private void BtnNavMaterials_Click(object sender, EventArgs e)
    {
        using var form = new StockForm(
            new MaterialRepository(_factory),
            new StockMovementRepository(_factory));
        form.ShowDialog();
        LoadLowStockAlerts();
        LoadKpis();
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

    /// <summary>Opens the Worker Management form. Managers only.</summary>
    private void BtnNavWorkers_Click(object sender, EventArgs e)
    {
        using var form = new WorkerManagementForm(_factory);
        form.ShowDialog();
    }
}
