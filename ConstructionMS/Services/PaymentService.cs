using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Enforces business rules for FR2 — Payment Management.
/// <para>
/// <see cref="Record"/> validates the budget constraint, then writes
/// the payment row and increments <c>Projects.Spent</c> in a single
/// ACID transaction — either both writes succeed or neither does.
/// </para>
/// </summary>
public class PaymentService
{
    private readonly DbConnectionFactory _factory;
    private readonly PaymentRepository   _payments;
    private readonly ProjectRepository   _projects;

    /// <summary>
    /// Initialises the service with its required dependencies.
    /// </summary>
    /// <param name="factory">Connection factory shared with the repositories.</param>
    /// <param name="payments">Repository for payment rows.</param>
    /// <param name="projects">Repository for project rows.</param>
    public PaymentService(DbConnectionFactory factory,
                          PaymentRepository   payments,
                          ProjectRepository   projects)
    {
        _factory  = factory;
        _payments = payments;
        _projects = projects;
    }

    /// <summary>
    /// Records a payment against its project, enforcing the budget constraint.
    /// <para>
    /// Steps: validate inputs → load project → check remaining budget →
    /// open connection → begin transaction → insert payment →
    /// increment Spent → commit. On any failure the transaction is
    /// rolled back and a <see cref="PaymentResult.Fail"/> is returned.
    /// </para>
    /// </summary>
    /// <param name="p">The payment to record.</param>
    /// <returns>
    /// <see cref="PaymentResult.Ok"/> on success, or
    /// <see cref="PaymentResult.Fail"/> with a reason message.
    /// </returns>
    public PaymentResult Record(Payment p)
    {
        // 1. Load the project (standalone connection — read only).
        var project = _projects.GetById(p.ProjectId);
        if (project is null)
            return PaymentResult.Fail("Project not found.");

        // 2. Budget validation.
        decimal remaining = project.Budget - project.Spent;
        if (p.Amount > remaining)
            return PaymentResult.Fail(
                $"Amount exceeds remaining budget of LKR {remaining:N2}.");

        // 3. Transactional write — both operations or neither.
        using var conn = _factory.Open();
        using var tx   = conn.BeginTransaction();
        try
        {
            _payments.Insert(conn, tx, p);
            _projects.IncrementSpent(conn, tx, p.ProjectId, p.Amount);
            tx.Commit();
            ActivityLogger.Log(_factory, "Payment Recorded",
                $"LKR {p.Amount:N2} to {p.PayeeType} - {p.Reference}");
            return PaymentResult.Ok();
        }
        catch (Exception ex)
        {
            tx.Rollback();
            return PaymentResult.Fail("System error: " + ex.Message);
        }
    }
}
