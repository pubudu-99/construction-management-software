using ConstructionMS.Data;
using ConstructionMS.Forms;

namespace ConstructionMS;

/// <summary>
/// Application entry point.
/// </summary>
static class Program
{
    /// <summary>
    /// Bootstraps the database and launches the sign-in form.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 1. Resolve DB path and ensure the db/ folder exists.
        var factory = new DbConnectionFactory();

        // 2. Create tables and seed default admin on first run.
        //    Safe to call every launch — uses CREATE TABLE IF NOT EXISTS.
        new DatabaseInitializer(factory).Run();

        // 3. Start the application at the sign-in form.
        Application.Run(new LoginForm(factory));
    }
}
