using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for FR1.1 (User Registration) and FR1.3 (Role-Based Access).
/// Validates input, hashes passwords, and delegates persistence to
/// <see cref="UserRepository"/>.
/// </summary>
public class UserService
{
    private readonly UserRepository _repo;

    /// <summary>Allowed role values for the application.</summary>
    private static readonly HashSet<string> AllowedRoles =
        new(StringComparer.Ordinal) { "Manager", "Supervisor" };

    /// <summary>Initialises the service with a user repository.</summary>
    public UserService(UserRepository repo) => _repo = repo;

    /// <summary>
    /// Returns every user in the system, ordered by full name.
    /// </summary>
    public List<User> GetAll() => _repo.GetAll();

    /// <summary>
    /// Validates inputs, hashes the password with BCrypt (work factor 12),
    /// and inserts a new active user.
    /// </summary>
    /// <param name="username">The login name. Must be unique and non-empty.</param>
    /// <param name="password">Plain-text password (will be hashed). Min 6 characters.</param>
    /// <param name="fullName">The user's display name. Must be non-empty.</param>
    /// <param name="role">Must be "Manager" or "Supervisor".</param>
    /// <returns><see cref="UserResult.Ok"/> on success; otherwise a failure with reason.</returns>
    public UserResult CreateUser(string username, string password, string fullName, string role)
    {
        if (string.IsNullOrWhiteSpace(username))
            return UserResult.Fail("Username is required.");

        if (password is null || password.Length < 6)
            return UserResult.Fail("Password must be at least 6 characters.");

        if (string.IsNullOrWhiteSpace(fullName))
            return UserResult.Fail("Full name is required.");

        if (!AllowedRoles.Contains(role))
            return UserResult.Fail("Role must be Manager or Supervisor.");

        if (_repo.UsernameExists(username))
            return UserResult.Fail("Username already taken.");

        string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        _repo.Insert(new User
        {
            Username           = username.Trim(),
            PasswordHash       = hash,
            FullName           = fullName.Trim(),
            Role               = role,
            IsActive           = true,
            // The supplied password is temporary — force a change at first login.
            MustChangePassword = true
        });

        return UserResult.Ok();
    }

    /// <summary>
    /// Resets a user's password to a new temporary value and forces a change
    /// at their next login. Used by a Manager when a user forgets their password.
    /// The Manager never sees the user's chosen password (it is hashed).
    /// </summary>
    /// <param name="userId">The user whose password is being reset.</param>
    /// <param name="tempPassword">The new temporary password (min 6 characters).</param>
    public UserResult ResetPassword(int userId, string tempPassword)
    {
        if (tempPassword is null || tempPassword.Length < 6)
            return UserResult.Fail("Temporary password must be at least 6 characters.");

        string hash = BCrypt.Net.BCrypt.HashPassword(tempPassword, workFactor: 12);
        _repo.UpdatePasswordHash(userId, hash);
        _repo.SetMustChangePassword(userId, true);
        return UserResult.Ok();
    }

    /// <summary>
    /// Validates the editable fields and updates an existing user.
    /// Username and password are intentionally untouched.
    /// </summary>
    /// <param name="u">The user with the new values; <c>UserId</c> selects the row.</param>
    public UserResult UpdateUser(User u)
    {
        if (string.IsNullOrWhiteSpace(u.FullName))
            return UserResult.Fail("Full name is required.");

        if (!AllowedRoles.Contains(u.Role))
            return UserResult.Fail("Role must be Manager or Supervisor.");

        _repo.Update(u);
        return UserResult.Ok();
    }

    /// <summary>
    /// Toggles the <c>IsActive</c> flag on a user (deactivate or reactivate).
    /// Never deletes the row so that audit references remain valid.
    /// </summary>
    /// <param name="userId">The user whose active flag will be flipped.</param>
    /// <param name="active">The new value for IsActive.</param>
    public UserResult SetActive(int userId, bool active)
    {
        var users = _repo.GetAll();
        var u = users.FirstOrDefault(x => x.UserId == userId);
        if (u is null) return UserResult.Fail("User not found.");

        u.IsActive = active;
        _repo.Update(u);
        return UserResult.Ok();
    }
}
