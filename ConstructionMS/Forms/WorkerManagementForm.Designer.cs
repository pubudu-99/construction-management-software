namespace ConstructionMS.Forms;

partial class WorkerManagementForm
{
    private System.ComponentModel.IContainer components = null;

    private Panel             pnlToolbar;
    private Label             lblTitle;
    private Button            btnAddWorker;
    private Button            btnEditWorker;
    private DataGridView      dgvWorkers;
    private ContextMenuStrip  ctxWorkerMenu;
    private ToolStripMenuItem mnuToggleActive;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlToolbar      = new Panel();
        lblTitle        = new Label();
        btnAddWorker    = new Button();
        btnEditWorker   = new Button();
        dgvWorkers      = new DataGridView();
        ctxWorkerMenu   = new ContextMenuStrip();
        mnuToggleActive = new ToolStripMenuItem();

        ((System.ComponentModel.ISupportInitialize)dgvWorkers).BeginInit();
        SuspendLayout();

        // ── Toolbar ─────────────────────────────────────────────────────────────
        // Width is set to the docked width (ClientSize 920 minus the form's
        // 12px left/right padding) BEFORE the buttons are parented, so the
        // right-anchor reference is correct and the buttons pin to the right.
        pnlToolbar.Dock      = DockStyle.Top;
        pnlToolbar.Height    = 56;
        pnlToolbar.Width     = 896;
        pnlToolbar.BackColor = Color.FromArgb(248, 249, 250);
        pnlToolbar.Padding   = new Padding(16, 10, 16, 10);

        lblTitle.Text      = "Workers";
        lblTitle.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblTitle.Location  = new Point(16, 16);
        lblTitle.AutoSize  = true;

        // Both buttons right-aligned and vertically aligned on the same row,
        // anchored to the top-right so they stay pinned when the form resizes.
        btnEditWorker.Text                      = "Edit Worker";
        btnEditWorker.Font                      = new Font("Segoe UI", 9.5F);
        btnEditWorker.FlatStyle                 = FlatStyle.Flat;
        btnEditWorker.FlatAppearance.BorderColor = Color.LightGray;
        btnEditWorker.BackColor                 = Color.White;
        btnEditWorker.Size                      = new Size(110, 32);
        btnEditWorker.Location                  = new Point(896 - 16 - 110, 12);   // 770
        btnEditWorker.Anchor                    = AnchorStyles.Top | AnchorStyles.Right;
        btnEditWorker.Cursor                    = Cursors.Hand;
        btnEditWorker.Click                    += BtnEditWorker_Click;

        btnAddWorker.Text                      = "+ Add Worker";
        btnAddWorker.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnAddWorker.BackColor                 = Color.FromArgb(40, 167, 69);
        btnAddWorker.ForeColor                 = Color.White;
        btnAddWorker.FlatStyle                 = FlatStyle.Flat;
        btnAddWorker.FlatAppearance.BorderSize = 0;
        btnAddWorker.Size                      = new Size(150, 32);
        btnAddWorker.Location                  = new Point(896 - 16 - 110 - 10 - 150, 12);  // 610
        btnAddWorker.Anchor                    = AnchorStyles.Top | AnchorStyles.Right;
        btnAddWorker.Cursor                    = Cursors.Hand;
        btnAddWorker.Click                    += BtnAddWorker_Click;

        pnlToolbar.Controls.AddRange(new Control[] { lblTitle, btnAddWorker, btnEditWorker });

        // ── Grid ──────────────────────────────────────────────────────────────────
        dgvWorkers.Dock              = DockStyle.Fill;
        dgvWorkers.CellDoubleClick  += DgvWorkers_CellDoubleClick;
        dgvWorkers.CellFormatting   += DgvWorkers_CellFormatting;

        // Context menu
        mnuToggleActive.Text   = "Toggle Active";
        mnuToggleActive.Click += MnuToggleActive_Click;

        ctxWorkerMenu.Items.AddRange(new ToolStripItem[] { mnuToggleActive });
        ctxWorkerMenu.Opening += CtxWorkerMenu_Opening;
        dgvWorkers.ContextMenuStrip = ctxWorkerMenu;

        // ── Form ──────────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(920, 560);
        Text          = "Worker Management";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(760, 480);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;
        Padding       = new Padding(12, 0, 12, 12);

        Controls.Add(dgvWorkers);
        Controls.Add(pnlToolbar);

        ((System.ComponentModel.ISupportInitialize)dgvWorkers).EndInit();
        ResumeLayout(false);
    }
}
