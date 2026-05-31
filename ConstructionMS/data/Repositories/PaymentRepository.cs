using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Payment"/> entity.
/// Insert uses a supplied connection/transaction so it can participate
/// in the same atomic operation as the budget update in Projects.
/// </summary>
public class PaymentRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>
    /// Initialises the repository with a connection factory.
    /// </summary>
    /// <param name="factory">The factory used to open SQLite connections.</param>
    public PaymentRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Inserts a new payment row within the supplied transaction.
    /// <c>PaymentDate</c> is stored as an ISO-8601 string (yyyy-MM-dd).
    /// <c>Note</c> is stored as NULL when the property is null or empty.
    /// </summary>
    /// <param name="conn">An already-open SQLite connection.</param>
    /// <param name="tx">The active transaction.</param>
    /// <param name="p">The payment to insert.</param>
    public void Insert(SqliteConnection conn, SqliteTransaction tx, Payment p)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = @"
            INSERT INTO Payments (ProjectId, PayeeType, Reference, Amount, PaymentDate, Note)
            VALUES ($pid, $pt, $ref, $amt, $dt, $note);
        ";
        cmd.Parameters.AddWithValue("$pid",  p.ProjectId);
        cmd.Parameters.AddWithValue("$pt",   p.PayeeType);
        cmd.Parameters.AddWithValue("$ref",  p.Reference);
        cmd.Parameters.AddWithValue("$amt",  (double)p.Amount);
        cmd.Parameters.AddWithValue("$dt",   p.PaymentDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$note",
            string.IsNullOrWhiteSpace(p.Note) ? DBNull.Value : (object)p.Note);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns the total number of payment rows across all projects.
    /// Used by the sample-data seeder to stay idempotent.
    /// </summary>
    public int CountAll()
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Payments;";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>
    /// Returns all payments for a project within the given date range,
    /// ordered newest first.
    /// </summary>
    /// <param name="projectId">The project to filter by.</param>
    /// <param name="from">The start of the date range (inclusive).</param>
    /// <param name="to">The end of the date range (inclusive).</param>
    /// <returns>A list of matching <see cref="Payment"/> objects.</returns>
    public List<Payment> GetForProject(int projectId, DateTime from, DateTime to)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT PaymentId, ProjectId, PayeeType, Reference,
                   Amount, PaymentDate, Note
            FROM   Payments
            WHERE  ProjectId   = $pid
              AND  PaymentDate BETWEEN $from AND $to
            ORDER  BY PaymentDate DESC;
        ";
        cmd.Parameters.AddWithValue("$pid",  projectId);
        cmd.Parameters.AddWithValue("$from", from.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$to",   to.ToString("yyyy-MM-dd"));

        var list = new List<Payment>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            list.Add(new Payment
            {
                PaymentId   = rd.GetInt32(0),
                ProjectId   = rd.GetInt32(1),
                PayeeType   = rd.GetString(2),
                Reference   = rd.GetString(3),
                Amount      = (decimal)rd.GetDouble(4),
                PaymentDate = DateTime.Parse(rd.GetString(5)),
                Note        = rd.IsDBNull(6) ? null : rd.GetString(6)
            });
        }
        return list;
    }
}
