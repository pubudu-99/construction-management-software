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
    private readonly ProjectService _service;

    /// <summary>The project currently being edited (loaded on form open).</summary>
    private Project? _project;

    /// <summary>Initialises the form with database access.</summary>
    public ProjectForm(DbConnectionFactory factory)
    {
        _service = new ProjectService(new ProjectRepository(factory));
        InitializeComponent();
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

        _project = _service.GetCurrent();
        if (_project is null)
        {
            MessageBox.Show(
                "No project found in the database.",
                "No Project",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            BeginInvoke((Action)Close);
            return;
        }

        // Populate the fields from the loaded project.
        txtProjectName.Text = _project.Name;
        numBudget.Value     = Math.Min(_project.Budget, numBudget.Maximum);
        lblSpent.Text       = $"LKR {_project.Spent:N2}";
        dtpStart.Value      = _project.StartDate;
        dtpEnd.Value        = _project.EndDate;
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

        ShowStatus("Project saved successfully.", success: true);
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
