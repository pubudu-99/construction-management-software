using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="User"/> entity.
/// This is the only class permitted to query the Users table.
/// </summary>
public class UserRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>
    /// Initialises the repository with a connection factory.
    /// </summary>
    /// <param name="factory">The factory used to open SQLite connections.</param>
    public UserRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Finds a user by their username using a parameterised query.
    /// </summary>
    /// <param name="username">The username to search for (case-sensitive).</param>
    /// <returns>
    /// The matching <see cref="User"/>, or <c>null</c> if no row is found.
    /// </returns>
    public User? FindByUsername(string username)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();

        cmd.CommandText = @"
            SELECT UserId, Username, PasswordHash, FullName, Role, IsActive, MustChangePassword
            FROM   Users
            WHERE  Username = $u
            LIMIT  1;
        ";
        cmd.Parameters.AddWithValue("$u", username);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        return MapRow(reader);
    }

    /// <summary>
    /// Returns every user in the database, ordered by full name (case-insensitive).
    /// </summary>
    /// <returns>The full list of users (including inactive ones).</returns>
    public List<User> GetAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT UserId, Username, PasswordHash, FullName, Role, IsActive, MustChangePassword
            FROM   Users
            ORDER  BY FullName COLLATE NOCASE ASC;
        ";

        var list = new List<User>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(MapRow(rd));
        return list;
    }

    /// <summary>
    /// Inserts a new user row. The caller is responsible for hashing the password
    /// and ensuring the username is unique (use <see cref="UsernameExists"/> first).
    /// </summary>
    /// <param name="u">The user to insert. <c>UserId</c> is ignored.</param>
    public void Insert(User u)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, MustChangePassword)
            VALUES ($u, $h, $n, $r, $a, $m);
        ";
        cmd.Parameters.AddWithValue("$u", u.Username);
        cmd.Parameters.AddWithValue("$h", u.PasswordHash);
        cmd.Parameters.AddWithValue("$n", u.FullName);
        cmd.Parameters.AddWithValue("$r", u.Role);
        cmd.Parameters.AddWithValue("$a", u.IsActive ? 1 : 0);
        cmd.Parameters.AddWithValue("$m", u.MustChangePassword ? 1 : 0);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Updates an existing user's editable fields (full name, role, active status).
    /// Username and password hash have separate dedicated update flows and are
    /// intentionally NOT modified here.
    /// </summary>
    /// <param name="u">The user with the new values; <c>UserId</c> selects the row.</param>
    public void Update(User u)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Users
            SET    FullName = $n,
                   Role     = $r,
                   IsActive = $a
            WHERE  UserId   = $id;
        ";
        cmd.Parameters.AddWithValue("$n",  u.FullName);
        cmd.Parameters.AddWithValue("$r",  u.Role);
        cmd.Parameters.AddWithValue("$a",  u.IsActive ? 1 : 0);
        cmd.Parameters.AddWithValue("$id", u.UserId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns <c>true</c> if a row with the given username already exists.
    /// Used to validate uniqueness before <see cref="Insert"/>.
    /// </summary>
    /// <param name="username">The username to check (case-sensitive).</param>
    public bool UsernameExists(string username)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = $u;";
        cmd.Parameters.AddWithValue("$u", username);
        long count = (long)(cmd.ExecuteScalar() ?? 0L);
        return count > 0;
    }

    /// <summary>
    /// Replaces the stored password hash for the given user.
    /// Only called after the current password has already been verified.
    /// </summary>
    /// <param name="userId">The ID of the user whose hash will be updated.</param>
    /// <param name="newHash">The new BCrypt hash to store.</param>
    public void UpdatePasswordHash(int userId, string newHash)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Users
            SET    PasswordHash = $h
            WHERE  UserId = $id;
        ";
        cmd.Parameters.AddWithValue("$h",  newHash);
        cmd.Parameters.AddWithValue("$id", userId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Sets or clears the <c>MustChangePassword</c> flag for a user.
    /// </summary>
    /// <param name="userId">The user to update.</param>
    /// <param name="mustChange"><c>true</c> to force a change at next login.</param>
    public void SetMustChangePassword(int userId, bool mustChange)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Users
            SET    MustChangePassword = $m
            WHERE  UserId = $id;
        ";
        cmd.Parameters.AddWithValue("$m",  mustChange ? 1 : 0);
        cmd.Parameters.AddWithValue("$id", userId);
        cmd.ExecuteNonQuery();
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>Maps the current reader row to a <see cref="User"/>.</summary>
    private static User MapRow(SqliteDataReader rd) => new()
    {
        UserId             = rd.GetInt32(0),
        Username           = rd.GetString(1),
        PasswordHash       = rd.GetString(2),
        FullName           = rd.GetString(3),
        Role               = rd.GetString(4),
        IsActive           = rd.GetInt32(5) == 1,
        MustChangePassword = rd.GetInt32(6) == 1
    };
}
