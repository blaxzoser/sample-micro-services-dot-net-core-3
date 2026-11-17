# Graph Report - sample-micro-services-dot-net-core-3  (2026-11-16)

## Corpus Check
- 143 files · ~38,353 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 9 file(s) not represented in the graph (top: .http 7, (none) 2)

## Summary
- 951 nodes · 1793 edges · 78 communities (53 shown, 25 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 137 edges (avg confidence: 0.86)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c2298276`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Catalog Service Namespaces
- Controller Cheat Sheet Concepts
- DataCollection
- Task
- Syslog Logging Provider
- http
- Launch Settings
- Initialize
- Products.Api.Tests
- Api & Logging Packages
- Paging & Collection Packages
- Domain & Persistence Packages
- Common & Handlers Packages
- Queries & Mapping Packages
- ETag & Concurrency Exercises
- OrderSummaryDto
- API Versioning
- Order State Machine Exercise
- .GetByAge
- http
- Entities.cs
- Customer.Api.csproj
- http
- AppDbContext
- ControllerBase
- ProductInStockUpdateCommnand
- Feedback.Api.Tests
- ApplicationDbContext
- .Create
- WeatherForecast
- Feedback.Api
- Feedback.Api/Program.cs
- http
- Customer.Api.Models
- Products.Api
- Products.Api.Tests
- http
- http
- http
- .AddProduct
- CLAUDE.md
- ProductInStockUpdateItem
- WeatherForecast
- WeatherForecast
- ProductDto
- WeatherForecast
- ProductController
- Feedback.Api.DTO
- WeatherForecast
- Orders.Api
- Payments.api
- Reports.api
- Users.api
- .ReadJsonAsync
- .Json
- Fact
- Post7_InMemoryNextIdLockTests
- FeedbackApiFactory
- Product
- Delete5_ApiKeyProtectedDeleteTests
- ApplicationDbContext
- Patch5_ConcurrentStockTests
- AppDbContext
- .SendAsync
- Patch4_JsonPatchTests
- Put1_ReplaceProductTests
- .ConfigureServices
- ModelBuilder
- .GetPagedAsync

## God Nodes (most connected - your core abstractions)
1. `ProductsApiTest` - 25 edges
2. `Post7_InMemoryNextIdLockTests` - 22 edges
3. `ProductsApiFactory` - 22 edges
4. `AppDbContext` - 19 edges
5. `SubmitFeedbackTests` - 19 edges
6. `Products.Api.Tests` - 17 edges
7. `ApplicationDbContext` - 14 edges
8. `Post2_CreateProductTests` - 14 edges
9. `DataCollection` - 14 edges
10. `ProductDto` - 13 edges

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

## Communities (78 total, 25 thin omitted)

### Community 0 - "Catalog Service Namespaces"
Cohesion: 0.07
Nodes (18): Common.Logging, Products.Api.Data, Catalog.Service.Queries, Catalog.Persistence.Database.Migrations, Catalog.Persistence.Database, Catalog.Api.Controllers, Catalog.Service.Queries.DTOs, Catalog.Persistence.Database.Configuration (+10 more)

### Community 1 - "Controller Cheat Sheet Concepts"
Cohesion: 0.05
Nodes (52): Auth Status Codes (401 vs 403, Authorize policies), Model Binding Sources (FromRoute/Query/Body/Header), NotFoundException / BusinessRuleException, ASP.NET Core Controller Cheat Sheet, Code-Review Notes on This Repo, Full CRUD Controller with CQRS + MediatR, ExceptionMiddleware (global exception to status code mapping), FluentValidation / MediatR IPipelineBehavior (+44 more)

### Community 2 - "DataCollection"
Cohesion: 0.19
Nodes (10): ProductInStockDto, ProductId, ProductInStockId, Stock, DataCollection, HasItem, Items, Page (+2 more)

### Community 4 - "Syslog Logging Provider"
Cohesion: 0.06
Nodes (17): NoopDisposable, SyslogLogger, SyslogLoggerExtensions, SyslogLoggerProvider, SyslogLogLevel, Alert, Critical, Debug (+9 more)

### Community 5 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 6 - "Launch Settings"
Cohesion: 0.10
Nodes (20): applicationUrl, commandName, environmentVariables, launchBrowser, launchUrl, ASPNETCORE_ENVIRONMENT, commandName, environmentVariables (+12 more)

### Community 9 - "Products.Api.Tests"
Cohesion: 0.14
Nodes (3): Products.Api.Tests, Feedback.Api.Tests, DtoMapperExtension

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

### Community 16 - "OrderSummaryDto"
Cohesion: 0.17
Nodes (10): Customer.Api.DTO, CustomerByAgeDto, Age, CustomerName, TotalPayments, OrderSummaryDto, Age, CustomerName (+2 more)

### Community 19 - ".GetByAge"
Cohesion: 0.16
Nodes (5): CustomersController, TodosController, Todo, InMemoryProductStore, Products

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
Cohesion: 0.20
Nodes (9): Customer.Api.Data, AppDbContext, CartItems, Customers, Orders, Payments, Products, Settings (+1 more)

### Community 25 - "ControllerBase"
Cohesion: 0.20
Nodes (5): Products.Api.Controllers, Orders.Api.Controllers, WeatherForecastController, MemoryProductsController, WeatherForecastController

### Community 26 - "ProductInStockUpdateCommnand"
Cohesion: 0.21
Nodes (8): ProductCreateCommand, Description, Name, Price, ProductInStockUpdateCommnand, Items, ProductCreateEventHandler, ProductInStockUpdateStockEventHandler

### Community 27 - "Feedback.Api.Tests"
Cohesion: 0.18
Nodes (9): Microsoft.EntityFrameworkCore.InMemory (9.0.23), Feedback.Api.Tests, net9.0, coverlet.collector (6.0.2), Microsoft.AspNetCore.Mvc.Testing (9.0.22), Microsoft.NET.Test.Sdk (17.12.0), xunit (2.9.2), xunit.runner.visualstudio (2.8.2) (+1 more)

### Community 28 - "ApplicationDbContext"
Cohesion: 0.15
Nodes (12): Product, Description, Name, Price, ProductId, Stock, ProductInStock, ProductId (+4 more)

### Community 29 - ".Create"
Cohesion: 0.15
Nodes (7): Feedback.Api.Controllers, FeedbackController, FeedbackEntry, Id, Message, Rating, ReceivedAt

### Community 30 - "WeatherForecast"
Cohesion: 0.17
Nodes (8): Reports.api, Reports.api.Controllers, WeatherForecastController, WeatherForecast, Date, Summary, TemperatureC, TemperatureF

### Community 31 - "Feedback.Api"
Cohesion: 0.33
Nodes (6): net9.0, Microsoft.AspNetCore.OpenApi (9.0.22), Microsoft.EntityFrameworkCore.Design (9.0.23), Microsoft.EntityFrameworkCore.SqlServer (9.0.23), Microsoft.NET.Sdk.Web, Feedback.Api

### Community 33 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 34 - "Customer.Api.Models"
Cohesion: 0.40
Nodes (3): Customer.Api.Models, Customer.Api.Controllers, CustomerDto

### Community 35 - "Products.Api"
Cohesion: 0.33
Nodes (6): net9.0, Microsoft.AspNetCore.OpenApi (9.0.22), Microsoft.EntityFrameworkCore.Design (9.0.23), Microsoft.EntityFrameworkCore.SqlServer (9.0.23), Microsoft.NET.Sdk.Web, Products.Api

### Community 36 - "Products.Api.Tests"
Cohesion: 0.20
Nodes (8): Products.Api.Tests, net9.0, coverlet.collector (6.0.2), Microsoft.AspNetCore.Mvc.Testing (9.0.22), Microsoft.NET.Test.Sdk (17.12.0), xunit (2.9.2), xunit.runner.visualstudio (2.8.2), Microsoft.NET.Sdk

### Community 37 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 38 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 39 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 40 - ".AddProduct"
Cohesion: 0.15
Nodes (9): Products.Api.Dtos, ProductsController, ProductDto, Category, Id, Name, Price, Sku (+1 more)

### Community 41 - "CLAUDE.md"
Cohesion: 0.17
Nodes (10): Adding a feature, Architecture, Commands, Configuration, Conventions, Docs, graphify, Known issues (+2 more)

### Community 42 - "ProductInStockUpdateItem"
Cohesion: 0.18
Nodes (10): Misspelled names, Catalog.Common, Enums, ProducInStockAction, Add, Substract, ProductInStockUpdateItem, Action (+2 more)

### Community 43 - "WeatherForecast"
Cohesion: 0.25
Nodes (6): Orders.Api, WeatherForecast, Date, Summary, TemperatureC, TemperatureF

### Community 44 - "WeatherForecast"
Cohesion: 0.18
Nodes (8): Users.api.Controllers, Users.api, WeatherForecastController, WeatherForecast, Date, Summary, TemperatureC, TemperatureF

### Community 45 - "ProductDto"
Cohesion: 0.28
Nodes (6): ProductDto, Description, Name, Price, ProductId, Stock

### Community 46 - "WeatherForecast"
Cohesion: 0.18
Nodes (8): Payments.api, Payments.api.Controllers, WeatherForecastController, WeatherForecast, Date, Summary, TemperatureC, TemperatureF

### Community 48 - "Feedback.Api.DTO"
Cohesion: 0.29
Nodes (5): Feedback.Api.DTO, FeedbackRequest, Message, Rating, FeedbackResponse

### Community 49 - "WeatherForecast"
Cohesion: 0.29
Nodes (6): Products.Api, WeatherForecast, Date, Summary, TemperatureC, TemperatureF

### Community 50 - "Orders.Api"
Cohesion: 0.50
Nodes (4): net9.0, Microsoft.AspNetCore.OpenApi (9.0.22), Microsoft.NET.Sdk.Web, Orders.Api

### Community 51 - "Payments.api"
Cohesion: 0.50
Nodes (4): net9.0, Microsoft.AspNetCore.OpenApi (9.0.22), Microsoft.NET.Sdk.Web, Payments.api

### Community 52 - "Reports.api"
Cohesion: 0.50
Nodes (4): net9.0, Microsoft.AspNetCore.OpenApi (9.0.22), Microsoft.NET.Sdk.Web, Reports.api

### Community 53 - "Users.api"
Cohesion: 0.50
Nodes (4): net9.0, Microsoft.AspNetCore.OpenApi (9.0.22), Microsoft.NET.Sdk.Web, Users.api

### Community 54 - ".ReadJsonAsync"
Cohesion: 0.13
Nodes (6): Get1_ProductByIdTests, Get2_FilterByCategoryTests, Get3_PaginationTests, ProductsApiFactory, ProductsApiTest, Put4_UpdatePriceTests

### Community 64 - "Fact"
Cohesion: 0.19
Nodes (3): Delete1_DeleteProductTests, Delete4_SoftDeleteTests, Put5_OptimisticConcurrencyTests

### Community 66 - "FeedbackApiFactory"
Cohesion: 0.15
Nodes (4): Program, FeedbackApiFactory, Program, MemoryApiFactory

### Community 67 - "Product"
Cohesion: 0.20
Nodes (9): Product, Category, Id, IsDeleted, Name, Price, Sku, Stock (+1 more)

### Community 69 - "ApplicationDbContext"
Cohesion: 0.28
Nodes (5): ApplicationDbContext, Products, Stocks, AppDbContext, Feedbacks

### Community 75 - ".ConfigureServices"
Cohesion: 0.47
Nodes (4): IProductInStockQueryService, IProductQueryService, ProductInStockQueryService, ProductQueryService

## Knowledge Gaps
- **218 isolated node(s):** `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk`, `Emergency`, `Alert` (+213 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 371 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **25 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Customer.Api.Models` connect `Customer.Api.Models` to `Catalog Service Namespaces`, `Entities.cs`?**
  _High betweenness centrality (0.037) - this node is a cross-community bridge._
- **What connects `netcoreapp3.1`, `Microsoft.Extensions.Logging (5.0.0)`, `Microsoft.NET.Sdk` to the rest of the system?**
  _218 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Catalog Service Namespaces` be split into smaller, more focused modules?**
  _Cohesion score 0.0653417645287564 - nodes in this community are weakly interconnected._
- **Why does `ProductsApiFactory` connect `.ReadJsonAsync` to `Fact`, `Catalog Service Namespaces`, `FeedbackApiFactory`, `Delete5_ApiKeyProtectedDeleteTests`, `Patch5_ConcurrentStockTests`, `AppDbContext`, `.SendAsync`, `Patch4_JsonPatchTests`, `Put1_ReplaceProductTests`, `.Json`?**
  _High betweenness centrality (0.032) - this node is a cross-community bridge._
- **Should `Controller Cheat Sheet Concepts` be split into smaller, more focused modules?**
  _Cohesion score 0.0531986531986532 - nodes in this community are weakly interconnected._
- **Why does `ApplicationDbContext` connect `ApplicationDbContext` to `Catalog Service Namespaces`, `.ConfigureServices`, `ModelBuilder`, `ProductInStockUpdateCommnand`, `ApplicationDbContext`?**
  _High betweenness centrality (0.021) - this node is a cross-community bridge._
- **Should `Syslog Logging Provider` be split into smaller, more focused modules?**
  _Cohesion score 0.06387921022067364 - nodes in this community are weakly interconnected._