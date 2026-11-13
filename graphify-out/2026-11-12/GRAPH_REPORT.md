# Graph Report - sample-micro-services-dot-net-core-3  (2026-11-12)

## Corpus Check
- 61 files · ~26,805 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 3 file(s) not represented in the graph (top: (none) 2, .http 1)

## Summary
- 469 nodes · 788 edges · 24 communities (19 shown, 5 thin omitted)
- Extraction: 93% EXTRACTED · 7% INFERRED · 0% AMBIGUOUS · INFERRED: 52 edges (avg confidence: 0.88)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `80377f9e`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Catalog Service Namespaces
- Controller Cheat Sheet Concepts
- Task
- EF Core Persistence & Product
- Syslog Logging Provider
- ProductInStockUpdateCommnand
- Launch Settings
- 20210224222723_Initialize.Designer.cs
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
- .GetById
- http
- Entities.cs
- Customer.Api.csproj
- Customer.Api/Program.cs

## God Nodes (most connected - your core abstractions)
1. `AppDbContext` - 19 edges
2. `ApplicationDbContext` - 14 edges
3. `DataCollection` - 14 edges
4. `ProductDto` - 13 edges
5. `Product` - 12 edges
6. `SyslogLogLevel` - 11 edges
7. `SyslogLogger` - 10 edges
8. `ProductInStock` - 9 edges
9. `ProductInStockUpdateCommnand` - 9 edges
10. `Product` - 9 edges

## Surprising Connections (you probably didn't know these)
- `Misspelled names` --references--> `IProductInStockQueryService`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Service.Queries/IProductInStockQueryService.cs
- `Misspelled names` --references--> `IProductQueryService`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Service.Queries/IProductQueryService.cs
- `Misspelled names` --references--> `ProducInStockAction`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Common/Enums.cs
- `Misspelled names` --references--> `ProductInStockUpdateCommnand`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Services.EventHandlers/Commands/ProductInStockUpdateCommnand.cs
- `Known issues` --references--> `ProductInStockUpdateStockEventHandler`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Services.EventHandlers/ProductInStockUpdateStockEventHandler.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Catalog CQRS read/write request flow** — readme_catalog_api, readme_catalog_service_queries, readme_mediatr, readme_catalog_services_eventhandlers, readme_sql_server [EXTRACTED 1.00]
- **409 Conflict business-state exercises** — doc_aspnet_mock_interview_exercises_post_3_duplicate_sku, doc_aspnet_mock_interview_exercises_post_4_pay_order, doc_aspnet_mock_interview_exercises_patch_3_state_machine, doc_aspnet_mock_interview_exercises_patch_5_concurrent_stock, doc_aspnet_mock_interview_exercises_delete_3_block_dependents [EXTRACTED 1.00]
- **Exercises requiring atomic check-then-write on the shared Db** — doc_aspnet_mock_interview_exercises_starter_kit_db, doc_aspnet_mock_interview_exercises_post_3_duplicate_sku, doc_aspnet_mock_interview_exercises_put_5_optimistic_concurrency, doc_aspnet_mock_interview_exercises_patch_5_concurrent_stock [INFERRED 0.85]

## Communities (24 total, 5 thin omitted)

### Community 0 - "Catalog Service Namespaces"
Cohesion: 0.10
Nodes (13): Common.Logging, Catalog.Service.Queries, Catalog.Persistence.Database, Catalog.Api.Controllers, Catalog.Service.Queries.DTOs, Catalog.Persistence.Database.Configuration, Catalog.Services.EventHandlers.Commands, Service.Common.Paging (+5 more)

### Community 1 - "Controller Cheat Sheet Concepts"
Cohesion: 0.05
Nodes (52): Auth Status Codes (401 vs 403, Authorize policies), Model Binding Sources (FromRoute/Query/Body/Header), NotFoundException / BusinessRuleException, ASP.NET Core Controller Cheat Sheet, Code-Review Notes on This Repo, Full CRUD Controller with CQRS + MediatR, ExceptionMiddleware (global exception to status code mapping), FluentValidation / MediatR IPipelineBehavior (+44 more)

### Community 2 - "Task"
Cohesion: 0.09
Nodes (23): ProductController, ProductInStockController, ProductDto, Description, Name, Price, ProductId, Stock (+15 more)

### Community 3 - "EF Core Persistence & Product"
Cohesion: 0.08
Nodes (23): Product, Description, Name, Price, ProductId, Stock, ProductInStock, ProductId (+15 more)

### Community 4 - "Syslog Logging Provider"
Cohesion: 0.09
Nodes (15): Known issues, NoopDisposable, SyslogLogger, SyslogLoggerExtensions, SyslogLoggerProvider, SyslogLogLevel, Alert, Critical (+7 more)

### Community 5 - "ProductInStockUpdateCommnand"
Cohesion: 0.07
Nodes (26): Adding a feature, Architecture, Commands, Configuration, Conventions, Docs, graphify, Misspelled names (+18 more)

### Community 6 - "Launch Settings"
Cohesion: 0.10
Nodes (20): applicationUrl, commandName, environmentVariables, launchBrowser, launchUrl, ASPNETCORE_ENVIRONMENT, commandName, environmentVariables (+12 more)

### Community 8 - "20210224222723_Initialize.Designer.cs"
Cohesion: 0.16
Nodes (3): Catalog.Persistence.Database.Migrations, Initialize, ApplicationDbContextModelSnapshot

### Community 9 - "Host Startup & Pipeline"
Cohesion: 0.20
Nodes (3): Program, Startup, Configuration

### Community 10 - "Api & Logging Packages"
Cohesion: 0.20
Nodes (8): MediatR.Extensions.Microsoft.DependencyInjection (9.0.0), Microsoft.EntityFrameworkCore.Tools (5.0.3), Microsoft.Extensions.Logging (5.0.0), netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, MediatR (9.0.0), Microsoft.NET.Sdk.Web

### Community 11 - "Paging & Collection Packages"
Cohesion: 0.29
Nodes (5): Microsoft.EntityFrameworkCore (5.0.3), netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, Microsoft.NET.Sdk

### Community 12 - "Domain & Persistence Packages"
Cohesion: 0.29
Nodes (5): netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, Microsoft.EntityFrameworkCore.SqlServer (5.0.3), Microsoft.NET.Sdk

### Community 13 - "Common & Handlers Packages"
Cohesion: 0.29
Nodes (5): netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, MediatR (9.0.0), Microsoft.NET.Sdk

### Community 14 - "Queries & Mapping Packages"
Cohesion: 0.33
Nodes (4): netcoreapp3.1, Microsoft.NET.Sdk, netcoreapp3.1, Microsoft.NET.Sdk

### Community 15 - "ETag & Concurrency Exercises"
Cohesion: 0.67
Nodes (4): Conditional GET with ETag (304), Optimistic Concurrency (412 / 409, If-Match), GET-5 Conditional GET with ETag (304), PUT-5 Optimistic Concurrency with If-Match (412/428)

### Community 19 - ".GetById"
Cohesion: 0.09
Nodes (9): Customer.Api.Models, Customer.Api.Controllers, Customer.Api.Data, DefaultController, CustomerDto, CustomerPaymentsSummaryDto, CustomersController, TodosController (+1 more)

### Community 20 - "http"
Cohesion: 0.18
Nodes (10): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, profiles (+2 more)

### Community 21 - "Entities.cs"
Cohesion: 0.05
Nodes (42): CartItem, CartId, ProductId, CustomerEntity, Balance, Birthday, Id, Name (+34 more)

### Community 22 - "Customer.Api.csproj"
Cohesion: 0.40
Nodes (4): net9.0, Microsoft.EntityFrameworkCore.Design (9.0.23), Microsoft.EntityFrameworkCore.SqlServer (9.0.23), Microsoft.NET.Sdk.Web

## Knowledge Gaps
- **131 isolated node(s):** `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk`, `Emergency`, `Alert` (+126 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 204 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Customer.Api.Models` connect `.GetById` to `Entities.cs`?**
  _High betweenness centrality (0.084) - this node is a cross-community bridge._
- **What connects `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk` to the rest of the system?**
  _131 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Catalog Service Namespaces` be split into smaller, more focused modules?**
  _Cohesion score 0.09830508474576272 - nodes in this community are weakly interconnected._
- **Why does `ApplicationDbContext` connect `EF Core Persistence & Product` to `Catalog Service Namespaces`, `Task`, `Syslog Logging Provider`, `ProductInStockUpdateCommnand`?**
  _High betweenness centrality (0.060) - this node is a cross-community bridge._
- **Should `Controller Cheat Sheet Concepts` be split into smaller, more focused modules?**
  _Cohesion score 0.0531986531986532 - nodes in this community are weakly interconnected._
- **Why does `AppDbContext` connect `EF Core Persistence & Product` to `.GetById`?**
  _High betweenness centrality (0.040) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.08562367864693446 - nodes in this community are weakly interconnected._