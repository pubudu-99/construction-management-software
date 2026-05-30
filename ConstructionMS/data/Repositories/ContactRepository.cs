using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Contact"/> entity.
/// </summary>
public class ContactRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public ContactRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>Returns all contacts ordered by name.</summary>
    public List<Contact> GetAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ContactId, Type, Name, ContactPerson, Phone, Email, Address, Notes
            FROM   Contacts
            ORDER  BY Name ASC;
        ";
        var list = new List<Contact>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) list.Add(MapRow(rd));
        return list;
    }

    /// <summary>Returns all contacts of a given type (e.g. "Supplier").</summary>
    public List<Contact> GetByType(string type)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ContactId, Type, Name, ContactPerson, Phone, Email, Address, Notes
            FROM   Contacts
            WHERE  Type = $type
            ORDER  BY Name ASC;
        ";
        cmd.Parameters.AddWithValue("$type", type);
        var list = new List<Contact>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read()) list.Add(MapRow(rd));
        return list;
    }

    /// <summary>Returns a single contact by ID, or null if not found.</summary>
    public Contact? GetById(int id)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT ContactId, Type, Name, ContactPerson, Phone, Email, Address, Notes
            FROM   Contacts WHERE ContactId = $id LIMIT 1;
        ";
        cmd.Parameters.AddWithValue("$id", id);
        using var rd = cmd.ExecuteReader();
        return rd.Read() ? MapRow(rd) : null;
    }

    /// <summary>Inserts a new contact record.</summary>
    public void Insert(Contact c)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Contacts (Type, Name, ContactPerson, Phone, Email, Address, Notes)
            VALUES ($type, $name, $cp, $ph, $em, $addr, $notes);
        ";
        BindParams(cmd, c);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Updates all fields of an existing contact record.</summary>
    public void Update(Contact c)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Contacts
            SET    Type=$type, Name=$name, ContactPerson=$cp,
                   Phone=$ph, Email=$em, Address=$addr, Notes=$notes
            WHERE  ContactId=$id;
        ";
        cmd.Parameters.AddWithValue("$id", c.ContactId);
        BindParams(cmd, c);
        cmd.ExecuteNonQuery();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void BindParams(SqliteCommand cmd, Contact c)
    {
        cmd.Parameters.AddWithValue("$type",  c.Type);
        cmd.Parameters.AddWithValue("$name",  c.Name);
        cmd.Parameters.AddWithValue("$cp",    (object?)c.ContactPerson ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$ph",    (object?)c.Phone         ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$em",    (object?)c.Email         ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$addr",  (object?)c.Address       ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$notes", (object?)c.Notes         ?? DBNull.Value);
    }

    private static Contact MapRow(SqliteDataReader rd) => new()
    {
        ContactId     = rd.GetInt32(0),
        Type          = rd.GetString(1),
        Name          = rd.GetString(2),
        ContactPerson = rd.IsDBNull(3) ? null : rd.GetString(3),
        Phone         = rd.IsDBNull(4) ? null : rd.GetString(4),
        Email         = rd.IsDBNull(5) ? null : rd.GetString(5),
        Address       = rd.IsDBNull(6) ? null : rd.GetString(6),
        Notes         = rd.IsDBNull(7) ? null : rd.GetString(7)
    };
}
