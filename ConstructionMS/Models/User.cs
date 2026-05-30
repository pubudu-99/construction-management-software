namespace ConstructionMS.Models;

/// <summary>
/// Represents a system user who can log in (Manager or Supervisor role).
/// </summary>
public class User
{
    public int    UserId       { get; set; }
    public string Username     { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FullName     { get; set; } = "";
    public string Role         { get; set; } = "";
    public bool   IsActive     { get; set; }

    /// <summary>
    /// When <c>true</c>, the user must change their password at next login
    /// before reaching the dashboard. Set on creation and password reset;
    /// cleared after a successful change.
    /// </summary>
    public bool   MustChangePassword { get; set; }
}
