using ConstructionMS.Services;

namespace ConstructionMS.Data.Repositories;

/// <summary>
/// Handles read-only aggregate queries used exclusively by the Reports module.
/// No inserts or updates — this repository is strictly for reporting.
/// </summary>
public class ReportRepository
{
    private readonly DbConnectionFactory _factory;

    /// <summary>Initialises the repository with a connection factory.</summary>
    public ReportRepository(DbConnectionFactory factory) => _factory = factory;

    /// <summary>
    /// Returns total payment count and amount grouped by payee type
    /// for the given project.
    /// </summary>
    /// <param name="projectId">The project to summarise.</param>
    /// <returns>
    /// One <see cref="PaymentSummaryLine"/> per distinct payee type,
    /// ordered alphabetically.
    /// </returns>
    public List<PaymentSummaryLine> GetPaymentSummaryByType(int projectId)
    {
        using var conn = _factory.Open();
        using var cmd  = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT PayeeType,
                   COUNT(*)      AS Cnt,
                   SUM(Amount)   AS Total
            FROM   Payments
            WHERE  ProjectId = $pid
            GROUP  BY PayeeType
            ORDER  BY PayeeType;
        ";
        cmd.Parameters.AddWithValue("$pid", projectId);

        var list = new List<PaymentSummaryLine>();
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            list.Add(new PaymentSummaryLine
            {
                PayeeType = rd.GetString(0),
                Count     = rd.GetInt32(1),
                Total     = (decimal)rd.GetDouble(2)
            });
        }
        return list;
    }
}
