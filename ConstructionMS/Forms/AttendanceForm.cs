using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Forms;

/// <summary>
/// FR4 — Records daily worker attendance and displays attendance history.
/// </summary>
public partial class AttendanceForm : Form
{
    private readonly WorkerRepository     _workerRepo;
    private readonly AttendanceRepository _attendanceRepo;

    /// <summary>Initialises the form with database repositories and loads initial data.</summary>
    public AttendanceForm(DbConnectionFactory factory)
    {
        InitializeComponent();
        GridStyle.Apply(dgvAttendance);
        Theme.Apply(this);
        _workerRepo     = new WorkerRepository(factory);
        _attendanceRepo = new AttendanceRepository(factory);
        LoadWorkers();
        LoadHistory();
    }

    // ── Inner helper ─────────────────────────────────────────────────────────
    private sealed class ComboItem
    {
        public string Text { get; }
        public int    Id   { get; }
        public ComboItem(string text, int id) { Text = text; Id = id; }
        public override string ToString() => Text;

        /// <summary>Builds the display text "Name (NIC: xxx)", or just "Name" when no NIC.</summary>
        public static ComboItem For(Worker w) =>
            new(string.IsNullOrWhiteSpace(w.NIC) ? w.Name : $"{w.Name} (NIC: {w.NIC})", w.WorkerId);
    }

    // ── Worker combo ─────────────────────────────────────────────────────────

    /// <summary>Populates the worker drop-down with all active workers.</summary>
    private void LoadWorkers()
    {
        int? selectedId = (cmbWorker.SelectedItem as ComboItem)?.Id;
        cmbWorker.Items.Clear();

        foreach (var w in _workerRepo.GetActive())
            cmbWorker.Items.Add(ComboItem.For(w));

        // Re-select previously chosen worker if still present
        if (selectedId.HasValue)
        {
            for (int i = 0; i < cmbWorker.Items.Count; i++)
            {
                if (cmbWorker.Items[i] is ComboItem ci && ci.Id == selectedId.Value)
                {
                    cmbWorker.SelectedIndex = i;
                    return;
                }
            }
        }

        if (cmbWorker.Items.Count > 0) cmbWorker.SelectedIndex = 0;
    }

    // ── Save attendance ──────────────────────────────────────────────────────

    /// <summary>Validates input, checks for duplicates, and inserts a new attendance record.</summary>
    private void BtnSaveAttendance_Click(object? sender, EventArgs e)
    {
        if (cmbWorker.SelectedItem is not ComboItem selected)
        {
            ShowStatus("Please select a worker.", success: false);
            return;
        }

        var date  = dtpAttDate.Value.Date;
        var hours = numHours.Value;

        if (_attendanceRepo.ExistsForWorkerOnDate(selected.Id, date))
        {
            ShowStatus(
                $"Attendance for {selected.Text} on {date:dd MMM yyyy} is already recorded.",
                success: false);
            return;
        }

        _attendanceRepo.Insert(new Attendance
        {
            WorkerId = selected.Id,
            Date     = date,
            Hours    = hours
        });

        ShowStatus($"Saved — {selected.Text}, {date:dd MMM yyyy}, {hours:0.#} h", success: true);
        LoadHistory();
    }

    // ── History ──────────────────────────────────────────────────────────────

    /// <summary>Reloads the attendance history grid for the selected date range.</summary>
    private void BtnRefreshHist_Click(object? sender, EventArgs e) => LoadHistory();

    /// <summary>
    /// Fetches attendance for all active workers within the chosen date range
    /// and binds the results to the history grid, sorted by date then worker name.
    /// </summary>
    private void LoadHistory()
    {
        var from = dtpHistFrom.Value.Date;
        var to   = dtpHistTo.Value.Date;

        if (from > to)
        {
            dgvAttendance.DataSource = null;
            return;
        }

        var workers = _workerRepo.GetActive();

        // Collect all attendance rows across every active worker
        var rows = workers
            .SelectMany(w => _attendanceRepo.GetForWorker(w.WorkerId, from, to)
                .Select(a => new { w.Name, a.Date, a.Hours }))
            .OrderBy(r => r.Date)
            .ThenBy(r => r.Name)
            .Select(r => new
            {
                Worker = r.Name,
                Date   = r.Date.ToString("dd MMM yyyy"),
                Hours  = r.Hours
            })
            .ToList();

        dgvAttendance.DataSource = rows;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowStatus(string message, bool success)
    {
        lblAttStatus.Text      = message;
        lblAttStatus.ForeColor = success ? Color.FromArgb(40, 167, 69) : Color.Crimson;
        lblAttStatus.Visible   = true;
    }
}
