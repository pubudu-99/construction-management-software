using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;
using ConstructionMS.Services;

namespace ConstructionMS.Forms;

/// <summary>
/// FR7 — Client and Supplier Contact Management form.
/// Left panel: add or edit a contact.
/// Right panel: live search and grid.  Double-click a row to edit it.
/// </summary>
public partial class ContactForm : Form
{
    private readonly ContactService    _service;
    private readonly ContactRepository _repo;

    /// <summary>Tracks the ID of the contact being edited; 0 means adding new.</summary>
    private int _editingContactId;

    /// <summary>All contacts loaded from the database; searched in-memory.</summary>
    private List<Contact> _allContacts = new();

    /// <summary>Initialises the form with the given repository.</summary>
    public ContactForm(ContactRepository repo)
    {
        _repo    = repo;
        _service = new ContactService(repo);
        InitializeComponent();
        GridStyle.Apply(dgvContacts);
        Theme.Apply(this);
        Load += ContactForm_Load;
    }

    // ── Load ──────────────────────────────────────────────────────────────────

    /// <summary>Loads the contacts list when the form is first shown.</summary>
    private void ContactForm_Load(object? sender, EventArgs e) => LoadContacts();

    // ── Data loading ──────────────────────────────────────────────────────────

    /// <summary>Loads all contacts from the database and refreshes the grid.</summary>
    private void LoadContacts()
    {
        _allContacts = _repo.GetAll();
        ApplySearch(txtContactSearch.Text);
    }

    /// <summary>Filters _allContacts by the search term and binds the result to the grid.</summary>
    private void ApplySearch(string? term)
    {
        var filtered = ContactService.Search(term, _allContacts)
            .Select(c => new
            {
                c.ContactId,
                c.Type,
                c.Name,
                ContactPerson = c.ContactPerson ?? "",
                Phone         = c.Phone   ?? "",
                Email         = c.Email   ?? "",
                Address       = c.Address ?? ""
            })
            .ToList();

        dgvContacts.DataSource = filtered;

        if (dgvContacts.Columns.Count == 0) return;

        SetColVisible("ContactId",     false);
        SetColHeader("Type",           "Type");
        SetColHeader("Name",           "Name");
        SetColHeader("ContactPerson",  "Contact Person");
        SetColHeader("Phone",          "Phone");
        SetColHeader("Email",          "Email");
        SetColHeader("Address",        "Address");

        SetColWeight("Type",           100);
        SetColWeight("Name",           180);
        SetColWeight("ContactPerson",  140);
        SetColWeight("Phone",          120);
        SetColWeight("Email",          180);
        SetColWeight("Address",        180);
    }

    // ── Entry panel events ────────────────────────────────────────────────────

    /// <summary>Validates and saves (insert or update) the contact from the entry panel.</summary>
    private void BtnSaveContact_Click(object sender, EventArgs e)
    {
        var contact = new Contact
        {
            ContactId     = _editingContactId,
            Type          = cmbContactType.SelectedItem?.ToString() ?? "Client",
            Name          = txtContactName.Text.Trim(),
            ContactPerson = txtContactPerson.Text.Trim() is { Length: > 0 } cp ? cp : null,
            Phone         = txtPhone.Text.Trim()   is { Length: > 0 } ph ? ph : null,
            Email         = txtEmail.Text.Trim()   is { Length: > 0 } em ? em : null,
            Address       = txtAddress.Text.Trim() is { Length: > 0 } ad ? ad : null,
            Notes         = txtNotes.Text.Trim()   is { Length: > 0 } nt ? nt : null
        };

        var result = _service.Save(contact);

        if (!result.Success)
        {
            ShowStatus(result.Message, success: false);
            return;
        }

        string verb = _editingContactId == 0 ? "added" : "updated";
        ShowStatus($"Contact {verb} successfully.", success: true);
        ClearEntry();
        LoadContacts();
    }

    /// <summary>Resets the entry panel to the "add new" state.</summary>
    private void BtnClearContact_Click(object sender, EventArgs e) => ClearEntry();

    // ── Grid events ───────────────────────────────────────────────────────────

    /// <summary>Populates the entry panel when the user double-clicks a contact row.</summary>
    private void DgvContacts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvContacts.Rows[e.RowIndex].Cells["ContactId"]?.Value is not int id) return;

        var c = _repo.GetById(id);
        if (c is null) return;

        _editingContactId = c.ContactId;

        for (int i = 0; i < cmbContactType.Items.Count; i++)
        {
            if (cmbContactType.Items[i]?.ToString() == c.Type)
            {
                cmbContactType.SelectedIndex = i;
                break;
            }
        }

        txtContactName.Text   = c.Name;
        txtContactPerson.Text = c.ContactPerson ?? "";
        txtPhone.Text         = c.Phone         ?? "";
        txtEmail.Text         = c.Email         ?? "";
        txtAddress.Text       = c.Address       ?? "";
        txtNotes.Text         = c.Notes         ?? "";

        lblContactEntryTitle.Text = "Edit Contact";
        btnSaveContact.Text       = "Update";
        txtContactName.Focus();
    }

    /// <summary>Filters the grid live as the user types in the search box.</summary>
    private void TxtContactSearch_TextChanged(object sender, EventArgs e)
    {
        ApplySearch(txtContactSearch.Text);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void ClearEntry()
    {
        _editingContactId = 0;

        cmbContactType.SelectedIndex = 0;
        txtContactName.Clear();
        txtContactPerson.Clear();
        txtPhone.Clear();
        txtEmail.Clear();
        txtAddress.Clear();
        txtNotes.Clear();

        lblContactEntryTitle.Text = "Add / Edit Contact";
        btnSaveContact.Text       = "Save";
    }

    private void ShowStatus(string message, bool success)
    {
        lblContactStatus.Text      = message;
        lblContactStatus.ForeColor = success
            ? Color.FromArgb(40, 167, 69)
            : Color.FromArgb(220, 53, 69);
        lblContactStatus.Visible   = true;

        var timer = new System.Windows.Forms.Timer { Interval = 3000 };
        timer.Tick += (_, _) =>
        {
            lblContactStatus.Visible = false;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    // Null-safe column helpers
    private void SetColVisible(string name, bool v) { if (dgvContacts.Columns[name] is { } c) c.Visible = v; }
    private void SetColHeader(string name, string h) { if (dgvContacts.Columns[name] is { } c) c.HeaderText = h; }
    private void SetColWeight(string name, int w)   { if (dgvContacts.Columns[name] is { } c) c.FillWeight = w; }
}
