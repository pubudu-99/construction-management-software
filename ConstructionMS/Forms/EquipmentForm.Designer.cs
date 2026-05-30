namespace ConstructionMS.Forms;

partial class EquipmentForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Entry panel ───────────────────────────────────────────────────────────
    private Panel         pnlEntry;
    private Label         lblEntryTitle;
    private Label         lblEquipName;
    private TextBox       txtEquipName;
    private Label         lblEquipType;
    private TextBox       txtEquipType;
    private Label         lblEquipStatusLbl;
    private ComboBox      cmbEquipStatus;
    private Label         lblCurrentSite;
    private TextBox       txtCurrentSite;
    private Label         lblLastMaint;
    private DateTimePicker dtpLastMaint;
    private Label         lblNextMaint;
    private DateTimePicker dtpNextMaint;
    private Button        btnSaveEquip;
    private Label         lblEquipStatus;

    // ── Grid panel ────────────────────────────────────────────────────────────
    private Panel         pnlGrid;
    private Label         lblGridTitle;
    private DataGridView  dgvEquipment;
    private ContextMenuStrip ctxEquipMenu;
    private ToolStripMenuItem mnuSetStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlEntry        = new Panel();
        lblEntryTitle   = new Label();
        lblEquipName    = new Label();
        txtEquipName    = new TextBox();
        lblEquipType    = new Label();
        txtEquipType    = new TextBox();
        lblEquipStatusLbl = new Label();
        cmbEquipStatus  = new ComboBox();
        lblCurrentSite  = new Label();
        txtCurrentSite  = new TextBox();
        lblLastMaint    = new Label();
        dtpLastMaint    = new DateTimePicker();
        lblNextMaint    = new Label();
        dtpNextMaint    = new DateTimePicker();
        btnSaveEquip    = new Button();
        lblEquipStatus  = new Label();
        pnlGrid         = new Panel();
        lblGridTitle    = new Label();
        dgvEquipment    = new DataGridView();
        ctxEquipMenu    = new ContextMenuStrip();
        mnuSetStatus    = new ToolStripMenuItem();

        ((System.ComponentModel.ISupportInitialize)dgvEquipment).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // ENTRY PANEL  (two-column layout)
        // ══════════════════════════════════════════════════════════════════════
        pnlEntry.Dock      = DockStyle.Top;
        pnlEntry.Height    = 272;
        pnlEntry.BackColor = Color.FromArgb(248, 249, 250);

        const int lx = 16, rx = 492, cw = 452;
        int y = 10;

        lblEntryTitle.Text      = "Add New Equipment";
        lblEntryTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblEntryTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblEntryTitle.Location  = new Point(lx, y);
        lblEntryTitle.Size      = new Size(900, 24);
        y += 36;

        // Row 1 — Name / Type
        lblEquipName.Text      = "Name  *";
        lblEquipName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEquipName.ForeColor = Color.FromArgb(60, 60, 60);
        lblEquipName.Location  = new Point(lx, y);
        lblEquipName.Size      = new Size(cw, 18);

        lblEquipType.Text      = "Type  *";
        lblEquipType.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEquipType.ForeColor = Color.FromArgb(60, 60, 60);
        lblEquipType.Location  = new Point(rx, y);
        lblEquipType.Size      = new Size(cw, 18);
        y += 20;

        txtEquipName.Font     = new Font("Segoe UI", 9.5F);
        txtEquipName.Location = new Point(lx, y);
        txtEquipName.Size     = new Size(cw, 26);

        txtEquipType.Font     = new Font("Segoe UI", 9.5F);
        txtEquipType.Location = new Point(rx, y);
        txtEquipType.Size     = new Size(cw, 26);
        y += 38;

        // Row 2 — Status / Current Site
        lblEquipStatusLbl.Text      = "Status";
        lblEquipStatusLbl.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEquipStatusLbl.ForeColor = Color.FromArgb(60, 60, 60);
        lblEquipStatusLbl.Location  = new Point(lx, y);
        lblEquipStatusLbl.Size      = new Size(cw, 18);

        lblCurrentSite.Text      = "Current Site";
        lblCurrentSite.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCurrentSite.ForeColor = Color.FromArgb(60, 60, 60);
        lblCurrentSite.Location  = new Point(rx, y);
        lblCurrentSite.Size      = new Size(cw, 18);
        y += 20;

        cmbEquipStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEquipStatus.Items.AddRange(new object[] { "Available", "In Use", "Under Maintenance" });
        cmbEquipStatus.SelectedIndex = 0;
        cmbEquipStatus.Font          = new Font("Segoe UI", 9.5F);
        cmbEquipStatus.Location      = new Point(lx, y);
        cmbEquipStatus.Size          = new Size(cw, 26);

        txtCurrentSite.Font        = new Font("Segoe UI", 9.5F);
        txtCurrentSite.Location    = new Point(rx, y);
        txtCurrentSite.Size        = new Size(cw, 26);
        txtCurrentSite.PlaceholderText = "e.g. Site A, Head Office";
        y += 38;

        // Row 3 — Last Maintenance / Next Maintenance
        lblLastMaint.Text      = "Last Maintenance";
        lblLastMaint.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblLastMaint.ForeColor = Color.FromArgb(60, 60, 60);
        lblLastMaint.Location  = new Point(lx, y);
        lblLastMaint.Size      = new Size(cw, 18);

        lblNextMaint.Text      = "Next Maintenance  (alert trigger)";
        lblNextMaint.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNextMaint.ForeColor = Color.FromArgb(60, 60, 60);
        lblNextMaint.Location  = new Point(rx, y);
        lblNextMaint.Size      = new Size(cw, 18);
        y += 20;

        dtpLastMaint.Format      = DateTimePickerFormat.Short;
        dtpLastMaint.ShowCheckBox = true;
        dtpLastMaint.Checked     = false;
        dtpLastMaint.Font        = new Font("Segoe UI", 9.5F);
        dtpLastMaint.Location    = new Point(lx, y);
        dtpLastMaint.Size        = new Size(cw, 26);

        dtpNextMaint.Format      = DateTimePickerFormat.Short;
        dtpNextMaint.ShowCheckBox = true;
        dtpNextMaint.Checked     = false;
        dtpNextMaint.Font        = new Font("Segoe UI", 9.5F);
        dtpNextMaint.Location    = new Point(rx, y);
        dtpNextMaint.Size        = new Size(cw, 26);
        y += 40;

        // Row 4 — Save button + status label
        btnSaveEquip.Text                      = "Add Equipment";
        btnSaveEquip.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveEquip.BackColor                 = Color.FromArgb(40, 167, 69);
        btnSaveEquip.ForeColor                 = Color.White;
        btnSaveEquip.FlatStyle                 = FlatStyle.Flat;
        btnSaveEquip.FlatAppearance.BorderSize = 0;
        btnSaveEquip.Location                  = new Point(lx, y);
        btnSaveEquip.Size                      = new Size(180, 34);
        btnSaveEquip.Cursor                    = Cursors.Hand;
        btnSaveEquip.Click                    += BtnSaveEquip_Click;

        lblEquipStatus.Text      = "";
        lblEquipStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEquipStatus.Location  = new Point(lx + 190, y + 6);
        lblEquipStatus.Size      = new Size(560, 22);
        lblEquipStatus.Visible   = false;

        pnlEntry.Controls.AddRange(new Control[] {
            lblEntryTitle,
            lblEquipName, txtEquipName, lblEquipType, txtEquipType,
            lblEquipStatusLbl, cmbEquipStatus, lblCurrentSite, txtCurrentSite,
            lblLastMaint, dtpLastMaint, lblNextMaint, dtpNextMaint,
            btnSaveEquip, lblEquipStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // GRID PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlGrid.Dock      = DockStyle.Fill;
        pnlGrid.BackColor = Color.White;
        pnlGrid.Padding   = new Padding(12, 8, 12, 12);

        lblGridTitle.Text      = "Equipment List";
        lblGridTitle.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblGridTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblGridTitle.Dock      = DockStyle.Top;
        lblGridTitle.Height    = 24;

        dgvEquipment.Dock = DockStyle.Fill;
        dgvEquipment.CellFormatting += DgvEquipment_CellFormatting;

        // Context menu
        mnuSetStatus.Text   = "Set Status...";
        mnuSetStatus.Click += MnuSetStatus_Click;
        ctxEquipMenu.Items.Add(mnuSetStatus);
        dgvEquipment.ContextMenuStrip = ctxEquipMenu;

        pnlGrid.Controls.Add(dgvEquipment);
        pnlGrid.Controls.Add(lblGridTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(980, 620);
        Text          = "Equipment Management";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(820, 500);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlGrid);
        Controls.Add(pnlEntry);

        ((System.ComponentModel.ISupportInitialize)dgvEquipment).EndInit();
        ResumeLayout(false);
    }
}
