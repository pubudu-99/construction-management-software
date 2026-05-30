using System.Text.RegularExpressions;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for FR7 — Client and Supplier Contact Management.
/// </summary>
public class ContactService
{
    private readonly ContactRepository _repo;

    /// <summary>Initialises the service with a contact repository.</summary>
    public ContactService(ContactRepository repo) => _repo = repo;

    /// <summary>
    /// Validates and saves a contact record.
    /// Inserts when <c>ContactId == 0</c>; updates otherwise.
    /// </summary>
    /// <returns><see cref="ContactResult.Ok"/> or a <see cref="ContactResult.Fail"/> with a reason.</returns>
    public ContactResult Save(Contact c)
    {
        if (string.IsNullOrWhiteSpace(c.Name))
            return ContactResult.Fail("Contact name is required.");

        if (!string.IsNullOrWhiteSpace(c.Email) && !IsValidEmail(c.Email))
            return ContactResult.Fail("Email address format is invalid.");

        if (!string.IsNullOrWhiteSpace(c.Phone) && !IsValidPhone(c.Phone))
            return ContactResult.Fail("Phone number format is invalid.");

        if (c.ContactId == 0)
            _repo.Insert(c);
        else
            _repo.Update(c);

        return ContactResult.Ok();
    }

    /// <summary>
    /// Filters a contact list in-memory by name, phone, or email.
    /// The search is case-insensitive. Returns all contacts when
    /// <paramref name="term"/> is null or white-space.
    /// </summary>
    public static List<Contact> Search(string? term, List<Contact> contacts)
    {
        if (string.IsNullOrWhiteSpace(term)) return contacts;

        var t = term.Trim();
        return contacts
            .Where(c =>
                c.Name.Contains(t, StringComparison.OrdinalIgnoreCase)          ||
                (c.Phone  ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ||
                (c.Email  ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ||
                (c.ContactPerson ?? "").Contains(t, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private static bool IsValidPhone(string phone) =>
        Regex.IsMatch(phone.Trim(), @"^\+?[\d\s\-\(\)]{7,20}$");
}
