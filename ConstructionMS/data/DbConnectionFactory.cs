using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data;

/// <summary>
/// Resolves the SQLite database path and creates open connections on demand.
/// All repositories receive this factory via constructor injection.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _dbPath;

    /// <summary>
    /// Locates (or creates) the <c>db/</c> folder next to the executable
    /// and sets the full path to <c>construction.db</c>.
    /// </summary>
    public DbConnectionFactory()
    {
        // BaseDirectory = bin/Debug/net8.0-windows/ at runtime.
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string dbDir   = Path.Combine(baseDir, "db");

        // Create the folder if this is a first run on a new machine.
        Directory.CreateDirectory(dbDir);

        _dbPath = Path.Combine(dbDir, "construction.db");
    }

    /// <summary>
    /// Gets the full file-system path to the SQLite database file.
    /// </summary>
    public string DatabasePath => _dbPath;

    /// <summary>
    /// Creates a new <see cref="SqliteConnection"/>, opens it, and returns it.
    /// The caller is responsible for disposing the connection (use a <c>using</c> block).
    /// </summary>
    /// <returns>An open <see cref="SqliteConnection"/>.</returns>
    public SqliteConnection Open()
    {
        var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();
        return connection;
    }
}
