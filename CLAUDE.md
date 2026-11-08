# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Project

**KODOTI.Commerce** is a sample e-commerce backend built as microservices on **ASP.NET Core 3.1** (`netcoreapp3.1`). Right now it has one service, **Catalog**, which handles products and stock. It uses **CQRS** with **MediatR 9**, **EF Core 5** on SQL Server, and syslog logging to Papertrail.

Solution file: `KODOTI.Commerce.sln`. The user-facing overview is in `README.md`.

## Commands

Run all commands from the repo root.

```bash
dotnet build KODOTI.Commerce.sln
```
```bash
dotnet run --project src/Services/Catalog/Catalog.Api
```
The API listens on http://localhost:5000. `GET /` returns `Running....`.

EF Core migrations need `dotnet-ef` 5.0.x. The DbContext lives in the Persistence project, and the startup project is the Api:
```bash
dotnet ef migrations add <Name> --project src/Services/Catalog/Catalog.Persistence.Database --startup-project src/Services/Catalog/Catalog.Api
```
```bash
dotnet ef database update --project src/Services/Catalog/Catalog.Persistence.Database --startup-project src/Services/Catalog/Catalog.Api
```

There's no test project yet.

## Architecture

```
src/Common.Logging/                       Syslog ILoggerProvider -> Papertrail (AddSyslog extension)
src/Services/Catalog/
  Catalog.Api/                            Controllers + Startup (DI, MediatR, DbContext registration)
  Catalog.Common/                         Enums (ProducInStockAction: Add, Substract)
  Catalog.Domain/                         Entities: Product, ProductInStock
  Catalog.Persistence.Database/           ApplicationDbContext, entity configs, migrations, HasData seed
  Catalog.Service.Queries/                READ side: I*QueryService + implementations + DTOs
  Catalog.Services.EventHandlers/         WRITE side: Commands (INotification) + INotificationHandler
src/Services/Service.Common.Paging/       IQueryable<T>.GetPagedAsync(page, take) -> DataCollection<T>
src/Services/Services.Common.Collection/  DataCollection<T> { Items, Total, Page, Pages, HasItem }
src/Services/Services.Common.Mapping/     object.MapTo<T>() (System.Text.Json round-trip)
```

**How requests flow:**
- **Reads (GET):** the controller calls `I*QueryService`. The service runs an EF query, then `.GetPagedAsync()` and `.MapTo<DataCollection<Dto>>()`.
- **Writes (POST/PUT):** the controller calls `_mediator.Publish(command)`. The matching `INotificationHandler<TCommand>` changes the data and calls `SaveChangesAsync`.
- Controllers don't access the DbContext directly. Business rules belong in the handlers.

**Endpoints:** `GET /products`, `GET /products/{id}`, `POST /products`, `GET /stocks`, `PUT /stocks` (bulk add or subtract stock).

## Conventions

### Adding a feature
- **New query:** add a DTO in `Catalog.Service.Queries/DTOs`, plus an interface and implementation in `Catalog.Service.Queries`. Register it in `Startup.ConfigureServices` with `AddTransient<IX, X>()`.
- **New command:** add a class in `Catalog.Services.EventHandlers/Commands` that implements `INotification`, and a handler next to it. `AddMediatR(Assembly.Load("Catalog.Services.EventHandlers"))` registers handlers automatically.
- **New entity:** add the class to `Catalog.Domain`, a `*Configuration` class in `Persistence.Database/Configuration` (constructor takes `EntityTypeBuilder<T>`), wire it up in `ApplicationDbContext.ModelConfig`, add a `DbSet`, then add a migration.

### Style
- The database schema is `Catalog`, and the migrations history table is `Catalog.__EFMigrationsHistory`.
- Inject `ILogger<T>` and log handler steps with the `--- message` prefix.
- Use attribute routing on controllers that derive from `ControllerBase` and have `[ApiController]`.

### Misspelled names
Several names have typos. Keep them as they are unless the task is to rename them, and if you do rename one, update every reference:
- `ProducInStockAction`, `Substract` in `Catalog.Common/Enums.cs`
- `ProductInStockUpdateCommnand`
- `GetAllAsyc`, `GetAsyc` in `IProductQueryService` (`IProductInStockQueryService` uses the correct `GetAllAsync`)

### Known issues
These are tracked in the README roadmap. Don't fix them unless asked:
- `ProductQueryService.GetAsyc` uses `SingleAsync`, so a missing id becomes a 500 instead of a 404.
- `POST /products` returns `200 Ok()` with no id. It would need `IRequest<int>` + `Send` to return a 201.
- `ProductInStockUpdateStockEventHandler` throws `System.Exception` when stock runs out (500). It also dereferences `entry` when `entry` is null.
- `ids` and `products` are parsed with `Convert.ToInt32`, so bad input becomes a 500.
- No validation, no global exception middleware, no `CancellationToken`, no Swagger.

## Configuration

`src/Services/Catalog/Catalog.Api/appsettings.json` contains `ConnectionStrings:DefaultConnection` (it currently points at a machine-specific SQL Server) and `Papertrail:host`/`Papertrail:port`. Override them locally with user secrets or environment variables. Don't commit personal connection strings or credentials.

## Docs

`doc/` holds the study material:
- `doc/aspnet-controller-cheatsheet.md`: REST verbs, status codes, and controller patterns
- `doc/aspnet-mock-interview-exercises.md`: timed practice exercises

When the controller behavior described there changes, update these docs and `README.md`.

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- When graphify-out/graph.json exists, run `graphify query "<question>"` before your first grep, search or source-file read for a codebase question, and put this rule in every subagent prompt that explores code. Read raw files only for lines the graph or an earlier read already located. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).
- A committed graphify-out/graph.json is rewritten whole on every build, so a large diff to it in a pull request is expected. Do not flag it as churn or suggest skipping the graph rebuild or its hooks.
