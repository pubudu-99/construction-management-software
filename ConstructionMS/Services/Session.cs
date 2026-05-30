using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Holds the currently authenticated user for the lifetime of the session.
/// Set <see cref="Current"/> on successful login; call <see cref="SignOut"/> on logout.
/// </summary>
public static class Session
{
    /// <summary>
    /// Gets or sets the currently logged-in user.
    /// <c>null</c> means no user is signed in.
    /// </summary>
    public static User? Current { get; set; }

    /// <summary>
    /// Returns <c>true</c> if the current user holds the Manager role.
    /// </summary>
    public static bool IsManager => Current?.Role == "Manager";

    /// <summary>
    /// Clears the current session, effectively signing the user out.
    /// </summary>
    public static void SignOut() => Current = null;
}
