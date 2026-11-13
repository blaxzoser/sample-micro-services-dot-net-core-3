# Graph Report - sample-micro-services-dot-net-core-3  (2026-11-13)

## Corpus Check
- 78 files · ~30,604 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 4 file(s) not represented in the graph (top: (none) 2, .http 2)

## Summary
- 581 nodes · 966 edges · 32 communities (27 shown, 5 thin omitted)
- Extraction: 94% EXTRACTED · 6% INFERRED · 0% AMBIGUOUS · INFERRED: 55 edges (avg confidence: 0.88)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `80377f9e`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Catalog Service Namespaces
- Controller Cheat Sheet Concepts
- Task
- Product
- Syslog Logging Provider
- ProductInStockUpdateItem
- Launch Settings
- microsoft_entityframeworkcore
- Host Startup & Pipeline
- Api & Logging Packages
- Paging & Collection Packages
- Domain & Persistence Packages
- Common & Handlers Packages
- Queries & Mapping Packages
- ETag & Concurrency Exercises
- API Versioning
- Order State Machine Exercise
- .GetById
- http
- Entities.cs
- Customer.Api.csproj
- http
- AppDbContext
- CLAUDE.md
- ProductInStockUpdateCommnand
- Feedback.Api.Tests
- AppDbContext
- Product
- ApplicationDbContext
- Feedback.Api
- Feedback.Api/Program.cs

## God Nodes (most connected - your core abstractions)
1. `AppDbContext` - 19 edges
2. `SubmitFeedbackTests` - 19 edges
3. `ApplicationDbContext` - 14 edges
4. `DataCollection` - 14 edges
5. `ProductDto` - 13 edges
6. `Product` - 12 edges
7. `Feedback.Api.Tests` - 11 edges
8. `SyslogLogLevel` - 11 edges
9. `SyslogLogger` - 10 edges
10. `ProductInStock` - 9 edges

## Surprising Connections (you probably didn't know these)
- `Misspelled names` --references--> `IProductInStockQueryService`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Service.Queries/IProductInStockQueryService.cs
- `Misspelled names` --references--> `IProductQueryService`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Service.Queries/IProductQueryService.cs
- `Misspelled names` --references--> `ProductInStockUpdateCommnand`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Services.EventHandlers/Commands/ProductInStockUpdateCommnand.cs
- `Known issues` --references--> `ProductInStockUpdateStockEventHandler`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Services.EventHandlers/ProductInStockUpdateStockEventHandler.cs
- `Misspelled names` --references--> `ProducInStockAction`  [INFERRED]
  CLAUDE.md → src/Services/Catalog/Catalog.Common/Enums.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Catalog CQRS read/write request flow** — readme_catalog_api, readme_catalog_service_queries, readme_mediatr, readme_catalog_services_eventhandlers, readme_sql_server [EXTRACTED 1.00]
- **409 Conflict business-state exercises** — doc_aspnet_mock_interview_exercises_post_3_duplicate_sku, doc_aspnet_mock_interview_exercises_post_4_pay_order, doc_aspnet_mock_interview_exercises_patch_3_state_machine, doc_aspnet_mock_interview_exercises_patch_5_concurrent_stock, doc_aspnet_mock_interview_exercises_delete_3_block_dependents [EXTRACTED 1.00]
- **Exercises requiring atomic check-then-write on the shared Db** — doc_aspnet_mock_interview_exercises_starter_kit_db, doc_aspnet_mock_interview_exercises_post_3_duplicate_sku, doc_aspnet_mock_interview_exercises_put_5_optimistic_concurrency, doc_aspnet_mock_interview_exercises_patch_5_concurrent_stock [INFERRED 0.85]

## Communities (32 total, 5 thin omitted)

### Community 0 - "Catalog Service Namespaces"
Cohesion: 0.11
Nodes (12): Common.Logging, Catalog.Service.Queries, Catalog.Api.Controllers, Catalog.Service.Queries.DTOs, Catalog.Persistence.Database.Configuration, Catalog.Services.EventHandlers.Commands, Service.Common.Paging, Catalog.Domain (+4 more)

### Community 1 - "Controller Cheat Sheet Concepts"
Cohesion: 0.05
Nodes (52): Auth Status Codes (401 vs 403, Authorize policies), Model Binding Sources (FromRoute/Query/Body/Header), NotFoundException / BusinessRuleException, ASP.NET Core Controller Cheat Sheet, Code-Review Notes on This Repo, Full CRUD Controller with CQRS + MediatR, ExceptionMiddleware (global exception to status code mapping), FluentValidation / MediatR IPipelineBehavior (+44 more)

### Community 2 - "Task"
Cohesion: 0.06
Nodes (25): Known issues, ProductController, ProductInStockController, ProductDto, Description, Name, Price, ProductId (+17 more)

### Community 3 - "Product"
Cohesion: 0.18
Nodes (6): ProductInStock, ProductId, ProductInStockId, Stock, ProductInStockConfiguration, ApplicationDbContextModelSnapshot

### Community 4 - "Syslog Logging Provider"
Cohesion: 0.09
Nodes (14): NoopDisposable, SyslogLogger, SyslogLoggerExtensions, SyslogLoggerProvider, SyslogLogLevel, Alert, Critical, Debug (+6 more)

### Community 5 - "ProductInStockUpdateItem"
Cohesion: 0.18
Nodes (10): Misspelled names, Catalog.Common, Enums, ProducInStockAction, Add, Substract, ProductInStockUpdateItem, Action (+2 more)

### Community 6 - "Launch Settings"
Cohesion: 0.10
Nodes (20): applicationUrl, commandName, environmentVariables, launchBrowser, launchUrl, ASPNETCORE_ENVIRONMENT, commandName, environmentVariables (+12 more)

### Community 8 - "microsoft_entityframeworkcore"
Cohesion: 0.05
Nodes (13): Feedback.Api.DTO, Catalog.Persistence.Database.Migrations, Catalog.Persistence.Database, Feedback.Api.Data, Feedback.Api.Tests, Customer.Api.Data, Feedback.Api.Models, Initialize (+5 more)

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
Cohesion: 0.06
Nodes (23): Customer.Api.DTO, Customer.Api.Models, Feedback.Api.Controllers, Customer.Api.Controllers, CustomerDto, CustomersController, TodosController, CustomerByAgeDto (+15 more)

### Community 20 - "http"
Cohesion: 0.18
Nodes (10): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, profiles (+2 more)

### Community 21 - "Entities.cs"
Cohesion: 0.05
Nodes (42): CartItem, CartId, ProductId, CustomerEntity, Balance, Birthday, Id, Name (+34 more)

### Community 22 - "Customer.Api.csproj"
Cohesion: 0.40
Nodes (4): net9.0, Microsoft.EntityFrameworkCore.Design (9.0.23), Microsoft.EntityFrameworkCore.SqlServer (9.0.23), Microsoft.NET.Sdk.Web

### Community 23 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 24 - "AppDbContext"
Cohesion: 0.23
Nodes (8): AppDbContext, CartItems, Customers, Orders, Payments, Products, Settings, Users

### Community 25 - "CLAUDE.md"
Cohesion: 0.18
Nodes (9): Adding a feature, Architecture, Commands, Configuration, Conventions, Docs, graphify, Project (+1 more)

### Community 26 - "ProductInStockUpdateCommnand"
Cohesion: 0.21
Nodes (8): ProductCreateCommand, Description, Name, Price, ProductInStockUpdateCommnand, Items, ProductCreateEventHandler, ProductInStockUpdateStockEventHandler

### Community 27 - "Feedback.Api.Tests"
Cohesion: 0.18
Nodes (9): coverlet.collector (6.0.2), Microsoft.AspNetCore.Mvc.Testing (9.0.22), Microsoft.EntityFrameworkCore.InMemory (9.0.23), Microsoft.NET.Test.Sdk (17.12.0), xunit (2.9.2), xunit.runner.visualstudio (2.8.2), Feedback.Api.Tests, net9.0 (+1 more)

### Community 29 - "Product"
Cohesion: 0.29
Nodes (7): Product, Description, Name, Price, ProductId, Stock, ProductConfiguration

### Community 30 - "ApplicationDbContext"
Cohesion: 0.38
Nodes (5): ApplicationDbContext, Products, Stocks, IProductInStockQueryService, ProductInStockQueryService

### Community 31 - "Feedback.Api"
Cohesion: 0.33
Nodes (6): Microsoft.AspNetCore.OpenApi (9.0.22), net9.0, Microsoft.EntityFrameworkCore.Design (9.0.23), Microsoft.EntityFrameworkCore.SqlServer (9.0.23), Microsoft.NET.Sdk.Web, Feedback.Api

## Knowledge Gaps
- **158 isolated node(s):** `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk`, `Emergency`, `Alert` (+153 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 250 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Customer.Api.Models` connect `.GetById` to `microsoft_entityframeworkcore`, `Entities.cs`?**
  _High betweenness centrality (0.070) - this node is a cross-community bridge._
- **What connects `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk` to the rest of the system?**
  _158 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Catalog Service Namespaces` be split into smaller, more focused modules?**
  _Cohesion score 0.10707070707070707 - nodes in this community are weakly interconnected._
- **Why does `ApplicationDbContext` connect `ApplicationDbContext` to `Task`, `Product`, `microsoft_entityframeworkcore`, `ProductInStockUpdateCommnand`, `AppDbContext`, `Product`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **Should `Controller Cheat Sheet Concepts` be split into smaller, more focused modules?**
  _Cohesion score 0.0531986531986532 - nodes in this community are weakly interconnected._
- **Why does `AppDbContext` connect `AppDbContext` to `microsoft_entityframeworkcore`, `AppDbContext`?**
  _High betweenness centrality (0.033) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.056265984654731455 - nodes in this community are weakly interconnected._