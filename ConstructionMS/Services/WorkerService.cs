using System.Text.RegularExpressions;
using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for worker (labour) management.
/// Validates input and delegates persistence to <see cref="WorkerRepository"/>.
/// </summary>
public class WorkerService
{
    private readonly DbConnectionFactory _factory;
    private readonly WorkerRepository    _repo;

    /// <summary>
    /// Sri Lankan NIC format: 9 digits followed by V/X, or 12 digits.
    /// </summary>
    private static readonly Regex NicPattern =
        new("^([0-9]{9}[VvXx]|[0-9]{12})$", RegexOptions.Compiled);

    /// <summary>Initialises the service with a connection factory and worker repository.</summary>
    /// <param name="factory">Connection factory used for audit logging.</param>
    /// <param name="repo">The repository used to read and write workers.</param>
    public WorkerService(DbConnectionFactory factory, WorkerRepository repo)
    {
        _factory = factory;
        _repo    = repo;
    }

    /// <summary>Returns every worker (active and inactive), ordered by name.</summary>
    public List<Worker> GetAll() => _repo.GetAll();

    /// <summary>Returns all active workers, ordered by name.</summary>
    public List<Worker> GetActive() => _repo.GetActive();

    /// <summary>
    /// Validates and inserts a new worker. Returns the new WorkerId on success.
    /// </summary>
    /// <param name="w">The worker to create.</param>
    /// <param name="newId">The auto-generated WorkerId when successful; otherwise 0.</param>
    public WorkerResult Create(Worker w, out int newId)
    {
        newId = 0;
        var check = Validate(w, w.WorkerId);
        if (!check.Success) return check;

        newId = _repo.Insert(w);
        ActivityLogger.Log(_factory, "Worker Added", w.Name);
        return WorkerResult.Ok();
    }

    /// <summary>Validates and updates an existing worker.</summary>
    /// <param name="w">The worker with new values; <c>WorkerId</c> selects the row.</param>
    public WorkerResult Update(Worker w)
    {
        var check = Validate(w, w.WorkerId);
        if (!check.Success) return check;

        _repo.Update(w);
        ActivityLogger.Log(_factory, "Worker Updated", w.Name);
        return WorkerResult.Ok();
    }

    /// <summary>Toggles a worker's active flag (deactivate or reactivate).</summary>
    /// <param name="workerId">The worker whose active flag will be set.</param>
    /// <param name="active">The new value for IsActive.</param>
    public WorkerResult SetActive(int workerId, bool active)
    {
        var w = _repo.GetById(workerId);
        if (w is null) return WorkerResult.Fail("Worker not found.");

        _repo.SetActive(workerId, active);
        ActivityLogger.Log(_factory,
            active ? "Worker Activated" : "Worker Deactivated", w.Name);
        return WorkerResult.Ok();
    }

    /// <summary>
    /// Shared validation for create and update. <paramref name="excludeWorkerId"/>
    /// is excluded from the duplicate-NIC check (the row being edited).
    /// </summary>
    private WorkerResult Validate(Worker w, int excludeWorkerId)
    {
        if (string.IsNullOrWhiteSpace(w.Name))
            return WorkerResult.Fail("Worker name is required.");

        if (w.HourlyRate <= 0)
            return WorkerResult.Fail("Hourly rate must be greater than zero.");

        var nic = w.NIC?.Trim();
        if (!string.IsNullOrEmpty(nic))
        {
            if (!NicPattern.IsMatch(nic))
                return WorkerResult.Fail("Invalid NIC format.");

            if (_repo.NicExists(nic, excludeWorkerId))
                return WorkerResult.Fail("NIC already exists.");
        }

        return WorkerResult.Ok();
    }
}
