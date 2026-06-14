using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for project setup — name, budget, and timeline.
/// Validation lives here, never in the form. <c>Spent</c> is never modified
/// by this service; it is controlled solely by the payment transaction.
/// </summary>
public class ProjectService
{
    private readonly ProjectRepository _repo;

    /// <summary>Initialises the service with a project repository.</summary>
    public ProjectService(ProjectRepository repo) => _repo = repo;

    /// <summary>
    /// Validates and saves changes to an existing project.
    /// Rules: name required; budget greater than zero; budget not below the
    /// amount already spent; end date after start date.
    /// </summary>
    /// <param name="p">The project carrying the edited values.</param>
    /// <returns><see cref="ProjectResult.Ok"/> on success, otherwise a failure with reason.</returns>
    public ProjectResult Update(Project p)
    {
        if (string.IsNullOrWhiteSpace(p.Name))
            return ProjectResult.Fail("Project name is required.");

        if (p.Budget <= 0)
            return ProjectResult.Fail("Budget must be greater than zero.");

        if (p.Budget < p.Spent)
            return ProjectResult.Fail(
                $"Budget (LKR {p.Budget:N2}) cannot be less than the amount " +
                $"already spent (LKR {p.Spent:N2}).");

        if (p.EndDate.Date <= p.StartDate.Date)
            return ProjectResult.Fail("End date must be after the start date.");

        _repo.Update(p);
        return ProjectResult.Ok();
    }
}
