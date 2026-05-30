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
    }

    // ── Worker combo ─────────────────────────────────────────────────────────

    /// <summary>Populates the worker drop-down with all active workers.</summary>
    private void LoadWorkers()
    {
        int? selectedId = (cmbWorker.SelectedItem as ComboItem)?.Id;
        cmbWorker.Items.Clear();

        foreach (var w in _workerRepo.GetActive())
            cmbWorker.Items.Add(new ComboItem(w.Name, w.WorkerId));

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

    // ── Add Worker ───────────────────────────────────────────────────────────

    /// <summary>Shows a mini-dialog to register a new worker, then reloads the drop-down.</summary>
    private void BtnAddWorker_Click(object? sender, EventArgs e)
    {
        using var dlg = BuildAddWorkerDialog();
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var txtName = (TextBox)dlg.Controls["txtName"]!;
        var numRate = (NumericUpDown)dlg.Controls["numRate"]!;

        var name = txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Worker name is required.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int newId = _workerRepo.Insert(new Worker
        {
            Name       = name,
            HourlyRate = numRate.Value,
            IsActive   = true
        });

        LoadWorkers();

        // Select the newly created worker in the combo
        for (int i = 0; i < cmbWorker.Items.Count; i++)
        {
            if (cmbWorker.Items[i] is ComboItem ci && ci.Id == newId)
            {
                cmbWorker.SelectedIndex = i;
                break;
            }
        }
    }

    /// <summary>Builds and returns the "Add Worker" mini-form.</summary>
    private static Form BuildAddWorkerDialog()
    {
        var dlg = new Form
        {
            Text            = "Add Worker",
            ClientSize      = new Size(340, 168),
            StartPosition   = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox     = false,
            MinimizeBox     = false,
            Font            = new Font("Segoe UI", 9F)
        };

        var lblName = new Label
        {
            Text      = "Full Name:",
            Location  = new Point(12, 18),
            Size      = new Size(112, 20),
            TextAlign = ContentAlignment.MiddleLeft
        };
        var txtName = new TextBox
        {
            Name     = "txtName",
            Location = new Point(128, 14),
            Size     = new Size(194, 24)
        };

        var lblRate = new Label
        {
            Text      = "Hourly Rate (LKR):",
            Location  = new Point(12, 56),
            Size      = new Size(112, 20),
            TextAlign = ContentAlignment.MiddleLeft
        };
        var numRate = new NumericUpDown
        {
            Name          = "numRate",
            Location      = new Point(128, 52),
            Size          = new Size(194, 24),
            Minimum       = 1,
            Maximum       = 999999,
            DecimalPlaces = 2,
            Value         = 250
        };

        var btnOk = new Button
        {
            Text         = "Add",
            DialogResult = DialogResult.OK,
            Location     = new Point(144, 116),
            Size         = new Size(82, 30),
            BackColor    = Color.FromArgb(40, 167, 69),
            ForeColor    = Color.White,
            FlatStyle    = FlatStyle.Flat
        };
        btnOk.FlatAppearance.BorderSize = 0;

        var btnCancel = new Button
        {
            Text         = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location     = new Point(240, 116),
            Size         = new Size(82, 30)
        };

        dlg.Controls.AddRange(new Control[] { lblName, txtName, lblRate, numRate, btnOk, btnCancel });
        dlg.AcceptButton = btnOk;
        dlg.CancelButton = btnCancel;
        return dlg;
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
