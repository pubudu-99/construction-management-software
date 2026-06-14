using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// Worker (labour) management. Managers only. Lists all workers and allows
/// adding/editing them (via <see cref="WorkerDialog"/>) and toggling their
/// active status. Inactive workers are excluded from the attendance dropdown.
/// </summary>
public partial class WorkerManagementForm : Form
{
    private readonly DbConnectionFactory _factory;
    private readonly WorkerService       _service;

    /// <summary>Initialises the form with database access.</summary>
    public WorkerManagementForm(DbConnectionFactory factory)
    {
        _factory = factory;
        _service = new WorkerService(factory, new WorkerRepository(factory));
        InitializeComponent();
        GridStyle.Apply(dgvWorkers);
        Theme.Apply(this);
        Load += WorkerManagementForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Enforces Manager-only access then loads the worker list.</summary>
    private void WorkerManagementForm_Load(object? sender, EventArgs e)
    {
        if (!Session.IsManager)
        {
            MessageBox.Show(
                "Access denied. Only Managers can open Worker Management.",
                "Access Denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            BeginInvoke((Action)Close);
            return;
        }

        LoadWorkers();
    }

    // ── Data loading ──────────────────────────────────────────────────────────

    /// <summary>Loads all workers from the database into the grid.</summary>
    private void LoadWorkers()
    {
        var rows = _service.GetAll()
            .Select(w => new
            {
                w.WorkerId,
                w.Name,
                NIC    = w.NIC ?? "",
                Phone  = w.Phone ?? "",
                Rate   = w.HourlyRate,
                Active = w.IsActive ? "Yes" : "No"
            })
            .ToList();

        dgvWorkers.DataSource = rows;

        if (dgvWorkers.Columns.Count == 0) return;

        SetColVisible("WorkerId", false);
        SetColHeader("Name",  "Name");
        SetColHeader("NIC",   "NIC");
        SetColHeader("Phone", "Phone");
        SetColHeader("Rate",  "Hourly Rate");
        SetColHeader("Active", "Active");

        SetColWeight("Name",   220);
        SetColWeight("NIC",    140);
        SetColWeight("Phone",  130);
        SetColWeight("Rate",   100);
        SetColWeight("Active", 70);

        if (dgvWorkers.Columns["Rate"] is { } colRate)
        {
            colRate.DefaultCellStyle.Format    = "N2";
            colRate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
    }

    /// <summary>Greys out inactive workers in the grid.</summary>
    private void DgvWorkers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvWorkers.Rows[e.RowIndex].DataBoundItem is null) return;
        if (e.CellStyle is not { } cs) return;

        var active = dgvWorkers.Rows[e.RowIndex].Cells["Active"]?.Value?.ToString();
        if (active == "No")
        {
            cs.ForeColor          = Color.FromArgb(140, 140, 140);
            cs.SelectionForeColor = Color.FromArgb(70, 70, 70);
        }
    }

    // ── Toolbar events ──────────────────────────────────────────────────────────

    /// <summary>Opens the Add Worker dialog.</summary>
    private void BtnAddWorker_Click(object sender, EventArgs e)
    {
        using var dlg = new WorkerDialog(_factory);
        if (dlg.ShowDialog(this) == DialogResult.OK)
            LoadWorkers();
    }

    /// <summary>Opens the Edit Worker dialog for the selected row.</summary>
    private void BtnEditWorker_Click(object sender, EventArgs e) => EditSelected();

    /// <summary>Edits the worker when a grid row is double-clicked.</summary>
    private void DgvWorkers_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        EditSelected();
    }

    /// <summary>Opens the Edit Worker dialog for the currently selected worker.</summary>
    private void EditSelected()
    {
        if (dgvWorkers.CurrentRow?.Cells["WorkerId"]?.Value is not int id) return;

        var worker = _service.GetAll().FirstOrDefault(w => w.WorkerId == id);
        if (worker is null) return;

        using var dlg = new WorkerDialog(_factory, worker);
        if (dlg.ShowDialog(this) == DialogResult.OK)
            LoadWorkers();
    }

    // ── Context menu ──────────────────────────────────────────────────────────

    /// <summary>Updates the context menu's toggle label based on the row state.</summary>
    private void CtxWorkerMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (dgvWorkers.CurrentRow?.Cells["Active"]?.Value is not string active)
        {
            e.Cancel = true;
            return;
        }
        mnuToggleActive.Text = active == "Yes" ? "Deactivate Worker" : "Activate Worker";
    }

    /// <summary>Toggles the IsActive flag on the selected worker.</summary>
    private void MnuToggleActive_Click(object sender, EventArgs e)
    {
        if (dgvWorkers.CurrentRow?.Cells["WorkerId"]?.Value is not int id) return;
        if (dgvWorkers.CurrentRow.Cells["Active"]?.Value is not string active) return;

        bool makeActive = active == "No";
        var result = _service.SetActive(id, makeActive);
        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        LoadWorkers();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetColVisible(string n, bool v) { if (dgvWorkers.Columns[n] is { } c) c.Visible    = v; }
    private void SetColHeader(string n, string h) { if (dgvWorkers.Columns[n] is { } c) c.HeaderText = h; }
    private void SetColWeight(string n, int w)    { if (dgvWorkers.Columns[n] is { } c) c.FillWeight = w; }
}
