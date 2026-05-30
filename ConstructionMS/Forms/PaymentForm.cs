using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR2 — Payment Management form.
/// Left panel: record a new payment against the current project.
/// Right panel: browse payment history with a date-range filter.
/// </summary>
public partial class PaymentForm : Form
{
    private readonly PaymentService    _service;
    private readonly PaymentRepository _paymentRepo;
    private readonly ProjectRepository _projectRepo;

    private int      _projectId;
    private Project? _currentProject;

    /// <summary>
    /// Initialises the form and builds the service/repository graph.
    /// </summary>
    /// <param name="factory">Database connection factory.</param>
    public PaymentForm(DbConnectionFactory factory)
    {
        _projectRepo = new ProjectRepository(factory);
        _paymentRepo = new PaymentRepository(factory);
        _service     = new PaymentService(factory, _paymentRepo, _projectRepo);

        InitializeComponent();
        GridStyle.Apply(dgvPayments);

        // FR1.3 — Role-based access: only Managers may record payments.
        if (!Session.IsManager)
        {
            btnSave.Enabled = false;
            ShowStatus("Manager access required for recording payments.", success: false);
        }

        Load += PaymentForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Loads the first project, budget summary, and payment history.</summary>
    private void PaymentForm_Load(object? sender, EventArgs e)
    {
        if (!LoadProject()) return;
        RefreshBudgetLabels();
        LoadHistory();
    }

    /// <summary>
    /// Finds the first project and stores its ID.
    /// Warns and closes if no project exists.
    /// </summary>
    private bool LoadProject()
    {
        _currentProject = _projectRepo.GetFirst();
        if (_currentProject is null)
        {
            MessageBox.Show(
                "No projects found in the database.\n" +
                "Please add a project before recording payments.",
                "No Project",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Close();
            return false;
        }
        _projectId = _currentProject.ProjectId;
        return true;
    }

    // ── Button handlers ───────────────────────────────────────────────────────

    /// <summary>
    /// Validates inputs, builds a Payment, and delegates to PaymentService.
    /// Shows inline feedback — no MessageBox popup.
    /// </summary>
    private void BtnSave_Click(object sender, EventArgs e)
    {
        lblStatus.Visible = false;

        if (string.IsNullOrWhiteSpace(txtReference.Text))
        {
            ShowStatus("Reference cannot be empty.", success: false);
            txtReference.Focus();
            return;
        }

        if (numAmount.Value <= 0)
        {
            ShowStatus("Amount must be greater than zero.", success: false);
            numAmount.Focus();
            return;
        }

        var payment = new Payment
        {
            ProjectId   = _projectId,
            PayeeType   = cmbPayeeType.SelectedItem!.ToString()!,
            Reference   = txtReference.Text.Trim(),
            Amount      = numAmount.Value,
            PaymentDate = dtpDate.Value.Date,
            Note        = string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim()
        };

        var result = _service.Record(payment);

        if (result.Success)
        {
            ShowStatus("Payment saved successfully.", success: true);
            ClearForm();
            RefreshBudgetLabels();
            LoadHistory();
        }
        else
        {
            ShowStatus(result.Message, success: false);
        }
    }

    /// <summary>Refreshes the payment history grid.</summary>
    private void BtnRefresh_Click(object sender, EventArgs e) => LoadHistory();

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Reloads the project from the database and updates all four budget labels.
    /// </summary>
    private void RefreshBudgetLabels()
    {
        _currentProject = _projectRepo.GetById(_projectId);
        if (_currentProject is null) return;

        decimal remaining = _currentProject.Budget - _currentProject.Spent;

        lblBudgetProject.Text   = $"Project:  {_currentProject.Name}";
        lblBudgetBudget.Text    = $"Budget:     LKR {_currentProject.Budget:N2}";
        lblBudgetSpent.Text     = $"Spent:        LKR {_currentProject.Spent:N2}";
        lblBudgetRemaining.Text = $"Remaining: LKR {remaining:N2}";

        // Turn remaining label red if over 90% spent
        lblBudgetRemaining.ForeColor = remaining < _currentProject.Budget * 0.10m
            ? Color.Crimson
            : Color.SeaGreen;
    }

    /// <summary>
    /// Queries payment history for the selected range and binds the grid.
    /// </summary>
    private void LoadHistory()
    {
        if (_projectId == 0) return;

        var rows = _paymentRepo.GetForProject(
            _projectId, dtpFrom.Value.Date, dtpTo.Value.Date);

        dgvPayments.DataSource = rows.Select(p => new
        {
            ID        = p.PaymentId,
            Date      = p.PaymentDate.ToString("yyyy-MM-dd"),
            Type      = p.PayeeType,
            Reference = p.Reference,
            Amount    = p.Amount.ToString("N2"),
            Note      = p.Note ?? ""
        }).ToList();
    }

    /// <summary>Resets all entry controls to their defaults.</summary>
    private void ClearForm()
    {
        cmbPayeeType.SelectedIndex = 0;
        txtReference.Clear();
        numAmount.Value = 0;
        dtpDate.Value   = DateTime.Today;
        txtNote.Clear();
    }

    /// <summary>Shows a coloured status message below the Save button.</summary>
    /// <param name="message">The message text.</param>
    /// <param name="success"><c>true</c> for green; <c>false</c> for red.</param>
    private void ShowStatus(string message, bool success)
    {
        lblStatus.Text      = message;
        lblStatus.ForeColor = success ? Color.FromArgb(40, 167, 69) : Color.Crimson;
        lblStatus.Visible   = true;
    }
}
