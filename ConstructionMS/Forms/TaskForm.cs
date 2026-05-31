using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR3 — Time Scheduling form.
/// Top panel: create a new task. Bottom grid: view and manage existing tasks.
/// Rows are colour-coded by proximity to deadline.
/// </summary>
public partial class TaskForm : Form
{
    private readonly DbConnectionFactory _factory;
    private readonly TaskRepository      _taskRepo;
    private readonly ProjectRepository   _projectRepo;
    private readonly ScheduleService     _scheduleService;
    private readonly UserRepository      _userRepo;

    private int _projectId;

    /// <summary>Initialises the form and builds the service graph.</summary>
    public TaskForm(DbConnectionFactory factory)
    {
        _factory         = factory;
        _taskRepo        = new TaskRepository(factory);
        _projectRepo     = new ProjectRepository(factory);
        _userRepo        = new UserRepository(factory);
        _scheduleService = new ScheduleService(factory, _taskRepo);

        InitializeComponent();
        GridStyle.Apply(dgvTasks);

        // FR1.3 — Role-based access: only Managers may create tasks.
        if (!Session.IsManager)
        {
            btnSaveTask.Enabled = false;
            ShowStatus("Manager access required for creating tasks.", success: false);
        }

        Load += TaskForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    private void TaskForm_Load(object? sender, EventArgs e)
    {
        var project = _projectRepo.GetFirst();
        if (project is null)
        {
            MessageBox.Show("No project found. Please add a project first.",
                "No Project", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Close();
            return;
        }
        _projectId = project.ProjectId;

        LoadAssignees();
        LoadTasks();
    }

    /// <summary>Populates the assignee ComboBox with active users.</summary>
    private void LoadAssignees()
    {
        cmbAssignee.Items.Clear();
        cmbAssignee.Items.Add(new ComboItem("(None)", 0));

        // Load users directly — UserRepository already has FindByUsername;
        // we query all active users with a raw call here.
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "SELECT UserId, FullName FROM Users WHERE IsActive=1 ORDER BY FullName;";
        using var rd   = cmd.ExecuteReader();
        while (rd.Read())
            cmbAssignee.Items.Add(new ComboItem(rd.GetString(1), rd.GetInt32(0)));

        cmbAssignee.SelectedIndex = 0;
    }

    // ── Save task ─────────────────────────────────────────────────────────────

    /// <summary>Validates input and creates the task via ScheduleService.</summary>
    private void BtnSaveTask_Click(object sender, EventArgs e)
    {
        lblTaskStatus.Visible = false;

        int? assigneeId = null;
        if (cmbAssignee.SelectedItem is ComboItem ci && ci.Id != 0)
            assigneeId = ci.Id;

        var task = new ProjectTask
        {
            ProjectId  = _projectId,
            Name       = txtTaskName.Text.Trim(),
            StartDate  = dtpStartDate.Value.Date,
            EndDate    = dtpEndDate.Value.Date,
            AssigneeId = assigneeId,
            Status     = "Open"
        };

        var result = _scheduleService.CreateTask(task);

        if (!result.Success)
        {
            ShowStatus(result.Message, success: false);
            return;
        }

        ShowStatus("Task added.", success: true);
        txtTaskName.Clear();
        dtpStartDate.Value = DateTime.Today;
        dtpEndDate.Value   = DateTime.Today.AddDays(7);
        cmbAssignee.SelectedIndex = 0;
        LoadTasks();
    }

    // ── Grid ──────────────────────────────────────────────────────────────────

    /// <summary>Loads tasks for the current project and binds the grid.</summary>
    private void LoadTasks()
    {
        var tasks = _taskRepo.GetByProject(_projectId);
        var now   = DateTime.Now;

        dgvTasks.DataSource = tasks.Select(t => new
        {
            t.TaskId,
            t.Name,
            Start     = t.StartDate.ToString("yyyy-MM-dd"),
            End       = t.EndDate.ToString("yyyy-MM-dd"),
            Status    = t.Status,
            Days      = (t.EndDate.Date - DateTime.Today).Days,   // hidden — drives colour
            Remaining = FormatRemaining(t.EndDate.Date, now)       // visible — human-readable
        }).ToList();

        // Hide internal columns
        if (dgvTasks.Columns["TaskId"] is { } colId)   colId.Visible   = false;
        if (dgvTasks.Columns["Days"]   is { } colDays) colDays.Visible = false;

        // Right-align the Remaining column
        if (dgvTasks.Columns["Remaining"] is { } colRem)
            colRem.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
    }

    /// <summary>
    /// Returns a concise human-readable countdown to the end of <paramref name="endDate"/>.
    /// Overdue values are prefixed with "-".
    /// Examples: "3d 4h", "5h 20m", "45m", "-2d 1h".
    /// </summary>
    private static string FormatRemaining(DateTime endDate, DateTime now)
    {
        // Deadline = midnight at the end of EndDate (start of the following day)
        var deadline  = endDate.Date.AddDays(1);
        var remaining = deadline - now;

        if (remaining.TotalSeconds <= 0)
        {
            // Overdue — negate to get a positive span then prefix with "-"
            var over = now - deadline;
            if (over.TotalDays >= 1)
                return $"-{(int)over.TotalDays}d {over.Hours}h";
            if (over.TotalHours >= 1)
                return $"-{(int)over.TotalHours}h {over.Minutes}m";
            return $"-{over.Minutes}m";
        }

        if (remaining.TotalDays >= 1)
            return $"{(int)remaining.TotalDays}d {remaining.Hours}h";
        if (remaining.TotalHours >= 1)
            return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
        return $"{remaining.Minutes}m";
    }

    /// <summary>
    /// Colours rows by status and deadline urgency using the shared GridStyle palette.
    /// Complete (any deadline) = green  |  Overdue/today = red  |  1–3 days = amber  |  else default.
    /// </summary>
    private void DgvTasks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvTasks.Rows[e.RowIndex].DataBoundItem is null) return;
        if (e.CellStyle is not { } cs) return;   // CellStyle is nullable in .NET 8 annotations

        var row    = dgvTasks.Rows[e.RowIndex];
        var status = row.Cells["Status"]?.Value?.ToString() ?? "";
        int days   = row.Cells["Days"]?.Value is int d ? d : 999;

        // Complete always wins — a finished task is green regardless of deadline
        if (status == "Complete")
        {
            cs.BackColor          = GridStyle.StatusGreen;
            cs.ForeColor          = GridStyle.StatusWhite;
            cs.SelectionBackColor = GridStyle.StatusGreenSel;
            cs.SelectionForeColor = GridStyle.StatusWhite;
        }
        else if (days <= 0)
        {
            cs.BackColor          = GridStyle.StatusRed;
            cs.ForeColor          = GridStyle.StatusWhite;
            cs.SelectionBackColor = GridStyle.StatusRedSel;
            cs.SelectionForeColor = GridStyle.StatusWhite;
        }
        else if (days <= 3)
        {
            cs.BackColor          = GridStyle.StatusAmber;
            cs.ForeColor          = GridStyle.StatusBlack;
            cs.SelectionBackColor = GridStyle.StatusAmberSel;
            cs.SelectionForeColor = GridStyle.StatusBlack;
        }
        // days > 3: leave default colours supplied by GridStyle.Apply()
    }

    // ── Context menu ──────────────────────────────────────────────────────────

    /// <summary>Marks the selected task as Complete.</summary>
    private void MenuMarkComplete_Click(object? sender, EventArgs e)
    {
        if (!TryGetSelectedTaskId(out int id)) return;
        _taskRepo.UpdateStatus(id, "Complete");
        LoadTasks();
    }

    /// <summary>Confirms then reloads (delete wired in a later module).</summary>
    private void MenuDelete_Click(object? sender, EventArgs e)
    {
        if (!TryGetSelectedTaskId(out _)) return;
        var confirm = MessageBox.Show(
            "Delete this task?", "Confirm Delete",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm == DialogResult.Yes)
            LoadTasks();
    }

    private bool TryGetSelectedTaskId(out int id)
    {
        id = 0;
        if (dgvTasks.CurrentRow is null) return false;
        var cell = dgvTasks.CurrentRow.Cells["TaskId"];
        if (cell?.Value is int v) { id = v; return true; }
        return false;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowStatus(string message, bool success)
    {
        lblTaskStatus.Text      = message;
        lblTaskStatus.ForeColor = success ? Color.FromArgb(40, 167, 69) : Color.Crimson;
        lblTaskStatus.Visible   = true;
    }

    /// <summary>Simple wrapper so ComboBox items carry both display text and ID.</summary>
    private sealed class ComboItem
    {
        public string Text { get; }
        public int    Id   { get; }
        public ComboItem(string text, int id) { Text = text; Id = id; }
        public override string ToString() => Text;
    }
}
