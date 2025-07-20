<h2 style="color:brown">Manage ECB Rates and Wallets using Web API,Backgroud Job,Redis and rate limiting mechanism </h2>

- **ΕCB.Api** – Hosts API endpoints for wallets and currency rates, providing clean separation for RESTful interactions.
- **ECB.Application** – Contains business logic, use cases, and service interfaces, ensuring the core logic is independent of frameworks and UI.
- **ECB.Domain** – Defines the core domain models and business rules, making the solution highly testable and extensible.
- **ΕCB.Infrastructure.Gateway** – Implements the gateway that fetches the rates grom ECB

- **ΕCB.Infrastructure.Gateway.Services** – Implements the periodic job that updates the currency rates
- **ECB.Persistence** – Handles data access with Entity Framework Core and maintains database context, migrations, and data seeding.
- **ECB.Api.Tests** – Provides unit tests for web api calls business logic and validation, ensuring high code coverage and robustness.
- **ECB.GatewayService.Tests** – Provides basic unit tests for  gateway service workflow.

This layered and modular architecture allows for clear separation of concerns, easy testing, and future scalability.

The solution is ready to run with `dotnet run`, includes database migration scripts and sample data,

The solution  has two startup projects, the  ΕCB.Api that will start on https://localhost:7169

and  ECB.Infrastructure.Gateway.Services

To build the solution and run the those two projects execute the script BuildRun.ps1 that is located on the root folder of the solution

To just run those two projects execute the script  Run.ps1  that is located on the root folder of the solution  

 
