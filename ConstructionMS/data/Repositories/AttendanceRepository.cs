using ConstructionMS.Models;
using Microsoft.Data.Sqlite;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles all database operations for the <see cref="Attendance"/> entity.
/// </summary>
public class AttendanceRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public AttendanceRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Inserts a new attendance record. Date is stored as yyyy-MM-dd.
    /// </summary>
    public void Insert(Attendance a)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Attendance (WorkerId, Date, Hours)
            VALUES ($w, $d, $h);
        ";
        cmd.Parameters.AddWithValue("$w", a.WorkerId);
        cmd.Parameters.AddWithValue("$d", a.Date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$h", (double)a.Hours);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns true if an attendance record already exists for the
    /// given worker on the given date — used to prevent duplicate entries.
    /// </summary>
    public bool ExistsForWorkerOnDate(int workerId, DateTime date)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT COUNT(*) FROM Attendance
            WHERE  WorkerId = $w AND Date = $d;
        ";
        cmd.Parameters.AddWithValue("$w", workerId);
        cmd.Parameters.AddWithValue("$d", date.ToString("yyyy-MM-dd"));
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    /// <summary>
    /// Returns attendance records for a worker within the given date range,
    /// ordered by date ascending.
    /// </summary>
    public List<Attendance> GetForWorker(int workerId, DateTime from, DateTime to)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT AttendanceId, WorkerId, Date, Hours
            FROM   Attendance
            WHERE  WorkerId = $w
              AND  Date BETWEEN $from AND $to
            ORDER  BY Date ASC;
        ";
        cmd.Parameters.AddWithValue("$w",    workerId);
        cmd.Parameters.AddWithValue("$from", from.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$to",   to.ToString("yyyy-MM-dd"));

        var list = new List<Attendance>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
            list.Add(new Attendance
            {
                AttendanceId = rd.GetInt32(0),
                WorkerId     = rd.GetInt32(1),
                Date         = DateTime.Parse(rd.GetString(2)),
                Hours        = (decimal)rd.GetDouble(3)
            });
        return list;
    }
}
