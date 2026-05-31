using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Business rules for FR5 — Vehicle and Equipment Management.
/// </summary>
public class EquipmentService
{
    private readonly DbConnectionFactory _factory;
    private readonly EquipmentRepository _repo;

    /// <summary>Initialises the service with a connection factory and equipment repository.</summary>
    /// <param name="factory">Connection factory used for audit logging.</param>
    /// <param name="repo">The repository used to read and write equipment.</param>
    public EquipmentService(DbConnectionFactory factory, EquipmentRepository repo)
    {
        _factory = factory;
        _repo    = repo;
    }

    /// <summary>
    /// Validates and inserts a new equipment record.
    /// </summary>
    /// <returns><see cref="EquipmentResult.Ok"/> or a <see cref="EquipmentResult.Fail"/> with a reason.</returns>
    public EquipmentResult AddEquipment(Equipment e)
    {
        if (string.IsNullOrWhiteSpace(e.Name))
            return EquipmentResult.Fail("Equipment name is required.");
        if (string.IsNullOrWhiteSpace(e.Type))
            return EquipmentResult.Fail("Equipment type is required.");

        _repo.Insert(e);
        return EquipmentResult.Ok();
    }

    /// <summary>
    /// Updates the status and current site of an equipment item.
    /// Clears <c>CurrentSite</c> when the new status is not "In Use".
    /// </summary>
    public EquipmentResult UpdateStatus(int equipmentId, string newStatus, string? siteName)
    {
        var item = _repo.GetById(equipmentId);
        if (item is null) return EquipmentResult.Fail("Equipment not found.");

        item.Status      = newStatus;
        item.CurrentSite = newStatus == "In Use" ? siteName : null;
        _repo.Update(item);
        ActivityLogger.Log(_factory, "Equipment Status Changed", $"{item.Name} -> {newStatus}");
        return EquipmentResult.Ok();
    }

    /// <summary>
    /// Returns maintenance alerts for equipment due within <paramref name="warningDays"/> days.
    /// When <paramref name="warningDays"/> is not supplied, the value is read from
    /// App.config key <c>MaintenanceWarningDays</c> (default 7).
    /// </summary>
    public List<MaintenanceAlert> GetMaintenanceAlerts(int warningDays = -1)
    {
        if (warningDays < 0)
            warningDays = AppConfig.MaintenanceWarningDays;

        var today = DateTime.Today;
        return _repo.GetMaintenanceDueAlerts(warningDays)
            .Select(e => new MaintenanceAlert
            {
                EquipmentName   = e.Name,
                NextMaintenance = e.NextMaintenance!.Value,
                DaysUntilDue    = (e.NextMaintenance.Value.Date - today).Days
            })
            .ToList();
    }
}
