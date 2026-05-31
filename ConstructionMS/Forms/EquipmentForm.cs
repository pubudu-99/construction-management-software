using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR5 — Vehicle and Equipment Management form.
/// Allows adding new equipment and managing equipment status via right-click context menu.
/// </summary>
public partial class EquipmentForm : Form
{
    private readonly EquipmentService    _service;
    private readonly EquipmentRepository _repo;

    /// <summary>Initialises the form with the given connection factory.</summary>
    public EquipmentForm(DbConnectionFactory factory)
    {
        _repo    = new EquipmentRepository(factory);
        _service = new EquipmentService(factory, _repo);
        InitializeComponent();
        GridStyle.Apply(dgvEquipment);
        Theme.Apply(this);
        Load += EquipmentForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Loads the equipment list when the form is first shown.</summary>
    private void EquipmentForm_Load(object? sender, EventArgs e) => LoadEquipment();

    // ── Data loading ──────────────────────────────────────────────────────────

    /// <summary>Loads all equipment records from the database into the grid.</summary>
    private void LoadEquipment()
    {
        var today = DateTime.Today;
        var rows = _repo.GetAll()
            .Select(e => new
            {
                e.EquipmentId,
                e.Name,
                e.Type,
                e.Status,
                CurrentSite      = e.CurrentSite ?? "—",
                LastMaintenance  = e.LastMaintenance.HasValue
                    ? e.LastMaintenance.Value.ToString("dd MMM yyyy") : "—",
                NextMaintenance  = e.NextMaintenance.HasValue
                    ? e.NextMaintenance.Value.ToString("dd MMM yyyy") : "—",
                DaysUntilMaint   = e.NextMaintenance.HasValue
                    ? (e.NextMaintenance.Value.Date - today).Days : 999
            })
            .ToList();

        dgvEquipment.DataSource = rows;

        // Customise columns only once (first load auto-generates them).
        if (dgvEquipment.Columns.Count == 0) return;

        // Hide technical columns.
        SetColVisible("EquipmentId",    false);
        SetColVisible("DaysUntilMaint", false);

        // Friendly header labels.
        SetColHeader("Name",            "Equipment Name");
        SetColHeader("Type",            "Type");
        SetColHeader("Status",          "Status");
        SetColHeader("CurrentSite",     "Current Site");
        SetColHeader("LastMaintenance", "Last Maint.");
        SetColHeader("NextMaintenance", "Next Maint.");

        // Proportional widths via FillWeight (safe in Fill mode).
        SetColWeight("Name",            200);
        SetColWeight("Type",            140);
        SetColWeight("Status",          145);
        SetColWeight("CurrentSite",     160);
        SetColWeight("LastMaintenance", 130);
        SetColWeight("NextMaintenance", 130);
    }

    // ── Entry panel events ────────────────────────────────────────────────────

    /// <summary>Validates entry fields and inserts a new equipment record.</summary>
    private void BtnSaveEquip_Click(object sender, EventArgs e)
    {
        var equipment = new Equipment
        {
            Name            = txtEquipName.Text.Trim(),
            Type            = txtEquipType.Text.Trim(),
            Status          = cmbEquipStatus.SelectedItem?.ToString() ?? "Available",
            CurrentSite     = txtCurrentSite.Text.Trim() is { Length: > 0 } s ? s : null,
            LastMaintenance = dtpLastMaint.Checked ? dtpLastMaint.Value.Date : null,
            NextMaintenance = dtpNextMaint.Checked ? dtpNextMaint.Value.Date : null
        };

        var result = _service.AddEquipment(equipment);

        if (!result.Success)
        {
            ShowStatus(result.Message, success: false);
            return;
        }

        txtEquipName.Clear();
        txtEquipType.Clear();
        cmbEquipStatus.SelectedIndex = 0;
        txtCurrentSite.Clear();
        dtpLastMaint.Checked = false;
        dtpNextMaint.Checked = false;

        ShowStatus("Equipment saved successfully.", success: true);
        LoadEquipment();
    }

    // ── Grid events ───────────────────────────────────────────────────────────

    /// <summary>
    /// Applies status-based row colouring to the equipment grid.
    /// Priority: Under Maintenance → grey; overdue → red; due soon → amber.
    /// Warning window is read from App.config (key: MaintenanceWarningDays).
    /// </summary>
    private void DgvEquipment_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvEquipment.Rows[e.RowIndex].DataBoundItem is null) return;
        if (e.CellStyle is not { } cs) return;

        var row    = dgvEquipment.Rows[e.RowIndex];
        var status = row.Cells["Status"]?.Value?.ToString() ?? "";

        // Under Maintenance always gets grey, regardless of dates.
        if (status == "Under Maintenance")
        {
            cs.BackColor          = Color.FromArgb(220, 220, 220);
            cs.ForeColor          = GridStyle.StatusBlack;
            cs.SelectionBackColor = Color.FromArgb(190, 190, 190);
            cs.SelectionForeColor = GridStyle.StatusBlack;
            return;
        }

        // Parse the visible NextMaintenance date string ("dd MMM yyyy" or "—").
        string nextMaintStr = row.Cells["NextMaintenance"]?.Value?.ToString() ?? "—";
        if (nextMaintStr == "—" || string.IsNullOrWhiteSpace(nextMaintStr)) return;

        if (!DateTime.TryParse(nextMaintStr, out DateTime nextMaint)) return;

        int days        = (nextMaint.Date - DateTime.Today).Days;
        int warningDays = AppConfig.MaintenanceWarningDays;

        if (days <= 0)
        {
            // Overdue maintenance
            cs.BackColor          = GridStyle.StatusRed;
            cs.ForeColor          = GridStyle.StatusWhite;
            cs.SelectionBackColor = GridStyle.StatusRedSel;
            cs.SelectionForeColor = GridStyle.StatusWhite;
        }
        else if (days <= warningDays)
        {
            // Due soon
            cs.BackColor          = GridStyle.StatusAmber;
            cs.ForeColor          = GridStyle.StatusBlack;
            cs.SelectionBackColor = GridStyle.StatusAmberSel;
            cs.SelectionForeColor = GridStyle.StatusBlack;
        }
    }

    /// <summary>Opens the Set Status dialog when the right-click menu item is clicked.</summary>
    private void MnuSetStatus_Click(object sender, EventArgs e)
    {
        if (dgvEquipment.CurrentRow?.Cells["EquipmentId"]?.Value is not int equipId) return;

        var currentStatus = dgvEquipment.CurrentRow.Cells["Status"]?.Value?.ToString()
                            ?? "Available";

        using var dlg = BuildSetStatusDialog(currentStatus);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var cmbNew    = (ComboBox)dlg.Controls["cmbNewStatus"]!;
        var txtSite   = (TextBox)dlg.Controls["txtSite"]!;
        var newStatus = cmbNew.SelectedItem?.ToString() ?? "Available";
        var siteName  = txtSite.Text.Trim() is { Length: > 0 } sn ? sn : null;

        var result = _service.UpdateStatus(equipId, newStatus, siteName);
        if (!result.Success)
        {
            MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        LoadEquipment();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowStatus(string message, bool success)
    {
        lblEquipStatus.Text      = message;
        lblEquipStatus.ForeColor = success
            ? Color.FromArgb(40, 167, 69)
            : Color.FromArgb(220, 53, 69);
        lblEquipStatus.Visible   = true;

        var timer = new System.Windows.Forms.Timer { Interval = 3000 };
        timer.Tick += (_, _) =>
        {
            lblEquipStatus.Visible = false;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    private void SetColVisible(string name, bool visible)
    {
        if (dgvEquipment.Columns[name] is { } col) col.Visible = visible;
    }

    private void SetColHeader(string name, string header)
    {
        if (dgvEquipment.Columns[name] is { } col) col.HeaderText = header;
    }

    private void SetColWeight(string name, int weight)
    {
        if (dgvEquipment.Columns[name] is { } col) col.FillWeight = weight;
    }

    /// <summary>Builds an inline dialog for changing an equipment item's status.</summary>
    private static Form BuildSetStatusDialog(string currentStatus)
    {
        var dlg = new Form
        {
            Text            = "Set Equipment Status",
            ClientSize      = new Size(320, 200),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition   = FormStartPosition.CenterParent,
            MaximizeBox     = false,
            MinimizeBox     = false,
            Font            = new Font("Segoe UI", 9F)
        };

        var lblStatus = new Label
        {
            Text     = "New Status",
            Font     = new Font("Segoe UI", 9F, FontStyle.Bold),
            Location = new Point(16, 16),
            Size     = new Size(288, 18)
        };

        var cmbNewStatus = new ComboBox
        {
            Name          = "cmbNewStatus",
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9.5F),
            Location      = new Point(16, 36),
            Size          = new Size(288, 26)
        };
        cmbNewStatus.Items.AddRange(new object[] { "Available", "In Use", "Under Maintenance" });
        cmbNewStatus.SelectedItem = currentStatus;
        if (cmbNewStatus.SelectedIndex < 0) cmbNewStatus.SelectedIndex = 0;

        var lblSite = new Label
        {
            Text     = "Current Site  (required when In Use)",
            Font     = new Font("Segoe UI", 9F, FontStyle.Bold),
            Location = new Point(16, 76),
            Size     = new Size(288, 18),
            Visible  = currentStatus == "In Use"
        };

        var txtSite = new TextBox
        {
            Name            = "txtSite",
            Font            = new Font("Segoe UI", 9.5F),
            Location        = new Point(16, 96),
            Size            = new Size(288, 26),
            PlaceholderText = "e.g. Site A, Head Office",
            Visible         = currentStatus == "In Use"
        };

        cmbNewStatus.SelectedIndexChanged += (_, _) =>
        {
            bool inUse      = cmbNewStatus.SelectedItem?.ToString() == "In Use";
            lblSite.Visible = inUse;
            txtSite.Visible = inUse;
        };

        var btnConfirm = new Button
        {
            Text         = "Confirm",
            Font         = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            BackColor    = Color.FromArgb(40, 167, 69),
            ForeColor    = Color.White,
            FlatStyle    = FlatStyle.Flat,
            Location     = new Point(16, 152),
            Size         = new Size(120, 34),
            DialogResult = DialogResult.OK,
            Cursor       = Cursors.Hand
        };
        btnConfirm.FlatAppearance.BorderSize = 0;

        var btnCancel = new Button
        {
            Text         = "Cancel",
            Font         = new Font("Segoe UI", 9.5F),
            Location     = new Point(152, 152),
            Size         = new Size(90, 34),
            DialogResult = DialogResult.Cancel,
            Cursor       = Cursors.Hand
        };

        dlg.Controls.AddRange(new Control[]
            { lblStatus, cmbNewStatus, lblSite, txtSite, btnConfirm, btnCancel });
        dlg.AcceptButton = btnConfirm;
        dlg.CancelButton = btnCancel;

        return dlg;
    }
}
