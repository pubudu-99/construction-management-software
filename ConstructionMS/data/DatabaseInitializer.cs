using BCrypt.Net;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data;

/// <summary>
/// Runs once at application startup to create all database tables and
/// seed the default administrator account if the database is empty.
/// Safe to call on every launch — all statements use IF NOT EXISTS.
/// </summary>
public class DatabaseInitializer
{
    private readonly DbConnectionFactory _factory;

    /// <summary>
    /// Initialises the <see cref="DatabaseInitializer"/> with a connection factory.
    /// </summary>
    /// <param name="factory">The factory used to open a SQLite connection.</param>
    public DatabaseInitializer(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Creates all tables and indexes, then seeds the default admin if needed.
    /// </summary>
    public void Run()
    {
        using var conn = _factory.Open();
        CreateTables(conn);
        MigrateSchema(conn);
        SeedDefaultAdmin(conn);
        SeedDefaultProject(conn);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Executes all CREATE TABLE IF NOT EXISTS and CREATE INDEX IF NOT EXISTS
    /// statements inside a single transaction.
    /// </summary>
    private static void CreateTables(SqliteConnection conn)
    {
        using var tx  = conn.BeginTransaction();
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;

        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserId       INTEGER PRIMARY KEY AUTOINCREMENT,
                Username     TEXT    NOT NULL UNIQUE,
                PasswordHash TEXT    NOT NULL,
                FullName     TEXT    NOT NULL DEFAULT '',
                Role         TEXT    NOT NULL DEFAULT 'Supervisor',
                IsActive     INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Projects (
                ProjectId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name      TEXT    NOT NULL,
                Budget    REAL    NOT NULL DEFAULT 0,
                Spent     REAL    NOT NULL DEFAULT 0,
                StartDate TEXT    NOT NULL,
                EndDate   TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Payments (
                PaymentId   INTEGER PRIMARY KEY AUTOINCREMENT,
                ProjectId   INTEGER NOT NULL REFERENCES Projects(ProjectId),
                PayeeType   TEXT    NOT NULL DEFAULT '',
                Reference   TEXT    NOT NULL DEFAULT '',
                Amount      REAL    NOT NULL DEFAULT 0,
                PaymentDate TEXT    NOT NULL,
                Note        TEXT
            );

            CREATE TABLE IF NOT EXISTS Tasks (
                TaskId     INTEGER PRIMARY KEY AUTOINCREMENT,
                ProjectId  INTEGER NOT NULL REFERENCES Projects(ProjectId),
                Name       TEXT    NOT NULL DEFAULT '',
                StartDate  TEXT    NOT NULL,
                EndDate    TEXT    NOT NULL,
                AssigneeId INTEGER REFERENCES Workers(WorkerId),
                Status     TEXT    NOT NULL DEFAULT 'Pending'
            );

            CREATE TABLE IF NOT EXISTS Workers (
                WorkerId   INTEGER PRIMARY KEY AUTOINCREMENT,
                Name       TEXT    NOT NULL DEFAULT '',
                HourlyRate REAL    NOT NULL DEFAULT 0,
                IsActive   INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE IF NOT EXISTS Attendance (
                AttendanceId INTEGER PRIMARY KEY AUTOINCREMENT,
                WorkerId     INTEGER NOT NULL REFERENCES Workers(WorkerId),
                Date         TEXT    NOT NULL,
                Hours        REAL    NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Equipment (
                EquipmentId     INTEGER PRIMARY KEY AUTOINCREMENT,
                Name            TEXT NOT NULL DEFAULT '',
                Type            TEXT NOT NULL DEFAULT '',
                Status          TEXT NOT NULL DEFAULT 'Available',
                CurrentSiteId   INTEGER REFERENCES Projects(ProjectId),
                LastMaintenance TEXT,
                NextMaintenance TEXT
            );

            CREATE TABLE IF NOT EXISTS Materials (
                MaterialId   INTEGER PRIMARY KEY AUTOINCREMENT,
                Name         TEXT NOT NULL DEFAULT '',
                Unit         TEXT NOT NULL DEFAULT '',
                Stock        REAL NOT NULL DEFAULT 0,
                ReorderPoint REAL NOT NULL DEFAULT 0,
                SupplierId   INTEGER REFERENCES Contacts(ContactId)
            );

            CREATE TABLE IF NOT EXISTS StockMovements (
                MovementId   INTEGER PRIMARY KEY AUTOINCREMENT,
                MaterialId   INTEGER NOT NULL REFERENCES Materials(MaterialId),
                MovementType TEXT    NOT NULL DEFAULT '',
                Quantity     REAL    NOT NULL DEFAULT 0,
                Date         TEXT    NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Contacts (
                ContactId     INTEGER PRIMARY KEY AUTOINCREMENT,
                Type          TEXT NOT NULL DEFAULT '',
                Name          TEXT NOT NULL DEFAULT '',
                ContactPerson TEXT,
                Phone         TEXT,
                Email         TEXT,
                Address       TEXT,
                Notes         TEXT
            );

            CREATE INDEX IF NOT EXISTS idx_users_username
                ON Users(Username);

            CREATE INDEX IF NOT EXISTS idx_payments_date
                ON Payments(PaymentDate);

            CREATE INDEX IF NOT EXISTS idx_tasks_end
                ON Tasks(Status, EndDate);

            CREATE INDEX IF NOT EXISTS idx_attendance
                ON Attendance(WorkerId, Date);

            CREATE INDEX IF NOT EXISTS idx_movements
                ON StockMovements(MaterialId, Date);
        ";

        cmd.ExecuteNonQuery();
        tx.Commit();
    }

    /// <summary>
    /// Applies schema changes that cannot use CREATE TABLE IF NOT EXISTS.
    /// Each statement is wrapped in try/catch so it is safe to run on
    /// an existing database that already has the column.
    /// </summary>
    private static void MigrateSchema(SqliteConnection conn)
    {
        // Replace INTEGER CurrentSiteId with TEXT CurrentSite (Module 5).
        // Silently ignored if the column already exists.
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE Equipment ADD COLUMN CurrentSite TEXT;";
            cmd.ExecuteNonQuery();
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            // Column already exists — nothing to do.
        }

        // Add MustChangePassword flag to Users (Module 6 polish).
        // New users and password resets set this to 1; cleared after a change.
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "ALTER TABLE Users ADD COLUMN MustChangePassword INTEGER NOT NULL DEFAULT 0;";
            cmd.ExecuteNonQuery();
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            // Column already exists — nothing to do.
        }
    }

    /// <summary>
    /// Inserts a default admin user if the Users table is empty.
    /// The password is BCrypt-hashed with work factor 12.
    /// </summary>
    private static void SeedDefaultAdmin(SqliteConnection conn)
    {
        // Check whether any users exist.
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM Users;";
        long count = (long)(checkCmd.ExecuteScalar() ?? 0L);

        if (count > 0)
            return; // Database already has users — nothing to seed.

        string hash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123", workFactor: 12);

        using var tx      = conn.BeginTransaction();
        using var seedCmd = conn.CreateCommand();
        seedCmd.Transaction = tx;
        seedCmd.CommandText = @"
            INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive)
            VALUES ($u, $h, $f, $r, 1);
        ";
        seedCmd.Parameters.AddWithValue("$u", "admin");
        seedCmd.Parameters.AddWithValue("$h", hash);
        seedCmd.Parameters.AddWithValue("$f", "Default Administrator");
        seedCmd.Parameters.AddWithValue("$r", "Manager");
        seedCmd.ExecuteNonQuery();
        tx.Commit();
    }

    /// <summary>
    /// Inserts a starter project if the Projects table is empty, so a fresh
    /// database always has one project ready for payments, tasks, and reports.
    /// On existing databases (which already hold a project) this does nothing.
    /// </summary>
    private static void SeedDefaultProject(SqliteConnection conn)
    {
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM Projects;";
        long count = (long)(checkCmd.ExecuteScalar() ?? 0L);

        if (count > 0)
            return; // Database already has a project — nothing to seed.

        var today = DateTime.Today;

        using var tx      = conn.BeginTransaction();
        using var seedCmd = conn.CreateCommand();
        seedCmd.Transaction = tx;
        seedCmd.CommandText = @"
            INSERT INTO Projects (Name, Budget, Spent, StartDate, EndDate)
            VALUES ($n, $b, 0, $s, $e);
        ";
        seedCmd.Parameters.AddWithValue("$n", "My Construction Project");
        seedCmd.Parameters.AddWithValue("$b", 5_000_000.0);
        seedCmd.Parameters.AddWithValue("$s", today.ToString("yyyy-MM-dd"));
        seedCmd.Parameters.AddWithValue("$e", today.AddDays(365).ToString("yyyy-MM-dd"));
        seedCmd.ExecuteNonQuery();
        tx.Commit();
    }
}
