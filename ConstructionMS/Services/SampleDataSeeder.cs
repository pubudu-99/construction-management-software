using ConstructionMS.Data;
using ConstructionMS.Data.Repositories;
using ConstructionMS.Models;

namespace ConstructionMS.Services;

/// <summary>
/// Populates the database with realistic demo data for the presentation.
/// Idempotent: each section checks the current row count and only inserts
/// when the table is below its expected size, so it is safe to run repeatedly.
/// </summary>
public class SampleDataSeeder
{
    private readonly DbConnectionFactory  _factory;
    private readonly WorkerRepository     _workers;
    private readonly AttendanceRepository _attendance;
    private readonly MaterialRepository   _materials;
    private readonly EquipmentRepository  _equipment;
    private readonly ContactRepository    _contacts;
    private readonly PaymentRepository    _payments;
    private readonly TaskRepository       _tasks;
    private readonly ProjectRepository    _projects;

    /// <summary>Initialises the seeder with the factory and every repository it needs.</summary>
    public SampleDataSeeder(DbConnectionFactory factory,
                            WorkerRepository workers,
                            AttendanceRepository attendance,
                            MaterialRepository materials,
                            EquipmentRepository equipment,
                            ContactRepository contacts,
                            PaymentRepository payments,
                            TaskRepository tasks,
                            ProjectRepository projects)
    {
        _factory    = factory;
        _workers    = workers;
        _attendance = attendance;
        _materials  = materials;
        _equipment  = equipment;
        _contacts   = contacts;
        _payments   = payments;
        _tasks      = tasks;
        _projects   = projects;
    }

    /// <summary>
    /// Inserts demo workers, attendance, materials, equipment, contacts,
    /// payments, and tasks — but only the sections that are not already present.
    /// </summary>
    /// <returns>A human-readable summary of what was loaded (or that nothing changed).</returns>
    public string LoadSampleData()
    {
        var project = _projects.GetFirst();
        if (project is null)
            return "No project found — cannot load sample data.";

        bool added = false;

        // ── Workers ──
        if (_workers.GetActive().Count < 3)
        {
            _workers.Insert(new Worker { Name = "Kamal Silva",    HourlyRate = 350m, IsActive = true });
            _workers.Insert(new Worker { Name = "Nimal Perera",   HourlyRate = 400m, IsActive = true });
            _workers.Insert(new Worker { Name = "Sunil Fernando", HourlyRate = 300m, IsActive = true });
            added = true;
        }

        // ── Attendance (last 5 working days, varied hours for overtime testing) ──
        if (_attendance.CountAll() == 0)
        {
            var workdays = LastWorkingDays(5);
            foreach (var worker in _workers.GetActive())
            {
                for (int i = 0; i < workdays.Count; i++)
                {
                    // Alternate 8h and 10h so some days generate overtime.
                    decimal hours = (i % 2 == 0) ? 8m : 10m;
                    _attendance.Insert(new Attendance
                    {
                        WorkerId = worker.WorkerId,
                        Date     = workdays[i],
                        Hours    = hours
                    });
                }
            }
            added = true;
        }

        // ── Materials ──
        if (_materials.GetAll().Count < 4)
        {
            _materials.Insert(new Material { Name = "Cement",     Unit = "bags",     Stock = 100m, ReorderPoint = 50m });
            _materials.Insert(new Material { Name = "Sand",       Unit = "cubic_m",  Stock = 20m,  ReorderPoint = 10m });
            _materials.Insert(new Material { Name = "Steel Bars", Unit = "tons",     Stock = 5m,   ReorderPoint = 2m });
            _materials.Insert(new Material { Name = "Bricks",     Unit = "thousand", Stock = 15m,  ReorderPoint = 5m });
            added = true;
        }

        // ── Equipment ──
        if (_equipment.GetAll().Count < 3)
        {
            var today = DateTime.Today;
            _equipment.Insert(new Equipment
            {
                Name = "JCB Excavator", Type = "Heavy Machinery",
                Status = "Available", NextMaintenance = today.AddDays(30)
            });
            _equipment.Insert(new Equipment
            {
                Name = "Concrete Mixer 01", Type = "Equipment",
                Status = "In Use", CurrentSite = "Site A",
                NextMaintenance = today.AddDays(5)   // triggers amber maintenance alert
            });
            _equipment.Insert(new Equipment
            {
                Name = "Pickup Truck", Type = "Vehicle",
                Status = "Available", NextMaintenance = today.AddDays(60)
            });
            added = true;
        }

        // ── Contacts ──
        if (_contacts.GetAll().Count < 4)
        {
            _contacts.Insert(new Contact { Type = "Supplier", Name = "Lanka Cement Ltd" });
            _contacts.Insert(new Contact { Type = "Supplier", Name = "Sri Steel Suppliers" });
            _contacts.Insert(new Contact { Type = "Client",   Name = "Hemas Holdings" });
            _contacts.Insert(new Contact { Type = "Client",   Name = "Cargills Foods Lanka" });
            added = true;
        }

        // ── Payments (atomic: insert rows AND increment the project's Spent) ──
        if (_payments.CountAll() < 5)
        {
            var today = DateTime.Today;
            var demo = new[]
            {
                new Payment { ProjectId = project.ProjectId, PayeeType = "Supplier",   Reference = "INV-1001", Amount = 75_000m, PaymentDate = today.AddDays(-2) },
                new Payment { ProjectId = project.ProjectId, PayeeType = "Supplier",   Reference = "INV-1002", Amount = 42_500m, PaymentDate = today.AddDays(-8) },
                new Payment { ProjectId = project.ProjectId, PayeeType = "Contractor", Reference = "CTR-2001", Amount = 95_000m, PaymentDate = today.AddDays(-15) },
                new Payment { ProjectId = project.ProjectId, PayeeType = "Worker",     Reference = "WGE-3001", Amount = 18_750m, PaymentDate = today.AddDays(-22) },
                new Payment { ProjectId = project.ProjectId, PayeeType = "Contractor", Reference = "CTR-2002", Amount = 60_000m, PaymentDate = today.AddDays(-28) },
            };

            decimal total = demo.Sum(p => p.Amount);

            using var conn = _factory.Open();
            using var tx   = conn.BeginTransaction();
            foreach (var p in demo)
                _payments.Insert(conn, tx, p);
            _projects.IncrementSpent(conn, tx, project.ProjectId, total);
            tx.Commit();

            added = true;
        }

        // ── Tasks (Status must be "Open" to show on the dashboard deadline card) ──
        if (_tasks.GetByProject(project.ProjectId).Count < 3)
        {
            var today = DateTime.Today;
            _tasks.Insert(new ProjectTask
            {
                ProjectId = project.ProjectId, Name = "Site Preparation",
                StartDate = today.AddDays(-7), EndDate = today.AddDays(-2), Status = "Open"   // overdue (red)
            });
            _tasks.Insert(new ProjectTask
            {
                ProjectId = project.ProjectId, Name = "Foundation Pouring",
                StartDate = today.AddDays(-3), EndDate = today.AddDays(2), Status = "Open"    // due soon (amber)
            });
            _tasks.Insert(new ProjectTask
            {
                ProjectId = project.ProjectId, Name = "Steel Erection",
                StartDate = today.AddDays(5), EndDate = today.AddDays(20), Status = "Open"    // normal
            });
            added = true;
        }

        return added
            ? "Sample data loaded: 3 workers, 15 attendance records, 4 materials, " +
              "3 equipment items, 4 contacts, 5 payments, 3 tasks."
            : "Sample data already present — no changes made.";
    }

    /// <summary>
    /// Returns the most recent <paramref name="count"/> working days (Mon–Fri),
    /// counting back from today, in chronological (oldest-first) order.
    /// </summary>
    private static List<DateTime> LastWorkingDays(int count)
    {
        var days = new List<DateTime>();
        var d = DateTime.Today;
        while (days.Count < count)
        {
            if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                days.Add(d);
            d = d.AddDays(-1);
        }
        days.Reverse();
        return days;
    }
}
