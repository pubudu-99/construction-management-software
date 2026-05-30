namespace ConstructionMS.Forms;

partial class DashboardForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Menu strip ────────────────────────────────────────────────────────────
    private MenuStrip          mainMenu;
    private ToolStripMenuItem  mnuFile;
    private ToolStripMenuItem  mnuFileBackup;
    private ToolStripSeparator mnuFileSep;
    private ToolStripMenuItem  mnuFileSignOut;
    private ToolStripMenuItem  mnuHelp;
    private ToolStripMenuItem  mnuHelpAbout;

    // ── Header ───────────────────────────────────────────────────────────────
    private Panel  pnlHeader;
    private Label  lblWelcome;
    private Button btnChangePassword;
    private Button btnSignOut;

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
    private Button btnNavUsers;

    // ── Main area ─────────────────────────────────────────────────────────────
    private Panel             pnlMain;
    private TableLayoutPanel  tlpCards;
    private GroupBox     grpDeadlines;
    private RichTextBox  rtbDeadlines;
    private GroupBox     grpMaintenance;
    private RichTextBox  rtbMaintenance;
    private GroupBox     grpLowStock;
    private RichTextBox  rtbLowStock;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        mainMenu        = new MenuStrip();
        mnuFile         = new ToolStripMenuItem();
        mnuFileBackup   = new ToolStripMenuItem();
        mnuFileSep      = new ToolStripSeparator();
        mnuFileSignOut  = new ToolStripMenuItem();
        mnuHelp         = new ToolStripMenuItem();
        mnuHelpAbout    = new ToolStripMenuItem();

        pnlHeader         = new Panel();
        lblWelcome        = new Label();
        btnChangePassword = new Button();
        btnSignOut        = new Button();
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
        btnNavUsers       = new Button();
        pnlMain           = new Panel();
        tlpCards          = new TableLayoutPanel();
        grpDeadlines      = new GroupBox();
        rtbDeadlines      = new RichTextBox();
        grpMaintenance    = new GroupBox();
        rtbMaintenance    = new RichTextBox();
        grpLowStock       = new GroupBox();
        rtbLowStock       = new RichTextBox();

        SuspendLayout();

        // ── Menu strip ────────────────────────────────────────────────────────
        mnuFileBackup.Text   = "Backup Database...";
        mnuFileBackup.Click += MnuFileBackup_Click;

        mnuFileSignOut.Text   = "Sign Out";
        mnuFileSignOut.Click += MnuFileSignOut_Click;

        mnuFile.Text = "File";
        mnuFile.DropDownItems.AddRange(new ToolStripItem[]
            { mnuFileBackup, mnuFileSep, mnuFileSignOut });

        mnuHelpAbout.Text   = "About";
        mnuHelpAbout.Click += MnuHelpAbout_Click;

        mnuHelp.Text = "Help";
        mnuHelp.DropDownItems.Add(mnuHelpAbout);

        mainMenu.Items.AddRange(new ToolStripItem[] { mnuFile, mnuHelp });
        mainMenu.BackColor = Color.FromArgb(245, 245, 245);
        mainMenu.Font      = new Font("Segoe UI", 9F);

        // ── Header ────────────────────────────────────────────────────────────
        pnlHeader.Dock      = DockStyle.Top;
        pnlHeader.Height    = 50;
        pnlHeader.BackColor = Color.SteelBlue;
        pnlHeader.Controls.Add(lblWelcome);
        pnlHeader.Controls.Add(btnChangePassword);
        pnlHeader.Controls.Add(btnSignOut);

        lblWelcome.Text      = "";
        lblWelcome.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblWelcome.ForeColor = Color.White;
        lblWelcome.Location  = new Point(12, 12);
        lblWelcome.Size      = new Size(680, 26);
        lblWelcome.AutoSize  = false;

        btnChangePassword.Text                      = "Change Password";
        btnChangePassword.Font                      = new Font("Segoe UI", 9F);
        btnChangePassword.BackColor                 = Color.FromArgb(70, 110, 180);
        btnChangePassword.ForeColor                 = Color.White;
        btnChangePassword.FlatStyle                 = FlatStyle.Flat;
        btnChangePassword.FlatAppearance.BorderSize = 0;
        btnChangePassword.Size                      = new Size(140, 30);
        btnChangePassword.Anchor                    = AnchorStyles.Top | AnchorStyles.Right;
        btnChangePassword.Location                  = new Point(736, 10);
        btnChangePassword.Cursor                    = Cursors.Hand;
        btnChangePassword.Click                    += BtnChangePassword_Click;

        btnSignOut.Text                      = "Sign Out";
        btnSignOut.Font                      = new Font("Segoe UI", 9F);
        btnSignOut.BackColor                 = Color.FromArgb(180, 30, 30);
        btnSignOut.ForeColor                 = Color.White;
        btnSignOut.FlatStyle                 = FlatStyle.Flat;
        btnSignOut.FlatAppearance.BorderSize = 0;
        btnSignOut.Size                      = new Size(88, 30);
        btnSignOut.Anchor                    = AnchorStyles.Top | AnchorStyles.Right;
        btnSignOut.Location                  = new Point(892, 10);
        btnSignOut.Cursor                    = Cursors.Hand;
        btnSignOut.Click                    += BtnSignOut_Click;

        // ── Left nav ──────────────────────────────────────────────────────────
        pnlMenu.Dock      = DockStyle.Left;
        pnlMenu.Width     = 180;
        pnlMenu.BackColor = Color.WhiteSmoke;

        int navY = 14;
        SetupNavButton(btnNavProject,    "Project",    ref navY); btnNavProject.Click    += BtnNavProject_Click;
        SetupNavButton(btnNavPayments,   "Payments",   ref navY); btnNavPayments.Click   += BtnNavPayments_Click;
        SetupNavButton(btnNavTasks,      "Tasks",      ref navY); btnNavTasks.Click      += BtnNavTasks_Click;
        SetupNavButton(btnNavAttendance, "Attendance", ref navY); btnNavAttendance.Click += BtnNavAttendance_Click;
        SetupNavButton(btnNavPayroll,    "Payroll",    ref navY); btnNavPayroll.Click    += BtnNavPayroll_Click;
        SetupNavButton(btnNavEquipment,  "Equipment",  ref navY); btnNavEquipment.Click  += BtnNavEquipment_Click;
        SetupNavButton(btnNavMaterials,  "Materials",  ref navY); btnNavMaterials.Click  += BtnNavMaterials_Click;
        SetupNavButton(btnNavContacts,   "Contacts",   ref navY); btnNavContacts.Click   += BtnNavContacts_Click;
        SetupNavButton(btnNavReports,    "Reports",    ref navY); btnNavReports.Click    += BtnNavReports_Click;
        SetupNavButton(btnNavUsers,      "Users",      ref navY); btnNavUsers.Click      += BtnNavUsers_Click;

        pnlMenu.Controls.AddRange(new Control[] {
            btnNavProject, btnNavPayments, btnNavTasks, btnNavAttendance, btnNavPayroll,
            btnNavEquipment, btnNavMaterials, btnNavContacts,
            btnNavReports, btnNavUsers
        });

        // ── Main area ─────────────────────────────────────────────────────────
        pnlMain.Dock      = DockStyle.Fill;
        pnlMain.BackColor = Color.White;
        pnlMain.Padding   = new Padding(20);
        pnlMain.Controls.Add(tlpCards);

        // Responsive card row: 3 equal columns × 1 row, fills the dashboard area
        // and resizes automatically as the window grows.
        tlpCards.Dock        = DockStyle.Top;
        tlpCards.Height      = 280;
        tlpCards.ColumnCount = 3;
        tlpCards.RowCount    = 1;
        tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
        tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        tlpCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tlpCards.BackColor   = Color.White;

        // Upcoming Deadlines
        grpDeadlines.Text     = "Upcoming Deadlines";
        grpDeadlines.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpDeadlines.Dock     = DockStyle.Fill;
        grpDeadlines.Margin   = new Padding(0, 0, 8, 0);
        grpDeadlines.Controls.Add(rtbDeadlines);

        rtbDeadlines.ReadOnly    = true;
        rtbDeadlines.BorderStyle = BorderStyle.None;
        rtbDeadlines.BackColor   = Color.White;
        rtbDeadlines.Font        = new Font("Segoe UI", 9F);
        rtbDeadlines.ScrollBars  = RichTextBoxScrollBars.Vertical;
        rtbDeadlines.DetectUrls  = false;
        rtbDeadlines.Dock        = DockStyle.Fill;
        rtbDeadlines.Padding     = new Padding(4, 2, 4, 2);

        // Maintenance Due
        grpMaintenance.Text     = "Maintenance Due";
        grpMaintenance.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpMaintenance.Dock     = DockStyle.Fill;
        grpMaintenance.Margin   = new Padding(4, 0, 4, 0);
        grpMaintenance.Controls.Add(rtbMaintenance);

        rtbMaintenance.ReadOnly    = true;
        rtbMaintenance.BorderStyle = BorderStyle.None;
        rtbMaintenance.BackColor   = Color.White;
        rtbMaintenance.Font        = new Font("Segoe UI", 9F);
        rtbMaintenance.ScrollBars  = RichTextBoxScrollBars.Vertical;
        rtbMaintenance.DetectUrls  = false;
        rtbMaintenance.Dock        = DockStyle.Fill;
        rtbMaintenance.Padding     = new Padding(4, 2, 4, 2);

        // Low Stock
        grpLowStock.Text     = "Low Stock";
        grpLowStock.Font     = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        grpLowStock.Dock     = DockStyle.Fill;
        grpLowStock.Margin   = new Padding(8, 0, 0, 0);
        grpLowStock.Controls.Add(rtbLowStock);

        rtbLowStock.ReadOnly    = true;
        rtbLowStock.BorderStyle = BorderStyle.None;
        rtbLowStock.BackColor   = Color.White;
        rtbLowStock.Font        = new Font("Segoe UI", 9F);
        rtbLowStock.ScrollBars  = RichTextBoxScrollBars.Vertical;
        rtbLowStock.DetectUrls  = false;
        rtbLowStock.Dock        = DockStyle.Fill;
        rtbLowStock.Padding     = new Padding(4, 2, 4, 2);

        // Place the 3 GroupBoxes into the 3 columns (col, row).
        tlpCards.Controls.Add(grpDeadlines,   0, 0);
        tlpCards.Controls.Add(grpMaintenance, 1, 0);
        tlpCards.Controls.Add(grpLowStock,    2, 0);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(1000, 600);
        Text          = "Construction Management - Dashboard";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize   = new Size(900, 520);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlMain);
        Controls.Add(pnlMenu);
        Controls.Add(pnlHeader);
        Controls.Add(mainMenu);
        MainMenuStrip = mainMenu;

        ResumeLayout(false);
        PerformLayout();
    }

    /// <summary>Applies consistent style and position to a nav button.</summary>
    private static void SetupNavButton(Button btn, string text, ref int y)
    {
        btn.Text      = text;
        btn.Font      = new Font("Segoe UI", 9.5F);
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = Color.LightGray;
        btn.BackColor = Color.White;
        btn.TextAlign = ContentAlignment.MiddleLeft;
        btn.Padding   = new Padding(8, 0, 0, 0);
        btn.Location  = new Point(10, y);
        btn.Size      = new Size(160, 34);
        btn.Cursor    = Cursors.Hand;
        y += 38;
    }
}
