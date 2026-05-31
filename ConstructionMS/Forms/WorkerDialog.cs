using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Modal dialog for adding or editing a worker. Validation (name, rate, NIC
/// format and uniqueness) is performed by <see cref="WorkerService"/>.
/// Returns <see cref="DialogResult.OK"/> when the worker is saved.
/// </summary>
public partial class WorkerDialog : Form
{
    private readonly WorkerService _service;
    private readonly Worker?       _editing;

    /// <summary>The WorkerId saved by this dialog (new or edited), 0 until saved.</summary>
    public int SavedWorkerId { get; private set; }

    /// <summary>
    /// Initialises the dialog. Pass an existing worker to edit it, or null to
    /// add a new one.
    /// </summary>
    /// <param name="factory">Connection factory for database access.</param>
    /// <param name="existing">The worker to edit, or null to add a new worker.</param>
    public WorkerDialog(DbConnectionFactory factory, Worker? existing = null)
    {
        _service = new WorkerService(factory, new WorkerRepository(factory));
        _editing = existing;

        InitializeComponent();
        Theme.Apply(this);

        if (_editing is not null)
        {
            lblTitle.Text   = "Edit Worker";
            Text            = "Edit Worker";
            btnSave.Text    = "Save Changes";
            txtName.Text    = _editing.Name;
            txtNic.Text     = _editing.NIC   ?? "";
            txtPhone.Text   = _editing.Phone ?? "";
            numRate.Value   = ClampRate(_editing.HourlyRate);
            chkActive.Checked = _editing.IsActive;
        }
    }

    /// <summary>Validates input via the service and saves the worker.</summary>
    private void BtnSave_Click(object sender, EventArgs e)
    {
        var worker = new Worker
        {
            WorkerId   = _editing?.WorkerId ?? 0,
            Name       = txtName.Text.Trim(),
            NIC        = txtNic.Text.Trim(),
            Phone      = txtPhone.Text.Trim(),
            HourlyRate = numRate.Value,
            IsActive   = chkActive.Checked
        };

        WorkerResult result;
        if (_editing is null)
        {
            result = _service.Create(worker, out int newId);
            if (result.Success) SavedWorkerId = newId;
        }
        else
        {
            result = _service.Update(worker);
            if (result.Success) SavedWorkerId = worker.WorkerId;
        }

        if (!result.Success)
        {
            ShowError(result.Message);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>Closes the dialog without saving.</summary>
    private void BtnCancel_Click(object sender, EventArgs e) => Close();

    /// <summary>Shows a red inline error message.</summary>
    private void ShowError(string message)
    {
        lblStatus.Text      = message;
        lblStatus.ForeColor = Color.FromArgb(220, 53, 69);
        lblStatus.Visible   = true;
    }

    /// <summary>Keeps a stored rate within the NumericUpDown's accepted range.</summary>
    private decimal ClampRate(decimal rate) =>
        Math.Min(numRate.Maximum, Math.Max(numRate.Minimum, rate));

    /// <summary>Defaults the dialog result to Cancel if closed without saving.</summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (DialogResult == DialogResult.None)
            DialogResult = DialogResult.Cancel;
        base.OnFormClosing(e);
    }
}
