namespace ConstructionMS.Forms;

partial class StockForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Tab control ───────────────────────────────────────────────────────────
    private TabControl    tabControl;
    private TabPage       tabCatalogue;
    private TabPage       tabMovement;

    // ── Catalogue tab ─────────────────────────────────────────────────────────
    private Panel         pnlCatalogueEntry;
    private Label         lblCatTitle;
    private Label         lblMatName;
    private TextBox       txtMatName;
    private Label         lblUnit;
    private TextBox       txtUnit;
    private Label         lblInitStock;
    private NumericUpDown numInitStock;
    private Label         lblReorder;
    private NumericUpDown numReorder;
    private Button        btnAddMaterial;
    private Label         lblCatStatus;
    private DataGridView  dgvMaterials;

    // ── Stock Movement tab ────────────────────────────────────────────────────
    private Panel         pnlMovementEntry;
    private Label         lblMovTitle;
    private Label         lblMovMaterial;
    private ComboBox      cmbMovMaterial;
    private Label         lblMovType;
    private ComboBox      cmbMovType;
    private Label         lblMovQty;
    private NumericUpDown numMovQty;
    private Label         lblMovDate;
    private DateTimePicker dtpMovDate;
    private Button        btnRecordMovement;
    private Label         lblMovStatus;
    private DataGridView  dgvMovements;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tabControl         = new TabControl();
        tabCatalogue       = new TabPage();
        tabMovement        = new TabPage();

        pnlCatalogueEntry  = new Panel();
        lblCatTitle        = new Label();
        lblMatName         = new Label();
        txtMatName         = new TextBox();
        lblUnit            = new Label();
        txtUnit            = new TextBox();
        lblInitStock       = new Label();
        numInitStock       = new NumericUpDown();
        lblReorder         = new Label();
        numReorder         = new NumericUpDown();
        btnAddMaterial     = new Button();
        lblCatStatus       = new Label();
        dgvMaterials       = new DataGridView();

        pnlMovementEntry   = new Panel();
        lblMovTitle        = new Label();
        lblMovMaterial     = new Label();
        cmbMovMaterial     = new ComboBox();
        lblMovType         = new Label();
        cmbMovType         = new ComboBox();
        lblMovQty          = new Label();
        numMovQty          = new NumericUpDown();
        lblMovDate         = new Label();
        dtpMovDate         = new DateTimePicker();
        btnRecordMovement  = new Button();
        lblMovStatus       = new Label();
        dgvMovements       = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)numInitStock).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numReorder).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numMovQty).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvMaterials).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvMovements).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // CATALOGUE ENTRY PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlCatalogueEntry.Dock      = DockStyle.Top;
        pnlCatalogueEntry.Height    = 210;
        pnlCatalogueEntry.BackColor = Color.FromArgb(248, 249, 250);

        const int lx = 16, cw1 = 380, rx1 = 412, cw2 = 200, rx2 = 256;
        int y = 10;

        lblCatTitle.Text      = "Add New Material";
        lblCatTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblCatTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblCatTitle.Location  = new Point(lx, y);
        lblCatTitle.Size      = new Size(800, 24);
        y += 36;

        // Row 1 — Name / Unit
        lblMatName.Text      = "Name  *";
        lblMatName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMatName.ForeColor = Color.FromArgb(60, 60, 60);
        lblMatName.Location  = new Point(lx, y);
        lblMatName.Size      = new Size(cw1, 18);

        lblUnit.Text      = "Unit  *";
        lblUnit.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUnit.ForeColor = Color.FromArgb(60, 60, 60);
        lblUnit.Location  = new Point(rx1, y);
        lblUnit.Size      = new Size(200, 18);
        y += 20;

        txtMatName.Font     = new Font("Segoe UI", 9.5F);
        txtMatName.Location = new Point(lx, y);
        txtMatName.Size     = new Size(cw1, 26);

        txtUnit.Font            = new Font("Segoe UI", 9.5F);
        txtUnit.Location        = new Point(rx1, y);
        txtUnit.Size            = new Size(200, 26);
        txtUnit.PlaceholderText = "e.g. kg, bags, m³";
        y += 36;

        // Row 2 — Initial Stock / Reorder Point
        lblInitStock.Text      = "Initial Stock";
        lblInitStock.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblInitStock.ForeColor = Color.FromArgb(60, 60, 60);
        lblInitStock.Location  = new Point(lx, y);
        lblInitStock.Size      = new Size(cw2, 18);

        lblReorder.Text      = "Reorder Point";
        lblReorder.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblReorder.ForeColor = Color.FromArgb(60, 60, 60);
        lblReorder.Location  = new Point(rx2, y);
        lblReorder.Size      = new Size(cw2, 18);
        y += 20;

        numInitStock.Minimum        = 0;
        numInitStock.Maximum        = 999999;
        numInitStock.DecimalPlaces  = 2;
        numInitStock.Font           = new Font("Segoe UI", 9.5F);
        numInitStock.Location       = new Point(lx, y);
        numInitStock.Size           = new Size(cw2, 26);

        numReorder.Minimum        = 0;
        numReorder.Maximum        = 999999;
        numReorder.DecimalPlaces  = 2;
        numReorder.Font           = new Font("Segoe UI", 9.5F);
        numReorder.Location       = new Point(rx2, y);
        numReorder.Size           = new Size(cw2, 26);
        y += 42;

        // Row 3 — Add button + status
        btnAddMaterial.Text                      = "Add Material";
        btnAddMaterial.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnAddMaterial.BackColor                 = Color.FromArgb(40, 167, 69);
        btnAddMaterial.ForeColor                 = Color.White;
        btnAddMaterial.FlatStyle                 = FlatStyle.Flat;
        btnAddMaterial.FlatAppearance.BorderSize = 0;
        btnAddMaterial.Location                  = new Point(lx, y);
        btnAddMaterial.Size                      = new Size(160, 34);
        btnAddMaterial.Cursor                    = Cursors.Hand;
        btnAddMaterial.Click                    += BtnAddMaterial_Click;

        lblCatStatus.Text      = "";
        lblCatStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCatStatus.Location  = new Point(lx + 170, y + 6);
        lblCatStatus.Size      = new Size(500, 22);
        lblCatStatus.Visible   = false;

        pnlCatalogueEntry.Controls.AddRange(new Control[] {
            lblCatTitle,
            lblMatName, txtMatName, lblUnit, txtUnit,
            lblInitStock, numInitStock, lblReorder, numReorder,
            btnAddMaterial, lblCatStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // MATERIALS GRID
        // ══════════════════════════════════════════════════════════════════════
        dgvMaterials.Dock              = DockStyle.Fill;
        dgvMaterials.CellFormatting   += DgvMaterials_CellFormatting;

        tabCatalogue.Text    = "Catalogue";
        tabCatalogue.Padding = new Padding(4);
        tabCatalogue.Controls.Add(dgvMaterials);
        tabCatalogue.Controls.Add(pnlCatalogueEntry);

        // ══════════════════════════════════════════════════════════════════════
        // STOCK MOVEMENT ENTRY PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlMovementEntry.Dock      = DockStyle.Top;
        pnlMovementEntry.Height    = 210;
        pnlMovementEntry.BackColor = Color.FromArgb(248, 249, 250);

        const int mx1 = 400, mrx1 = 432, mx2 = 220, mrx2 = 256;
        y = 10;

        lblMovTitle.Text      = "Record Stock Movement";
        lblMovTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblMovTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblMovTitle.Location  = new Point(lx, y);
        lblMovTitle.Size      = new Size(800, 24);
        y += 36;

        // Row 1 — Material / Movement Type
        lblMovMaterial.Text      = "Material  *";
        lblMovMaterial.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMovMaterial.ForeColor = Color.FromArgb(60, 60, 60);
        lblMovMaterial.Location  = new Point(lx, y);
        lblMovMaterial.Size      = new Size(mx1, 18);

        lblMovType.Text      = "Movement Type";
        lblMovType.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMovType.ForeColor = Color.FromArgb(60, 60, 60);
        lblMovType.Location  = new Point(mrx1, y);
        lblMovType.Size      = new Size(200, 18);
        y += 20;

        cmbMovMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbMovMaterial.Font          = new Font("Segoe UI", 9.5F);
        cmbMovMaterial.Location      = new Point(lx, y);
        cmbMovMaterial.Size          = new Size(mx1, 26);

        cmbMovType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbMovType.Items.AddRange(new object[] { "In", "Out" });
        cmbMovType.SelectedIndex = 0;
        cmbMovType.Font          = new Font("Segoe UI", 9.5F);
        cmbMovType.Location      = new Point(mrx1, y);
        cmbMovType.Size          = new Size(200, 26);
        y += 36;

        // Row 2 — Quantity / Date
        lblMovQty.Text      = "Quantity  *";
        lblMovQty.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMovQty.ForeColor = Color.FromArgb(60, 60, 60);
        lblMovQty.Location  = new Point(lx, y);
        lblMovQty.Size      = new Size(mx2, 18);

        lblMovDate.Text      = "Date";
        lblMovDate.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMovDate.ForeColor = Color.FromArgb(60, 60, 60);
        lblMovDate.Location  = new Point(mrx2, y);
        lblMovDate.Size      = new Size(280, 18);
        y += 20;

        numMovQty.Minimum       = 0.01M;
        numMovQty.Maximum       = 999999;
        numMovQty.DecimalPlaces = 2;
        numMovQty.Font          = new Font("Segoe UI", 9.5F);
        numMovQty.Location      = new Point(lx, y);
        numMovQty.Size          = new Size(mx2, 26);

        dtpMovDate.Format   = DateTimePickerFormat.Short;
        dtpMovDate.Font     = new Font("Segoe UI", 9.5F);
        dtpMovDate.Location = new Point(mrx2, y);
        dtpMovDate.Size     = new Size(280, 26);
        y += 42;

        // Row 3 — Record button + status
        btnRecordMovement.Text                      = "Record Movement";
        btnRecordMovement.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnRecordMovement.BackColor                 = Color.FromArgb(0, 123, 255);
        btnRecordMovement.ForeColor                 = Color.White;
        btnRecordMovement.FlatStyle                 = FlatStyle.Flat;
        btnRecordMovement.FlatAppearance.BorderSize = 0;
        btnRecordMovement.Location                  = new Point(lx, y);
        btnRecordMovement.Size                      = new Size(180, 34);
        btnRecordMovement.Cursor                    = Cursors.Hand;
        btnRecordMovement.Click                    += BtnRecordMovement_Click;

        lblMovStatus.Text      = "";
        lblMovStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMovStatus.Location  = new Point(lx + 190, y + 6);
        lblMovStatus.Size      = new Size(500, 22);
        lblMovStatus.Visible   = false;

        pnlMovementEntry.Controls.AddRange(new Control[] {
            lblMovTitle,
            lblMovMaterial, cmbMovMaterial, lblMovType, cmbMovType,
            lblMovQty, numMovQty, lblMovDate, dtpMovDate,
            btnRecordMovement, lblMovStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // MOVEMENTS GRID
        // ══════════════════════════════════════════════════════════════════════
        dgvMovements.Dock = DockStyle.Fill;

        tabMovement.Text    = "Stock Movement";
        tabMovement.Padding = new Padding(4);
        tabMovement.Controls.Add(dgvMovements);
        tabMovement.Controls.Add(pnlMovementEntry);

        // ── Tab control ───────────────────────────────────────────────────────
        tabControl.Dock = DockStyle.Fill;
        tabControl.Font = new Font("Segoe UI", 9F);
        tabControl.TabPages.Add(tabCatalogue);
        tabControl.TabPages.Add(tabMovement);
        tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(980, 620);
        Text          = "Materials & Stock";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(820, 500);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(tabControl);

        ((System.ComponentModel.ISupportInitialize)numInitStock).EndInit();
        ((System.ComponentModel.ISupportInitialize)numReorder).EndInit();
        ((System.ComponentModel.ISupportInitialize)numMovQty).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvMaterials).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvMovements).EndInit();
        ResumeLayout(false);
    }
}
