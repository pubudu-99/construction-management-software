namespace ConstructionMS.Forms;

partial class ContactForm
{
    private System.ComponentModel.IContainer components = null;

    // ── Entry panel (left) ────────────────────────────────────────────────────
    private Panel    pnlContactEntry;
    private Label    lblContactEntryTitle;
    private Label    lblCType;
    private ComboBox cmbContactType;
    private Label    lblCName;
    private TextBox  txtContactName;
    private Label    lblCPerson;
    private TextBox  txtContactPerson;
    private Label    lblPhone;
    private TextBox  txtPhone;
    private Label    lblEmail;
    private TextBox  txtEmail;
    private Label    lblAddress;
    private TextBox  txtAddress;
    private Label    lblNotes;
    private TextBox  txtNotes;
    private Button   btnSaveContact;
    private Button   btnClearContact;
    private Label    lblContactStatus;

    // ── Grid panel (right) ────────────────────────────────────────────────────
    private Panel      pnlContactGrid;
    private Label      lblContactListTitle;
    private Panel      pnlSearch;
    private TextBox    txtContactSearch;
    private DataGridView dgvContacts;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlContactEntry      = new Panel();
        lblContactEntryTitle = new Label();
        lblCType             = new Label();
        cmbContactType       = new ComboBox();
        lblCName             = new Label();
        txtContactName       = new TextBox();
        lblCPerson           = new Label();
        txtContactPerson     = new TextBox();
        lblPhone             = new Label();
        txtPhone             = new TextBox();
        lblEmail             = new Label();
        txtEmail             = new TextBox();
        lblAddress           = new Label();
        txtAddress           = new TextBox();
        lblNotes             = new Label();
        txtNotes             = new TextBox();
        btnSaveContact       = new Button();
        btnClearContact      = new Button();
        lblContactStatus     = new Label();

        pnlContactGrid       = new Panel();
        lblContactListTitle  = new Label();
        pnlSearch            = new Panel();
        txtContactSearch     = new TextBox();
        dgvContacts          = new DataGridView();

        ((System.ComponentModel.ISupportInitialize)dgvContacts).BeginInit();
        SuspendLayout();

        // ══════════════════════════════════════════════════════════════════════
        // ENTRY PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlContactEntry.Dock      = DockStyle.Left;
        pnlContactEntry.Width     = 340;
        pnlContactEntry.BackColor = Color.FromArgb(248, 249, 250);

        const int px = 16, pw = 308;
        int y = 10;

        lblContactEntryTitle.Text      = "Add / Edit Contact";
        lblContactEntryTitle.Font      = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblContactEntryTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblContactEntryTitle.Location  = new Point(px, y);
        lblContactEntryTitle.Size      = new Size(pw, 24);
        y += 36;

        // Type
        lblCType.Text      = "Type";
        lblCType.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCType.ForeColor = Color.FromArgb(60, 60, 60);
        lblCType.Location  = new Point(px, y);
        lblCType.Size      = new Size(pw, 18);
        y += 20;
        cmbContactType.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbContactType.Items.AddRange(new object[] { "Client", "Supplier", "Subcontractor", "Other" });
        cmbContactType.SelectedIndex = 0;
        cmbContactType.Font          = new Font("Segoe UI", 9.5F);
        cmbContactType.Location      = new Point(px, y);
        cmbContactType.Size          = new Size(pw, 26);
        y += 34;

        // Name
        lblCName.Text      = "Name  *";
        lblCName.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCName.ForeColor = Color.FromArgb(60, 60, 60);
        lblCName.Location  = new Point(px, y);
        lblCName.Size      = new Size(pw, 18);
        y += 20;
        txtContactName.Font     = new Font("Segoe UI", 9.5F);
        txtContactName.Location = new Point(px, y);
        txtContactName.Size     = new Size(pw, 26);
        y += 34;

        // Contact Person
        lblCPerson.Text      = "Contact Person";
        lblCPerson.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCPerson.ForeColor = Color.FromArgb(60, 60, 60);
        lblCPerson.Location  = new Point(px, y);
        lblCPerson.Size      = new Size(pw, 18);
        y += 20;
        txtContactPerson.Font            = new Font("Segoe UI", 9.5F);
        txtContactPerson.Location        = new Point(px, y);
        txtContactPerson.Size            = new Size(pw, 26);
        txtContactPerson.PlaceholderText = "e.g. Mr. Perera";
        y += 34;

        // Phone
        lblPhone.Text      = "Phone";
        lblPhone.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPhone.ForeColor = Color.FromArgb(60, 60, 60);
        lblPhone.Location  = new Point(px, y);
        lblPhone.Size      = new Size(pw, 18);
        y += 20;
        txtPhone.Font            = new Font("Segoe UI", 9.5F);
        txtPhone.Location        = new Point(px, y);
        txtPhone.Size            = new Size(pw, 26);
        txtPhone.PlaceholderText = "+94 77 000 0000";
        y += 34;

        // Email
        lblEmail.Text      = "Email";
        lblEmail.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblEmail.ForeColor = Color.FromArgb(60, 60, 60);
        lblEmail.Location  = new Point(px, y);
        lblEmail.Size      = new Size(pw, 18);
        y += 20;
        txtEmail.Font            = new Font("Segoe UI", 9.5F);
        txtEmail.Location        = new Point(px, y);
        txtEmail.Size            = new Size(pw, 26);
        txtEmail.PlaceholderText = "name@example.com";
        y += 34;

        // Address
        lblAddress.Text      = "Address";
        lblAddress.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAddress.ForeColor = Color.FromArgb(60, 60, 60);
        lblAddress.Location  = new Point(px, y);
        lblAddress.Size      = new Size(pw, 18);
        y += 20;
        txtAddress.Font     = new Font("Segoe UI", 9.5F);
        txtAddress.Location = new Point(px, y);
        txtAddress.Size     = new Size(pw, 26);
        y += 34;

        // Notes
        lblNotes.Text      = "Notes";
        lblNotes.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblNotes.ForeColor = Color.FromArgb(60, 60, 60);
        lblNotes.Location  = new Point(px, y);
        lblNotes.Size      = new Size(pw, 18);
        y += 20;
        txtNotes.Font        = new Font("Segoe UI", 9.5F);
        txtNotes.Multiline   = true;
        txtNotes.ScrollBars  = ScrollBars.Vertical;
        txtNotes.Location    = new Point(px, y);
        txtNotes.Size        = new Size(pw, 52);
        y += 60;

        // Buttons
        btnSaveContact.Text                      = "Save";
        btnSaveContact.Font                      = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveContact.BackColor                 = Color.FromArgb(40, 167, 69);
        btnSaveContact.ForeColor                 = Color.White;
        btnSaveContact.FlatStyle                 = FlatStyle.Flat;
        btnSaveContact.FlatAppearance.BorderSize = 0;
        btnSaveContact.Location                  = new Point(px, y);
        btnSaveContact.Size                      = new Size(100, 34);
        btnSaveContact.Cursor                    = Cursors.Hand;
        btnSaveContact.Click                    += BtnSaveContact_Click;

        btnClearContact.Text                      = "Clear";
        btnClearContact.Font                      = new Font("Segoe UI", 9.5F);
        btnClearContact.BackColor                 = Color.FromArgb(108, 117, 125);
        btnClearContact.ForeColor                 = Color.White;
        btnClearContact.FlatStyle                 = FlatStyle.Flat;
        btnClearContact.FlatAppearance.BorderSize = 0;
        btnClearContact.Location                  = new Point(px + 110, y);
        btnClearContact.Size                      = new Size(80, 34);
        btnClearContact.Cursor                    = Cursors.Hand;
        btnClearContact.Click                    += BtnClearContact_Click;
        y += 44;

        lblContactStatus.Text      = "";
        lblContactStatus.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblContactStatus.Location  = new Point(px, y);
        lblContactStatus.Size      = new Size(pw, 22);
        lblContactStatus.Visible   = false;

        pnlContactEntry.Controls.AddRange(new Control[] {
            lblContactEntryTitle,
            lblCType, cmbContactType,
            lblCName, txtContactName,
            lblCPerson, txtContactPerson,
            lblPhone, txtPhone,
            lblEmail, txtEmail,
            lblAddress, txtAddress,
            lblNotes, txtNotes,
            btnSaveContact, btnClearContact,
            lblContactStatus
        });

        // ══════════════════════════════════════════════════════════════════════
        // GRID PANEL
        // ══════════════════════════════════════════════════════════════════════
        pnlContactGrid.Dock      = DockStyle.Fill;
        pnlContactGrid.BackColor = Color.White;
        pnlContactGrid.Padding   = new Padding(12, 8, 12, 12);

        lblContactListTitle.Text      = "Contact List";
        lblContactListTitle.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblContactListTitle.ForeColor = Color.FromArgb(30, 90, 160);
        lblContactListTitle.Dock      = DockStyle.Top;
        lblContactListTitle.Height    = 24;

        pnlSearch.Dock      = DockStyle.Top;
        pnlSearch.Height    = 40;
        pnlSearch.BackColor = Color.White;

        txtContactSearch.Font            = new Font("Segoe UI", 9.5F);
        txtContactSearch.Dock            = DockStyle.Fill;
        txtContactSearch.PlaceholderText = "Search by name, phone or email...";
        txtContactSearch.TextChanged    += TxtContactSearch_TextChanged;

        pnlSearch.Controls.Add(txtContactSearch);

        dgvContacts.Dock             = DockStyle.Fill;
        dgvContacts.CellDoubleClick += DgvContacts_CellDoubleClick;

        pnlContactGrid.Controls.Add(dgvContacts);
        pnlContactGrid.Controls.Add(pnlSearch);
        pnlContactGrid.Controls.Add(lblContactListTitle);

        // ── Form ──────────────────────────────────────────────────────────────
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize    = new Size(1000, 580);
        Text          = "Contacts";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize   = new Size(820, 500);
        Font          = new Font("Segoe UI", 9F);
        BackColor     = Color.White;

        Controls.Add(pnlContactGrid);
        Controls.Add(pnlContactEntry);

        ((System.ComponentModel.ISupportInitialize)dgvContacts).EndInit();
        ResumeLayout(false);
    }
}
