namespace ConstructionMS.Forms;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Menu strip ────────────────────────────────────────────────────────────
    private MenuStrip          mainMenu;
    private ToolStripMenuItem  mnuFile;
    private ToolStripMenuItem  mnuFileBackup;
    private ToolStripMenuItem  mnuFileSampleData;
    private ToolStripSeparator mnuFileSep;
    private ToolStripMenuItem  mnuFileChangePassword;
    private ToolStripMenuItem  mnuFileSignOut;
    private ToolStripMenuItem  mnuHelp;
    private ToolStripMenuItem  mnuHelpAbout;

    // ── Header ───────────────────────────────────────────────────────────────
    private Panel  pnlHeader;
    private Label  lblAppTitle;
    private Label  lblAppSub;
    private Label  lblWelcome;

    // ── Left nav ─────────────────────────────────────────────────────────────
    private Panel  pnlMenu;
    private Button btnNavProject;
    private Button btnNavPayments;
    private Button btnNavTasks;
    private Button btnNavAttendance;
    private Button btnNavPayroll;
    private Button btnNavEquipment;
    private Button btnNavMaterials;
    private Button btnNavContacts;
    private Button btnNavReports;
    private Button btnNavWorkers;
    private Button btnNavUsers;

    // Section captions — kept as fields so they can be hidden/repositioned per role.
    private Label  lblNavOperations;
    private Label  lblNavResources;
    private Label  lblNavPeople;
    private Label  lblNavAdmin;

    // ── Main area ─────────────────────────────────────────────────────────────
    private Panel             pnlMain;
    private Label             lblKpiHeading;
    private TableLayoutPanel  tlpKpiRow1;
    private TableLayoutPanel  tlpKpiRow2;
    private Label             lblAlertHeading;
    private TableLayoutPanel  tlpAlerts;
    private CardPanel    cardDeadlines;
    private RichTextBox  rtbDeadlines;
    private CardPanel    cardMaintenance;
    private RichTextBox  rtbMaintenance;
    private CardPanel    cardLowStock;
    private RichTextBox  rtbLowStock;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        mainMenu        = new MenuStrip();
        mnuFile               = new ToolStripMenuItem();
        mnuFileBackup         = new ToolStripMenuItem();
        mnuFileSampleData     = new ToolStripMenuItem();
        mnuFileSep            = new ToolStripSeparator();
        mnuFileChangePassword = new ToolStripMenuItem();
        mnuFileSignOut        = new ToolStripMenuItem();
        mnuHelp               = new ToolStripMenuItem();
        mnuHelpAbout          = new ToolStripMenuItem();

        pnlHeader         = new Panel();
        lblAppTitle       = new Label();
        lblAppSub         = new Label();
        lblWelcome        = new Label();
        pnlMenu           = new Panel();
        btnNavProject     = new Button();
        btnNavPayments    = new Button();
        btnNavTasks       = new Button();
        btnNavAttendance  = new Button();
        btnNavPayroll     = new Button();
        btnNavEquipment   = new Button();
        btnNavMaterials   = new Button();
        btnNavContacts    = new Button();
        btnNavReports     = new Button();
        btnNavWorkers     = new Button();
        btnNavUsers       = new Button();
        pnlMain           = new Panel();
        lblKpiHeading     = new Label();
        tlpKpiRow1        = new TableLayoutPanel();
        tlpKpiRow2        = new TableLayoutPanel();
        lblAlertHeading   = new Label();
        tlpAlerts         = new TableLayoutPanel();
        cardDeadlines     = new CardPanel("Upcoming Deadlines", Theme.Danger);
        rtbDeadlines      = new RichTextBox();
        cardMaintenance   = new CardPanel("Maintenance Due", Theme.Caution);
        rtbMaintenance    = new RichTextBox();
        cardLowStock      = new CardPanel("Low Stock", Theme.Primary);
        rtbLowStock       = new RichTextBox();

        SuspendLayout();

        // ── Menu strip ────────────────────────────────────────────────────────
        mnuFileBackup.Text   = "Backup Database...";
        mnuFileBackup.Click += MnuFileBackup_Click;

        mnuFileSampleData.Text   = "Load Sample Data...";
        mnuFileSampleData.Click += MnuFileSampleData_Click;

        mnuFileChangePassword.Text   = "Change Password...";
        mnuFileChangePassword.Click += MnuChangePassword_Click;

        mnuFileSignOut.Text   = "Sign Out";
        mnuFileSignOut.Click += MnuFileSignOut_Click;

        mnuFile.Text = "File";
        mnuFile.DropDownItems.AddRange(new ToolStripItem[]
            { mnuFileBackup, mnuFileSampleData, mnuFileSep, mnuFileChangePassword, mnuFileSignOut });

        mnuHelpAbout.Text   = "About";
        mnuHelpAbout.Click += MnuHelpAbout_Click;

        mnuHelp.Text = "Help";
        mnuHelp.DropDownItems.Add(mnuHelpAbout);

        mainMenu.Items.AddRange(new ToolStripItem[] { mnuFile, mnuHelp });
        mainMenu.BackColor = Color.FromArgb(245, 245, 245);
        mainMenu.Font      = new Font("Segoe UI", 9F);

        // ── Header ────────────────────────────────────────────────────────────
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Height    = 64;
        pnlHeader.BackColor = Theme.Brand;
        // Padding frames the right-docked welcome label and Sign Out button.
        pnlHeader.Padding   = new Padding(0, 16, 14, 16);

        lblAppTitle.Text      = "Construction Manager";
        lblAppTitle.Font      = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblAppTitle.ForeColor = Color.White;
        lblAppTitle.AutoSize  = true;
        lblAppTitle.Location  = new Point(18, 9);

        lblAppSub.Text      = "Management Dashboard";
        lblAppSub.Font      = new Font("Segoe UI", 9F);
        lblAppSub.ForeColor = Theme.Lighten(Theme.Brand, 0.7);
        lblAppSub.AutoSize  = true;
        lblAppSub.Location  = new Point(20, 38);

        // Welcome label is DOCKED right (not anchored): dock layout is
        // recomputed on every resize, so it stays visible at any window
        // width, DPI, or start-maximized state. Anchoring here previously
        // captured offsets against the panel's pre-layout width and pushed
        // the control off-screen on maximized startup.
        // Sign-out lives in the File menu — no header button needed.
        lblWelcome.Text      = "";
        lblWelcome.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblWelcome.ForeColor = Color.White;
        lblWelcome.TextAlign = ContentAlignment.MiddleRight;
        lblWelcome.AutoSize  = false;
        lblWelcome.Width     = 480;
        lblWelcome.Dock      = DockStyle.Right;

        pnlHeader.Controls.AddRange(new Control[] { lblAppTitle, lblAppSub, lblWelcome });

        // ── Left nav ──────────────────────────────────────────────────────────
        pnlMenu.Dock       = DockStyle.Left;
        pnlMenu.Width      = 200;
        pnlMenu.BackColor  = Color.White;
        pnlMenu.AutoScroll = true;

        int navY = 12;
        lblNavOperations = AddNavSection(pnlMenu, "OPERATIONS", ref navY);
        SetupNavButton(pnlMenu, btnNavProject,  "\U0001F4C1", "Project",  ref navY); btnNavProject.Click  += BtnNavProject_Click;
        SetupNavButton(pnlMenu, btnNavPayments, "\U0001F4B0", "Payments", ref navY); btnNavPayments.Click += BtnNavPayments_Click;
        SetupNavButton(pnlMenu, btnNavTasks,    "✔",      "Tasks",    ref navY); btnNavTasks.Click    += BtnNavTasks_Click;
        SetupNavButton(pnlMenu, btnNavReports,  "\U0001F4CA", "Reports",  ref navY); btnNavReports.Click  += BtnNavReports_Click;

        lblNavResources = AddNavSection(pnlMenu, "RESOURCES", ref navY);
        SetupNavButton(pnlMenu, btnNavEquipment, "\U0001F6E0", "Equipment", ref navY); btnNavEquipment.Click += BtnNavEquipment_Click;
        SetupNavButton(pnlMenu, btnNavMaterials, "\U0001F4E6", "Materials", ref navY); btnNavMaterials.Click += BtnNavMaterials_Click;

        lblNavPeople = AddNavSection(pnlMenu, "PEOPLE", ref navY);
        SetupNavButton(pnlMenu, btnNavAttendance, "\U0001F551", "Attendance", ref navY); btnNavAttendance.Click += BtnNavAttendance_Click;
        SetupNavButton(pnlMenu, btnNavPayroll,    "\U0001F4B5", "Payroll",    ref navY); btnNavPayroll.Click    += BtnNavPayroll_Click;
        SetupNavButton(pnlMenu, btnNavContacts,   "☎",      "Contacts",   ref navY); btnNavContacts.Click   += BtnNavContacts_Click;
        SetupNavButton(pnlMenu, btnNavWorkers,    "\U0001F477", "Workers",    ref navY); btnNavWorkers.Click    += BtnNavWorkers_Click;

        lblNavAdmin = AddNavSection(pnlMenu, "ADMIN", ref navY);
        SetupNavButton(pnlMenu, btnNavUsers, "\U0001F464", "Users", ref navY); btnNavUsers.Click += BtnNavUsers_Click;

        // ── Main area ─────────────────────────────────────────────────────────
        pnlMain.Dock       = DockStyle.Fill;
        pnlMain.BackColor  = Theme.Surface;
        pnlMain.Padding    = new Padding(20, 16, 20, 16);
        pnlMain.AutoScroll = true;

        lblKpiHeading.Text      = "Overview";
        lblKpiHeading.Dock      = DockStyle.Top;
        lblKpiHeading.Height    = 30;
        lblKpiHeading.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblKpiHeading.ForeColor = Theme.TextPrimary;

        // Row 1 — 3 equal full-width cards
        tlpKpiRow1.Dock        = DockStyle.Top;
        tlpKpiRow1.Height      = 132;          // DPI-adjusted at runtime in SizeKpiRows()
        tlpKpiRow1.ColumnCount = 3;
        tlpKpiRow1.RowCount    = 1;
        tlpKpiRow1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        tlpKpiRow1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpKpiRow1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpKpiRow1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpKpiRow1.BackColor   = Theme.Surface;

        // Row 2 — 4 equal full-width cards
        tlpKpiRow2.Dock        = DockStyle.Top;
        tlpKpiRow2.Height      = 132;
        tlpKpiRow2.ColumnCount = 4;
        tlpKpiRow2.RowCount    = 1;
        tlpKpiRow2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpKpiRow2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpKpiRow2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpKpiRow2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tlpKpiRow2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpKpiRow2.BackColor   = Theme.Surface;

        lblAlertHeading.Text      = "Alerts";
        lblAlertHeading.Dock      = DockStyle.Top;
        lblAlertHeading.Height    = 32;
        lblAlertHeading.Padding   = new Padding(0, 8, 0, 0);
        lblAlertHeading.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblAlertHeading.ForeColor = Theme.TextPrimary;

        // Alert cards row
        tlpAlerts.Dock        = DockStyle.Top;
        tlpAlerts.Height      = 300;
        tlpAlerts.ColumnCount = 3;
        tlpAlerts.RowCount    = 1;
        tlpAlerts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        tlpAlerts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpAlerts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpAlerts.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpAlerts.BackColor   = Theme.Surface;

        ConfigureAlertCard(cardDeadlines,   rtbDeadlines,   new Padding(0, 0, 8, 0));
        ConfigureAlertCard(cardMaintenance, rtbMaintenance, new Padding(4, 0, 4, 0));
        ConfigureAlertCard(cardLowStock,    rtbLowStock,    new Padding(8, 0, 0, 0));

        tlpAlerts.Controls.Add(cardDeadlines,   0, 0);
        tlpAlerts.Controls.Add(cardMaintenance, 1, 0);
        tlpAlerts.Controls.Add(cardLowStock,    2, 0);

        // Add bottom-up so the visual order (top→bottom) is heading, KPI rows, heading, alerts.
        pnlMain.Controls.Add(tlpAlerts);
        pnlMain.Controls.Add(lblAlertHeading);
        pnlMain.Controls.Add(tlpKpiRow2);
        pnlMain.Controls.Add(tlpKpiRow1);
        pnlMain.Controls.Add(lblKpiHeading);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(1000, 600);
        Text          = "Construction Management - Dashboard";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState   = FormWindowState.Maximized;
        MinimumSize   = new Size(940, 560);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Theme.Surface;

        Controls.Add(pnlMain);
        Controls.Add(pnlMenu);
        Controls.Add(pnlHeader);
        Controls.Add(mainMenu);
        MainMenuStrip = mainMenu;

        ResumeLayout(false);
        PerformLayout();
    }

    /// <summary>Adds a small muted section caption to the nav panel and returns it.</summary>
    private static Label AddNavSection(Panel parent, string text, ref int y)
    {
        var lbl = new Label
        {
            Text      = text,
            Font      = new Font("Segoe UI", 7.5F, FontStyle.Bold),
            ForeColor = Theme.TextMuted,
            Location  = new Point(16, y + 6),
            Size      = new Size(168, 14),
            TextAlign = ContentAlignment.MiddleLeft,
        };
        parent.Controls.Add(lbl);
        y += 24;
        return lbl;
    }

    /// <summary>Applies consistent style and position to a nav button and adds it.</summary>
    private static void SetupNavButton(Panel parent, Button btn, string glyph, string text, ref int y)
    {
        btn.Text      = $"  {glyph}   {text}";
        btn.Font      = new Font("Segoe UI", 9.5F);
        btn.ForeColor = Theme.TextPrimary;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize        = 0;
        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 240, 248);
        btn.BackColor = Color.White;
        btn.TextAlign = ContentAlignment.MiddleLeft;
        btn.Location  = new Point(10, y);
        btn.Size      = new Size(180, 36);
        btn.Cursor    = Cursors.Hand;
        parent.Controls.Add(btn);
        y += 40;
    }

    /// <summary>Docks an alert RichTextBox into a card and sets the card's grid margin.</summary>
    private static void ConfigureAlertCard(CardPanel card, RichTextBox rtb, Padding margin)
    {
        card.Dock   = DockStyle.Fill;
        card.Margin = margin;

        rtb.ReadOnly    = true;
        rtb.BorderStyle = BorderStyle.None;
        rtb.BackColor   = Color.White;
        rtb.Font        = new Font("Segoe UI", 9F);
        rtb.ScrollBars  = RichTextBoxScrollBars.Vertical;
        rtb.DetectUrls  = false;
        rtb.Dock        = DockStyle.Fill;

        card.Content.Controls.Add(rtb);
    }
}
