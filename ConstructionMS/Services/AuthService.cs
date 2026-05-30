using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Handles authentication and role enforcement for the application.
/// Forms must call this service — they must never verify passwords directly.
/// </summary>
public class AuthService
{
    private readonly UserRepository _users;

    /// <summary>
    /// Initialises the service with a user repository.
    /// </summary>
    /// <param name="users">The repository used to look up users.</param>
    public AuthService(UserRepository users)
    {
        _users = users;
    }

    /// <summary>
    /// Attempts to log in with the supplied credentials.
    /// </summary>
    /// <param name="username">The username entered by the user.</param>
    /// <param name="password">The plain-text password entered by the user.</param>
    /// <returns>
    /// The authenticated <see cref="User"/> if credentials are valid and the
    /// account is active; otherwise <c>null</c>.
    /// </returns>
    public User? Login(string username, string password)
    {
        // Reject blank inputs before touching the database.
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        User? user = _users.FindByUsername(username);

        if (user is null || !user.IsActive)
            return null;

        // BCrypt.Verify compares the plain-text password against the stored hash.
        bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return passwordValid ? user : null;
    }

    /// <summary>
    /// Checks whether the currently logged-in user holds the specified role.
    /// </summary>
    /// <param name="role">The required role (e.g. "Manager").</param>
    /// <returns>
    /// <c>true</c> if a user is signed in and their role matches
    /// <paramref name="role"/> (case-insensitive); otherwise <c>false</c>.
    /// </returns>
    public bool RequireRole(string role)
    {
        return Session.Current is not null &&
               string.Equals(Session.Current.Role, role, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifies the user's current password and, if valid, stores a new BCrypt hash.
    /// Works both for the forced first-login change and the voluntary change flow.
    /// </summary>
    /// <param name="userId">The ID of the user changing their password.</param>
    /// <param name="oldPassword">The current plain-text password to verify.</param>
    /// <param name="newPassword">The new plain-text password to hash and store.</param>
    /// <returns>
    /// <c>true</c> if the old password verified and the new hash was saved;
    /// <c>false</c> if the user was not found or the old password was incorrect.
    /// </returns>
    public bool ChangePassword(int userId, string oldPassword, string newPassword)
    {
        // Prefer the in-memory session user; otherwise load from the database
        // (the forced first-login flow may run before Session.Current is set).
        User? user = Session.Current?.UserId == userId
            ? Session.Current
            : _users.GetAll().FirstOrDefault(u => u.UserId == userId);

        if (user is null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            return false;

        string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);
        _users.UpdatePasswordHash(userId, newHash);

        // The user has now chosen their own password — clear the force-change flag.
        _users.SetMustChangePassword(userId, false);

        // Keep the in-memory object(s) current so a later verify still works.
        user.PasswordHash       = newHash;
        user.MustChangePassword = false;
        if (Session.Current is not null && Session.Current.UserId == userId)
        {
            Session.Current.PasswordHash       = newHash;
            Session.Current.MustChangePassword = false;
        }

        return true;
    }
}
