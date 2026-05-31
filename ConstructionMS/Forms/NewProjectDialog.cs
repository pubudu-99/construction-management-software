using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Modal dialog for creating a new active project. Only succeeds when no other
/// project is currently active — the repository enforces the single-active rule.
/// Returns <see cref="DialogResult.OK"/> when a project is created.
/// </summary>
public partial class NewProjectDialog : Form
{
    private readonly DbConnectionFactory _factory;
    private readonly ProjectRepository   _projectRepo;

    /// <summary>Initialises the dialog with database access.</summary>
    public NewProjectDialog(DbConnectionFactory factory)
    {
        _factory     = factory;
        _projectRepo = new ProjectRepository(factory);
        InitializeComponent();
        Theme.Apply(this);
    }

    /// <summary>Validates input, inserts the project, and logs the action.</summary>
    private void BtnCreate_Click(object sender, EventArgs e)
    {
        string name = txtNewProjName.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("Project name is required.");
            return;
        }

        if (numNewBudget.Value <= 0)
        {
            ShowError("Budget must be greater than zero.");
            return;
        }

        if (dtpNewEnd.Value.Date < dtpNewStart.Value.Date)
        {
            ShowError("End date must be on or after the start date.");
            return;
        }

        var project = new Project
        {
            Name      = name,
            Budget    = numNewBudget.Value,
            StartDate = dtpNewStart.Value.Date,
            EndDate   = dtpNewEnd.Value.Date,
            Status    = "Active"
        };

        try
        {
            _projectRepo.Insert(project);
        }
        catch (InvalidOperationException ex)
        {
            // Another active project already exists — show the exact message.
            ShowError(ex.Message);
            return;
        }

        ActivityLogger.Log(_factory, "Project Created",
            $"{project.Name} (Budget LKR {project.Budget:N2})");

        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>Closes the dialog without creating a project.</summary>
    private void BtnCancel_Click(object sender, EventArgs e) => Close();

    /// <summary>Shows a red inline error message.</summary>
    private void ShowError(string message)
    {
        lblNewStatus.Text      = message;
        lblNewStatus.ForeColor = Color.FromArgb(220, 53, 69);
        lblNewStatus.Visible   = true;
    }

    /// <summary>Defaults the dialog result to Cancel if closed without creating.</summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.None)
            DialogResult = DialogResult.Cancel;
        base.OnFormClosing(e);
    }
}
