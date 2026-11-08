# Graph Report - sample-micro-services-dot-net-core-3  (2026-11-07)

## Corpus Check
- Corpus is ~23,831 words - fits in a single context window. You may not need a graph.

## Summary
- 349 nodes · 616 edges · 19 communities (14 shown, 5 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 43 edges (avg confidence: 0.87)
- Token cost: 138,588 input · 0 output

## Community Hubs (Navigation)
- Catalog Service Namespaces
- Controller Cheat Sheet Concepts
- Catalog API Controllers
- EF Core Persistence & Product
- Syslog Logging Provider
- MediatR Commands & Enums
- Launch Settings
- EF Core Migrations
- Host Startup & Pipeline
- Api & Logging Packages
- Paging & Collection Packages
- Domain & Persistence Packages
- Common & Handlers Packages
- Queries & Mapping Packages
- ETag & Concurrency Exercises
- DTO Mapping Extension
- API Versioning
- Order State Machine Exercise

## God Nodes (most connected - your core abstractions)
1. `ApplicationDbContext` - 14 edges
2. `DataCollection` - 14 edges
3. `ProductDto` - 13 edges
4. `Product` - 12 edges
5. `SyslogLogLevel` - 11 edges
6. `SyslogLogger` - 10 edges
7. `ProductInStock` - 9 edges
8. `ProductController` - 8 edges
9. `Catalog.Persistence.Database` - 8 edges
10. `Catalog.Service.Queries.DTOs` - 8 edges

## Surprising Connections (you probably didn't know these)
- `PATCH-5 Concurrent Stock Adjustment` --semantically_similar_to--> `/stocks API (Add / Substract stock)`  [INFERRED] [semantically similar]
  doc/aspnet-mock-interview-exercises.md → README.md
- `Roadmap (404s, 201 Created, ProblemDetails, validation, more services)` --semantically_similar_to--> `Code-Review Notes on This Repo`  [INFERRED] [semantically similar]
  README.md → doc/aspnet-controller-cheatsheet.md
- `FluentValidation / MediatR IPipelineBehavior` --references--> `MediatR 9 (INotification / INotificationHandler)`  [EXTRACTED]
  doc/aspnet-controller-cheatsheet.md → README.md
- `Auth Status Codes (401 vs 403, Authorize policies)` --references--> `Catalog.Api (controllers, Startup)`  [EXTRACTED]
  doc/aspnet-controller-cheatsheet.md → README.md
- `Pagination, Filtering, Sorting` --references--> `Services.Common.Collection (DataCollection<T>)`  [EXTRACTED]
  doc/aspnet-controller-cheatsheet.md → README.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Catalog CQRS read/write request flow** — readme_catalog_api, readme_catalog_service_queries, readme_mediatr, readme_catalog_services_eventhandlers, readme_sql_server [EXTRACTED 1.00]
- **Exercises requiring atomic check-then-write on the shared Db** — doc_aspnet_mock_interview_exercises_starter_kit_db, doc_aspnet_mock_interview_exercises_post_3_duplicate_sku, doc_aspnet_mock_interview_exercises_put_5_optimistic_concurrency, doc_aspnet_mock_interview_exercises_patch_5_concurrent_stock [INFERRED 0.85]
- **409 Conflict business-state exercises** — doc_aspnet_mock_interview_exercises_post_3_duplicate_sku, doc_aspnet_mock_interview_exercises_post_4_pay_order, doc_aspnet_mock_interview_exercises_patch_3_state_machine, doc_aspnet_mock_interview_exercises_patch_5_concurrent_stock, doc_aspnet_mock_interview_exercises_delete_3_block_dependents [EXTRACTED 1.00]

## Communities (19 total, 5 thin omitted)

### Community 0 - "Catalog Service Namespaces"
Cohesion: 0.10
Nodes (13): Common.Logging, Catalog.Service.Queries, Catalog.Persistence.Database, Catalog.Api.Controllers, Catalog.Service.Queries.DTOs, Catalog.Persistence.Database.Configuration, Catalog.Services.EventHandlers.Commands, Service.Common.Paging (+5 more)

### Community 1 - "Controller Cheat Sheet Concepts"
Cohesion: 0.05
Nodes (52): Auth Status Codes (401 vs 403, Authorize policies), Model Binding Sources (FromRoute/Query/Body/Header), NotFoundException / BusinessRuleException, ASP.NET Core Controller Cheat Sheet, Code-Review Notes on This Repo, Full CRUD Controller with CQRS + MediatR, ExceptionMiddleware (global exception to status code mapping), FluentValidation / MediatR IPipelineBehavior (+44 more)

### Community 2 - "Catalog API Controllers"
Cohesion: 0.08
Nodes (20): DefaultController, ProductController, ProductInStockController, ProductDto, Description, Name, Price, ProductId (+12 more)

### Community 3 - "EF Core Persistence & Product"
Cohesion: 0.09
Nodes (20): Product, Description, Name, Price, ProductId, Stock, ProductInStock, ProductId (+12 more)

### Community 4 - "Syslog Logging Provider"
Cohesion: 0.10
Nodes (13): NoopDisposable, SyslogLogger, SyslogLoggerExtensions, SyslogLoggerProvider, SyslogLogLevel, Alert, Critical, Debug (+5 more)

### Community 5 - "MediatR Commands & Enums"
Cohesion: 0.10
Nodes (17): Catalog.Common, Enums, ProducInStockAction, Add, Substract, ProductCreateCommand, Description, Name (+9 more)

### Community 6 - "Launch Settings"
Cohesion: 0.10
Nodes (20): applicationUrl, commandName, environmentVariables, launchBrowser, launchUrl, ASPNETCORE_ENVIRONMENT, commandName, environmentVariables (+12 more)

### Community 9 - "Host Startup & Pipeline"
Cohesion: 0.20
Nodes (3): Program, Startup, Configuration

### Community 10 - "Api & Logging Packages"
Cohesion: 0.20
Nodes (8): MediatR.Extensions.Microsoft.DependencyInjection (9.0.0), Microsoft.EntityFrameworkCore.Tools (5.0.3), Microsoft.Extensions.Logging (5.0.0), Microsoft.NET.Sdk.Web, netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, MediatR (9.0.0)

### Community 11 - "Paging & Collection Packages"
Cohesion: 0.29
Nodes (5): Microsoft.EntityFrameworkCore (5.0.3), netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, Microsoft.NET.Sdk

### Community 12 - "Domain & Persistence Packages"
Cohesion: 0.29
Nodes (5): Microsoft.EntityFrameworkCore.SqlServer (5.0.3), netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, Microsoft.NET.Sdk

### Community 13 - "Common & Handlers Packages"
Cohesion: 0.29
Nodes (5): netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, MediatR (9.0.0), Microsoft.NET.Sdk

### Community 14 - "Queries & Mapping Packages"
Cohesion: 0.33
Nodes (4): netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, Microsoft.NET.Sdk

### Community 15 - "ETag & Concurrency Exercises"
Cohesion: 0.67
Nodes (4): Conditional GET with ETag (304), Optimistic Concurrency (412 / 409, If-Match), GET-5 Conditional GET with ETag (304), PUT-5 Optimistic Concurrency with If-Match (412/428)

## Knowledge Gaps
- **86 isolated node(s):** `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk`, `Emergency`, `Alert` (+81 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 136 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ApplicationDbContext` connect `EF Core Persistence & Product` to `Catalog Service Namespaces`, `MediatR Commands & Enums`?**
  _High betweenness centrality (0.039) - this node is a cross-community bridge._
- **What connects `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk` to the rest of the system?**
  _86 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Catalog Service Namespaces` be split into smaller, more focused modules?**
  _Cohesion score 0.09877264757451783 - nodes in this community are weakly interconnected._
- **Why does `SyslogLogger` connect `Syslog Logging Provider` to `Catalog Service Namespaces`, `Catalog API Controllers`?**
  _High betweenness centrality (0.031) - this node is a cross-community bridge._
- **Should `Controller Cheat Sheet Concepts` be split into smaller, more focused modules?**
  _Cohesion score 0.0531986531986532 - nodes in this community are weakly interconnected._
- **Why does `Catalog.Persistence.Database` connect `Catalog Service Namespaces` to `EF Core Migrations`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Should `Catalog API Controllers` be split into smaller, more focused modules?**
  _Cohesion score 0.08416389811738649 - nodes in this community are weakly interconnected._