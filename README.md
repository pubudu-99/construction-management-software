# Construction Management Software

Offline Windows desktop application for small construction firms to manage
payments, labour, equipment, materials, scheduling, and contacts.

Built as the final-year ICT Project (ITE 1943) for the BIT External Degree
programme, University of Moratuwa.

---

## Overview

A single-machine, offline-first management system for a small construction
company. All data is stored locally in a SQLite database — there is no
internet dependency. Two role types (Manager and Supervisor) provide
tiered access. The application is designed to be installed on a single
office PC and used by daily operations staff.

## Features

| # | Module | Description |
|---|--------|-------------|
| FR1 | **User Authentication & Role-Based Access** | Login, BCrypt-hashed passwords, forced password change at first login, user management (Managers only), administrator password reset |
| FR2 | **Payment Management** | Record payments against a project budget within a single atomic transaction; live "remaining budget" display |
| FR3 | **Task Scheduling** | Create tasks with start/end dates and assignees; colour-coded grid; dashboard deadline alerts split into Overdue and Due Soon |
| FR4 | **Labour & Payroll** | Worker registration, daily attendance, payroll report with overtime (configurable 8 h / 1.5× rule); CSV export |
| FR5 | **Equipment Management** | Catalogue with status (Available / In Use / Under Maintenance), site assignment, maintenance schedule with alerts |
| FR6 | **Materials & Stock** | Catalogue with reorder points; stock In/Out movements written in a single transaction; four-tier stock status (Out / Low / Getting Low / OK) |
| FR7 | **Client & Supplier Contacts** | Add/edit contacts with live search; type filtering (Client / Supplier) |
| — | **Reports** | Financial Summary, Payroll Summary, Stock Status — all exportable to CSV |
| — | **Database Backup** | One-click backup of the SQLite file from the File menu |

## Tech Stack

- **C# on .NET 8** (LTS)
- **Windows Forms** (not WPF/MAUI)
- **SQLite** via `Microsoft.Data.Sqlite`
- **BCrypt.Net-Next** for password hashing (work factor 12)
- **System.Configuration.ConfigurationManager** for `App.config`

## Architecture

Three-layer architecture, strictly enforced. Calls only flow downward.

```
Forms/                  <- Presentation (Windows Forms)
   |
   v
Services/               <- Business rules and validation
   |
   v
Data/Repositories/      <- The only classes allowed to talk to SQLite

Models/                 <- Plain entity classes
```

All SQL uses parameterised queries (`$paramName`); every multi-step write
is wrapped in a SQLite transaction so that partial failures cannot
corrupt budget, stock, or attendance figures.

## Getting Started

### Prerequisites

- Windows 10 or 11 (x64)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Clone and run

```
git clone https://github.com/pubudu-99/construction-management-software.git
cd construction-management-software
dotnet build
dotnet run --project ConstructionMS
```

The application creates `bin/Debug/net8.0-windows/db/construction.db` on
first run and seeds it with:

- A default Manager account — **admin / ChangeMe123** (forced password
  change at first login)
- A starter project — **My Construction Project** (LKR 5,000,000 budget,
  365-day timeline)

### Self-contained publish

To build a redistributable folder with the .NET runtime included (runs
on any 64-bit Windows machine without requiring .NET to be installed):

```
dotnet publish ConstructionMS/ConstructionMS.csproj ^
    -c Release -r win-x64 --self-contained true ^
    -p:PublishSingleFile=false -o publish/
```

## Configuration

Tunable values in `ConstructionMS/App.config` — read at runtime, no
rebuild required to change them:

| Key | Default | Purpose |
|-----|---------|---------|
| `StandardDailyHours` | 8 | Hours per working day before overtime applies |
| `OvertimeMultiplier` | 1.5 | Overtime rate multiplier (e.g. time-and-a-half) |
| `MaintenanceWarningDays` | 7 | Equipment-maintenance amber-alert window |
| `LowStockWarningBuffer` | 1.5 | Materials "Getting Low" buffer (× reorder point) |

## Project Structure

```
ConstructionMS/
├── ConstructionMS.sln          <- Classic solution file
├── ConstructionMS.slnx         <- XML solution file (SDK 10+)
├── README.md
├── ConstructionMS/
│   ├── App.config
│   ├── Program.cs
│   ├── Models/                 <- 10 entity classes (User, Payment, ...)
│   ├── data/
│   │   ├── DbConnectionFactory.cs
│   │   ├── DatabaseInitializer.cs
│   │   └── Repositories/       <- 11 repositories (one per aggregate)
│   ├── Services/               <- Business logic, DTOs, result types
│   └── Forms/                  <- Windows Forms (+ designers + GridStyle)
└── docs/
    └── screenshots/            <- Figures for the project report
```

## Roles & Permissions

| Action | Manager | Supervisor |
|---|:---:|:---:|
| View dashboard alerts | yes | yes |
| Edit project budget | yes | — |
| Record payments | yes | view-only (Save disabled) |
| Create tasks | yes | view-only (Add disabled) |
| Record attendance | yes | yes |
| Run payroll | yes | yes |
| Manage equipment / materials / contacts | yes | yes |
| Reports | yes | yes |
| User management | yes | hidden |
| Reset another user's password | yes | — |

## Screenshots

`docs/screenshots/` contains the figures referenced in the project
report (`fig-5-1` through `fig-5-17`), covering the database schema,
the dashboard, every module, the supervisor-restricted views, the
reports, and the backup confirmation.

## Author

**Pubudu Perera**
Student ID: E2421061
BIT External Degree — University of Moratuwa
Module: ITE 1943 (ICT Project), 2026
