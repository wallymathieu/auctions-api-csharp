# auctions-api-simple

Simple Auction API

There are currently these main implementations:

- [main](https://github.com/wallymathieu/auctions-api-csharp/tree/main) is the core main implementation
- [command-handlers-infrastructure](https://github.com/wallymathieu/auctions-api-csharp/tree/command-handlers-infrastructure) is the implementation but without hand written command handlers that binds to the entity methods

## Getting started

First copy .env sample to a new .env file

```bash
cp .env.sample .env
```

Then do change the password.

To start the database run:

```bash
docker compose up -d db
```

To start azurite run:

```bash
docker compose up -d azurite
```

To start redis run:

```bash
docker compose up -d redis
```

To build the app run:

```bash
cd src/Auctions.WebApi
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer
```

To run api/func/migrations locally you need the following env:

```bash
source .env
export ConnectionStrings__DefaultConnection="Server=localhost;Database=master;TrustServerCertificate=true;MultipleActiveResultSets=true;User Id=sa;Password=${SA_PASSWORD}"
export ConnectionStrings__Redis="localhost"
export ConnectionStrings__AzureStorage="UseDevelopmentStorage=true"

export AzureWebJobsStorage="UseDevelopmentStorage=true"
```

If you want to run inside dev containers then the setup will be slightly different (since it should be the same environment as you have if you the apps through docker compose):

```bash
. .devcontainer/env.sh
```

To run migrations and api using above environment:

```bash
dotnet tool restore
cd src/Auctions.WebApi
dotnet ef database update
dotnet run
```

In the frontend app you need to specify the frontend db context

```bash
cd src/Auctions.Frontend
dotnet ef database update --context Wallymathieu.Auctions.Frontend.Data.FrontendDbContext
```

To run Azure Functions locally using above environment:

```bash
cd src/Auctions.AzureFunctions
func start
```

## Auth

The API assumes that you have auth middleware in front of the app.

Either the decoded JWT in the `x-jwt-payload` header or specify an encoded claims principal by using configuration value in `PrincipalHeader`, such as `x-ms-client-principal`.

## Add migration

```bash
dotnet ef migrations add NewMigration --project ./src/Auctions.Infrastructure/Auctions.Infrastructure.csproj --startup-project ./src/Auctions.WebApi/Auctions.WebApi.csproj
```

## Testing

To run tests:

```bash
dotnet test
```

To run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

To generate coverage report:
```bash
dotnet tool restore
dotnet reportgenerator -reports:"./test/Tests/TestResults/*/coverage.cobertura.xml" -targetdir:./test/Tests/TestResults/CoverageReport
# then open|start ./test/Tests/TestResults/CoverageReport/index.html
```

## Inspiration

The main inspiration for the architecture of the API is found in this book:

- [Clean Architecture](https://www.goodreads.com/en/book/show/18043011)

Note that there are many variants of "the clean architecture" described in the .net space with different interpretations of what it means to implement this architecture.