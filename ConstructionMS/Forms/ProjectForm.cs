using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Project Setup form (Manager only).
/// Edits the current project's name, budget, and timeline.
/// Spent is shown read-only — it is changed only by the payment transaction.
/// </summary>
public partial class ProjectForm : Form
{
    private readonly DbConnectionFactory _factory;
    private readonly ProjectRepository   _projectRepo;
    private readonly ProjectService      _service;

    /// <summary>The project currently being edited (loaded on form open).</summary>
    private Project? _project;

    /// <summary>Initialises the form with database access.</summary>
    public ProjectForm(DbConnectionFactory factory)
    {
        _factory     = factory;
        _projectRepo = new ProjectRepository(factory);
        _service     = new ProjectService(_projectRepo);
        InitializeComponent();

        // Button roles — colours now match availability, with a proper disabled look.
        Theme.StyleButton(btnSave,         Theme.ButtonRole.Primary);  // main edit action
        Theme.StyleButton(btnMarkComplete, Theme.ButtonRole.Caution);  // deliberate state change
        Theme.StyleButton(btnCreateNew,    Theme.ButtonRole.Success);  // create next project
        Theme.Apply(this);

        Load += ProjectForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Enforces Manager-only access then loads the current project.</summary>
    private void ProjectForm_Load(object? sender, EventArgs e)
    {
        if (!Session.IsManager)
        {
            MessageBox.Show(
                "Access denied. Only Managers can edit project settings.",
                "Access Denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            BeginInvoke((Action)Close);
            return;
        }

        if (!LoadCurrentProject())
        {
            MessageBox.Show(
                "No project found in the database.",
                "No Project",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            BeginInvoke((Action)Close);
        }
    }

    /// <summary>
    /// Loads the active project (or the most recent project if none is active,
    /// so a completed project remains viewable) and populates the form.
    /// </summary>
    /// <returns><c>true</c> if a project was loaded; <c>false</c> if none exist.</returns>
    private bool LoadCurrentProject()
    {
        // Prefer the active project; fall back to the most recent (e.g. a just-
        // completed project) so the user can review it and create a new one.
        _project = _projectRepo.GetActive() ?? _projectRepo.GetAll().FirstOrDefault();
        if (_project is null) return false;

        txtProjectName.Text = _project.Name;
        numBudget.Value     = Math.Min(_project.Budget, numBudget.Maximum);
        lblSpent.Text       = $"LKR {_project.Spent:N2}";
        dtpStart.Value      = _project.StartDate;
        dtpEnd.Value        = _project.EndDate;

        ApplyStatusUi();
        return true;
    }

    /// <summary>
    /// Updates the status banner and enables/disables the action buttons and
    /// editable fields according to whether the loaded project is active.
    /// </summary>
    private void ApplyStatusUi()
    {
        if (_project is null) return;

        bool isActive = _project.Status == "Active";

        lblStatusBanner.Text      = $"Current Project: {_project.Name}   |   Status: {_project.Status}";
        lblStatusBanner.ForeColor = isActive ? Theme.Success : Theme.TextMuted;

        // Completed projects are read-only.
        txtProjectName.Enabled = isActive;
        numBudget.Enabled      = isActive;
        dtpStart.Enabled       = isActive;
        dtpEnd.Enabled         = isActive;
        btnSave.Enabled        = isActive;
        btnMarkComplete.Enabled = isActive;
        btnCreateNew.Enabled    = !isActive;
    }

    // ── Save ────────────────────────────────────────────────────────────────────

    /// <summary>Validates via ProjectService and saves the project changes.</summary>
    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (_project is null) return;

        // Apply edited values onto the loaded project (Spent stays untouched).
        _project.Name      = txtProjectName.Text.Trim();
        _project.Budget    = numBudget.Value;
        _project.StartDate = dtpStart.Value.Date;
        _project.EndDate   = dtpEnd.Value.Date;

        var result = _service.Update(_project);

        if (!result.Success)
        {
            ShowStatus(result.Message, success: false);
            return;
        }

        // Refresh the status banner so the renamed project shows immediately
        // (otherwise lblStatusBanner keeps the old name until the form is reopened).
        ApplyStatusUi();

        // Record the change in the activity log for the audit trail.
        ActivityLogger.Log(_factory, "Project Updated", _project.Name);

        ShowStatus("Project saved successfully.", success: true);
    }

    // ── Lifecycle actions ───────────────────────────────────────────────────────

    /// <summary>Marks the active project complete after confirmation, then closes.</summary>
    private void BtnMarkComplete_Click(object sender, EventArgs e)
    {
        if (_project is null) return;

        var answer = MessageBox.Show(
            $"Mark '{_project.Name}' as Complete? After this, you can create a new " +
            "project. The completed project's data will remain viewable in Reports. Continue?",
            "Mark Project Complete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (answer != DialogResult.Yes) return;

        _projectRepo.MarkComplete(_project.ProjectId);
        ActivityLogger.Log(_factory, "Project Completed", _project.Name);

        MessageBox.Show(
            $"'{_project.Name}' has been marked complete. You can now create a new project.",
            "Project Completed",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        Close();
    }

    /// <summary>Opens the new-project dialog; reloads the form on success.</summary>
    private void BtnCreateNew_Click(object sender, EventArgs e)
    {
        using var dlg = new NewProjectDialog(_factory);
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            LoadCurrentProject();
            ShowStatus("New project created and now active.", success: true);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Shows a coloured status message that auto-hides after 3 seconds.</summary>
    private void ShowStatus(string message, bool success)
    {
        lblStatus.Text      = message;
        lblStatus.ForeColor = success
            ? Color.FromArgb(40, 167, 69)
            : Color.FromArgb(220, 53, 69);
        lblStatus.Visible   = true;

        var timer = new System.Windows.Forms.Timer { Interval = 3000 };
        timer.Tick += (_, _) =>
        {
            lblStatus.Visible = false;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }
}
