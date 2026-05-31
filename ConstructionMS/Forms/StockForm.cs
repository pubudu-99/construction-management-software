using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR6 — Materials and Stock Management form.
/// Catalogue tab: add materials and view stock levels.
/// Stock Movement tab: record in/out movements and view history for a selected material.
/// </summary>
public partial class StockForm : Form
{
    private readonly InventoryService        _service;
    private readonly MaterialRepository      _matRepo;
    private readonly StockMovementRepository _movRepo;

    private sealed class ComboItem
    {
        public int    Id   { get; }
        public string Text { get; }
        public ComboItem(int id, string text) { Id = id; Text = text; }
        public override string ToString() => Text;
    }

    /// <summary>Initialises the form with repositories.</summary>
    public StockForm(MaterialRepository matRepo, StockMovementRepository movRepo)
    {
        _matRepo = matRepo;
        _movRepo = movRepo;
        _service = new InventoryService(matRepo, movRepo);
        InitializeComponent();
        GridStyle.Apply(dgvMaterials);
        GridStyle.Apply(dgvMovements);
        Theme.Apply(this);
        Load += StockForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Loads the catalogue when the form is first shown.</summary>
    private void StockForm_Load(object? sender, EventArgs e) => LoadCatalogue();

    // ── Catalogue tab ─────────────────────────────────────────────────────────

    /// <summary>Loads all materials into the catalogue grid and refreshes the movement ComboBox.</summary>
    private void LoadCatalogue()
    {
        var rows = _matRepo.GetAll()
            .Select(m => new
            {
                m.MaterialId,
                m.Name,
                m.Unit,
                Stock        = m.Stock,
                ReorderPoint = m.ReorderPoint
            })
            .ToList();

        dgvMaterials.DataSource = rows;

        if (dgvMaterials.Columns.Count > 0)
        {
            SetMatColVisible("MaterialId", false);
            SetMatColHeader("Name",        "Material");
            SetMatColHeader("Unit",        "Unit");
            SetMatColHeader("Stock",       "Current Stock");
            SetMatColHeader("ReorderPoint", "Reorder Point");

            SetMatColWeight("Name",        240);
            SetMatColWeight("Unit",        100);
            SetMatColWeight("Stock",       130);
            SetMatColWeight("ReorderPoint", 130);

            SetMatColFormat("Stock",        "0.##");
            SetMatColFormat("ReorderPoint", "0.##");
            SetMatColAlign("Stock",        DataGridViewContentAlignment.MiddleRight);
            SetMatColAlign("ReorderPoint", DataGridViewContentAlignment.MiddleRight);
        }

        PopulateMovementCombo();
    }

    /// <summary>Validates entry fields and adds a new material to the catalogue.</summary>
    private void BtnAddMaterial_Click(object sender, EventArgs e)
    {
        var material = new Material
        {
            Name         = txtMatName.Text.Trim(),
            Unit         = txtUnit.Text.Trim(),
            Stock        = numInitStock.Value,
            ReorderPoint = numReorder.Value
        };

        var result = _service.AddMaterial(material);

        if (!result.Success)
        {
            ShowCatStatus(result.Message, success: false);
            return;
        }

        txtMatName.Clear();
        txtUnit.Clear();
        numInitStock.Value = 0;
        numReorder.Value   = 0;

        ShowCatStatus("Material added successfully.", success: true);
        LoadCatalogue();
    }

    /// <summary>
    /// Applies status-based row colouring to the catalogue grid using the same
    /// thresholds as the Reports → Stock Status tab so colours mean the same thing
    /// everywhere: Out of Stock / Low Stock = red, Getting Low = amber, OK = normal.
    /// The "Getting Low" buffer comes from App.config (LowStockWarningBuffer).
    /// </summary>
    private void DgvMaterials_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || dgvMaterials.Rows[e.RowIndex].DataBoundItem is null) return;
        if (e.CellStyle is not { } cs) return;

        var row        = dgvMaterials.Rows[e.RowIndex];
        var stockVal   = row.Cells["Stock"]?.Value;
        var reorderVal = row.Cells["ReorderPoint"]?.Value;

        if (stockVal is not decimal stock || reorderVal is not decimal reorder)
            return;

        decimal buffer = AppConfig.LowStockWarningBuffer;

        // Out of Stock or at/below reorder → red (urgent).
        if (stock == 0 || (reorder > 0 && stock <= reorder))
        {
            cs.BackColor          = GridStyle.StatusRed;
            cs.ForeColor          = GridStyle.StatusWhite;
            cs.SelectionBackColor = GridStyle.StatusRedSel;
            cs.SelectionForeColor = GridStyle.StatusWhite;
        }
        // Within the early-warning band → amber (plan ahead).
        else if (reorder > 0 && stock <= reorder * buffer)
        {
            cs.BackColor          = GridStyle.StatusAmber;
            cs.ForeColor          = GridStyle.StatusBlack;
            cs.SelectionBackColor = GridStyle.StatusAmberSel;
            cs.SelectionForeColor = GridStyle.StatusBlack;
        }
        // OK → leave the default zebra striping (keeps the grid readable).
    }

    // ── Stock Movement tab ────────────────────────────────────────────────────

    /// <summary>Fills the material ComboBox on the Stock Movement tab.</summary>
    private void PopulateMovementCombo()
    {
        var selected = cmbMovMaterial.SelectedItem is ComboItem ci ? ci.Id : -1;
        cmbMovMaterial.Items.Clear();

        foreach (var m in _matRepo.GetAll())
            cmbMovMaterial.Items.Add(new ComboItem(m.MaterialId, m.Name));

        for (int i = 0; i < cmbMovMaterial.Items.Count; i++)
        {
            if (cmbMovMaterial.Items[i] is ComboItem item && item.Id == selected)
            {
                cmbMovMaterial.SelectedIndex = i;
                break;
            }
        }

        if (cmbMovMaterial.SelectedIndex < 0 && cmbMovMaterial.Items.Count > 0)
            cmbMovMaterial.SelectedIndex = 0;

        LoadMovementHistory();
    }

    /// <summary>Loads the movement history for the currently selected material.</summary>
    private void LoadMovementHistory()
    {
        if (cmbMovMaterial.SelectedItem is not ComboItem ci)
        {
            dgvMovements.DataSource = null;
            return;
        }

        var rows = _movRepo.GetForMaterial(ci.Id)
            .Select(sm => new
            {
                sm.MovementId,
                Type     = sm.MovementType,
                Quantity = sm.Quantity,
                Date     = sm.Date.ToString("dd MMM yyyy")
            })
            .ToList();

        dgvMovements.DataSource = rows;

        if (dgvMovements.Columns.Count > 0)
        {
            SetMovColVisible("MovementId", false);
            SetMovColHeader("Type",        "Type");
            SetMovColHeader("Quantity",    "Quantity");
            SetMovColHeader("Date",        "Date");

            SetMovColWeight("Type",     120);
            SetMovColWeight("Quantity", 130);
            SetMovColWeight("Date",     130);

            SetMovColFormat("Quantity", "0.##");
            SetMovColAlign("Quantity", DataGridViewContentAlignment.MiddleRight);
        }
    }

    /// <summary>Validates entry and records a stock movement, then refreshes both grids.</summary>
    private void BtnRecordMovement_Click(object sender, EventArgs e)
    {
        if (cmbMovMaterial.SelectedItem is not ComboItem ci)
        {
            ShowMovStatus("Please select a material.", success: false);
            return;
        }

        var movement = new StockMovement
        {
            MaterialId   = ci.Id,
            MovementType = cmbMovType.SelectedItem?.ToString() ?? "In",
            Quantity     = numMovQty.Value,
            Date         = dtpMovDate.Value.Date
        };

        var result = _service.RecordMovement(movement);

        if (!result.Success)
        {
            ShowMovStatus(result.Message, success: false);
            return;
        }

        numMovQty.Value = 0.01M;

        ShowMovStatus(result.IsWarning ? result.Message : "Movement recorded.", success: !result.IsWarning);

        LoadCatalogue();
        LoadMovementHistory();
    }

    /// <summary>Refreshes movement history when the user switches to the Stock Movement tab.</summary>
    private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (tabControl.SelectedTab == tabMovement)
            LoadMovementHistory();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ShowCatStatus(string message, bool success)
    {
        lblCatStatus.Text      = message;
        lblCatStatus.ForeColor = success ? Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69);
        lblCatStatus.Visible   = true;
        var timer = new System.Windows.Forms.Timer { Interval = 4000 };
        timer.Tick += (_, _) => { lblCatStatus.Visible = false; timer.Stop(); timer.Dispose(); };
        timer.Start();
    }

    private void ShowMovStatus(string message, bool success)
    {
        lblMovStatus.Text      = message;
        lblMovStatus.ForeColor = success ? Color.FromArgb(40, 167, 69) : Color.FromArgb(198, 120, 0);
        lblMovStatus.Visible   = true;
        var timer = new System.Windows.Forms.Timer { Interval = 4000 };
        timer.Tick += (_, _) => { lblMovStatus.Visible = false; timer.Stop(); timer.Dispose(); };
        timer.Start();
    }

    // Null-safe catalogue column helpers
    private void SetMatColVisible(string name, bool v) { if (dgvMaterials.Columns[name] is { } c) c.Visible = v; }
    private void SetMatColHeader(string name, string h) { if (dgvMaterials.Columns[name] is { } c) c.HeaderText = h; }
    private void SetMatColWeight(string name, int w)   { if (dgvMaterials.Columns[name] is { } c) c.FillWeight = w; }
    private void SetMatColFormat(string name, string f) { if (dgvMaterials.Columns[name] is { } c) c.DefaultCellStyle.Format = f; }
    private void SetMatColAlign(string name, DataGridViewContentAlignment a)
        { if (dgvMaterials.Columns[name] is { } c) c.DefaultCellStyle.Alignment = a; }

    // Null-safe movement column helpers
    private void SetMovColVisible(string name, bool v) { if (dgvMovements.Columns[name] is { } c) c.Visible = v; }
    private void SetMovColHeader(string name, string h) { if (dgvMovements.Columns[name] is { } c) c.HeaderText = h; }
    private void SetMovColWeight(string name, int w)   { if (dgvMovements.Columns[name] is { } c) c.FillWeight = w; }
    private void SetMovColFormat(string name, string f) { if (dgvMovements.Columns[name] is { } c) c.DefaultCellStyle.Format = f; }
    private void SetMovColAlign(string name, DataGridViewContentAlignment a)
        { if (dgvMovements.Columns[name] is { } c) c.DefaultCellStyle.Alignment = a; }
}
